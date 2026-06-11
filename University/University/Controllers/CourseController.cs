using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dto;
using University.Models;
using University.ServiceInterface;
using University.ViewModel;
using University.ViewModel.CoursesVM;

namespace University.Controllers
{
    public class CourseController : Controller
    {
        private readonly UniversityContext _context;
        private readonly IFileServices _fileServices;

        public CourseController(UniversityContext context, IFileServices fileServices)
        {
            _context = context;
            _fileServices = fileServices;
        }

        public async Task<IActionResult> Index()
        {
            var course = _context.Courses
                .Include(c => c.Departments)
                .Select(c => new CourseIndexViewModel
                {
                    CourseId = c.CourseId,
                    Credits = c.Credits,
                    Title = c.Title,
                    DepartmentId = c.DepartmentId,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments.Name
                    }
                });

            return View(course);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vm = await _context.Courses
                .Where(c => c.CourseId == id)
                .Select(c => new CourseUpdateViewModel
                {
                    CourseId = c.CourseId,
                    Credits = c.Credits,
                    Title = c.Title,
                    DepartmentId = c.DepartmentId,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments != null ? c.Departments.Name : string.Empty
                    }
                })
                .FirstOrDefaultAsync();

            if (vm == null) return NotFound();

            vm.Images = await _context.FileToApi
                .Where(f => f.CourseId == id)
                .Select(f => new ImageViewModel
                {
                    Id = f.Id,
                    ExistingFilepath = f.ExistingFilepath
                })
                .ToListAsync();

            PopulateDepartmentDropDownList(vm.DepartmentId);
            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseId = vm.CourseId,
                    Title = vm.Title,
                    Credits = vm.Credits,
                    DepartmentId = vm.DepartmentId
                };

                _context.Update(course);
                await _context.SaveChangesAsync();

                if (vm.Files != null && vm.Files.Count > 0)
                {
                    var dto = new CourseDto
                    {
                        CourseId = vm.CourseId,
                        Title = vm.Title ?? string.Empty,
                        Credits = vm.Credits,
                        DepartmentId = vm.DepartmentId,
                        Files = vm.Files
                    };

                    _fileServices.FilesToApi(dto, course);
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateDepartmentDropDownList(vm.DepartmentId);
            return View(vm);
        }

        // GET: Create
        public IActionResult Create()
        {
            PopulateDepartmentDropDownList();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseId = vm.CourseId,
                    Title = vm.Title,
                    Credits = vm.Credits,
                    DepartmentId = vm.DepartmentId
                };

                _context.Add(course);
                await _context.SaveChangesAsync();

                if (vm.Files != null && vm.Files.Count > 0)
                {
                    var dto = new CourseDto
                    {
                        CourseId = vm.CourseId,
                        Title = vm.Title ?? string.Empty,
                        Credits = vm.Credits,
                        DepartmentId = vm.DepartmentId,
                        Files = vm.Files
                    };

                    _fileServices.FilesToApi(dto, course);
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateDepartmentDropDownList(vm.DepartmentId);
            return View(vm);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Departments)
                .Where(c => c.CourseId == id)
                .Select(c => new CourseDetailsViewModel
                {
                    CourseId = c.CourseId,
                    Credits = c.Credits,
                    Title = c.Title,
                    DepartmentId = c.DepartmentId,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments.Name
                    }
                })
                .FirstOrDefaultAsync();

            if (course == null) return NotFound();

            course.Images = await _context.FileToApi
                .Where(f => f.CourseId == id)
                .Select(f => new ImageViewModel
                {
                    Id = f.Id,
                    ExistingFilepath = f.ExistingFilepath
                })
                .ToListAsync();

            return View(course);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Departments)
                .Where(c => c.CourseId == id)
                .Select(c => new CourseDeleteViewModel
                {
                    CourseId = c.CourseId,
                    Credits = c.Credits,
                    Title = c.Title,
                    DepartmentId = c.DepartmentId,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments != null ? c.Departments.Name : string.Empty
                    }
                })
                .FirstOrDefaultAsync();

            if (course == null) return NotFound();

            course.Images = await _context.FileToApi
                .Where(f => f.CourseId == id)
                .Select(f => new ImageViewModel
                {
                    Id = f.Id,
                    ExistingFilepath = f.ExistingFilepath
                })
                .ToListAsync();

            return View(course);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var files = await _context.FileToApi.Where(f => f.CourseId == id).ToListAsync();
            _context.FileToApi.RemoveRange(files);

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDepartmentDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = _context.Departments
                .OrderBy(d => d.Name)
                .GroupBy(d => d.Name)
                .Select(g => g.First());

            ViewBag.DepartmentId = new SelectList(departmentsQuery
                .AsNoTracking(), "DepartmentId", "Name", selectedDepartment);
        }
    }
}
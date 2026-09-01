using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ServiceInterface;
using University.ViewModel.CourseVM;

namespace University.Controllers
{
    public class CourseController : Controller
    {
        private readonly UniversityContext _context;
        private readonly IFileServices _fileServices;

        public CourseController
            (
                UniversityContext context,
                IFileServices fileServices
            )
        {
            _context = context;
            _fileServices = fileServices;
        }

        public async Task<IActionResult> Index()
        {
            var course = _context.Courses
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

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Files)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            var vm = new CourseUpdateViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId,
                Files = course.Files
                    .Select(f => new ImageViewModel
                    {
                        ImageId = f.Id,
                        FilePath = f.ExistingFilePath
                    })
                    .ToList()
            };

            PopulateDepartmentDropDownList(course.DepartmentId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CourseUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return View(vm);
            }
            PopulateDepartmentDropDownList(vm.DepartmentId);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == vm.CourseId);

            if (course == null)
            {
                return NotFound();
            }

            course.Title = vm.Title;
            course.Credits = vm.Credits;
            course.DepartmentId = vm.DepartmentId;

            await _context.SaveChangesAsync();

            if (vm.FileToApis != null && vm.FileToApis.Count > 0)
            {
                await _fileServices.AddFilesToCourse(vm.FileToApis, course.CourseId);
            }

            return RedirectToAction(nameof(Details), new { id = course.CourseId });
        }

        public IActionResult Create()
        {
            PopulateDepartmentDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PopulateDepartmentDropDownList(vm.DepartmentId);

                var course = new Course
                {
                    CourseId = vm.CourseId,
                    Title = vm.Title,
                    Credits = vm.Credits,
                    DepartmentId = vm.DepartmentId
                };

                // save course first to get CourseId
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                // save uploaded files
                await _fileServices.AddFilesToCourse(vm.Files, course.CourseId);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Departments)
                .Include(c => c.Files)
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
                    },
                    Files = c.Files
                        .Select(f => new ImageViewModel
                        {
                            ImageId = f.Id,
                            FilePath = f.ExistingFilePath
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

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
                        DepartmentName = c.Departments.Name
                    },
                    Files = c.Files
                        .Select(f => new ImageViewModel
                        {
                            ImageId = f.Id,
                            FilePath = f.ExistingFilePath
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Files)
                .FirstOrDefaultAsync(c => c.CourseId == id);


            if (course == null)
            {
                return NotFound();
            }

            // delete physical files + database records
            await _fileServices.RemoveImagesFromApi(
                course.Files.ToList());

            // delete course
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> RemoveFile(Guid imageId)
        {
            var file = await _context.FileToApis
                .FirstOrDefaultAsync(x => x.Id == imageId);

            if (file == null)
            {
                return NotFound();
            }

            await _fileServices.RemoveFileFromApi(file);

            return RedirectToAction(nameof(Update), new { id = file.CourseId });
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

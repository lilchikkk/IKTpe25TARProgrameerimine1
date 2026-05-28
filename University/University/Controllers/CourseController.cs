using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using University.Data;
using University.Models;
using University.ViewModel;
using University.ViewModel.CoursesVM;

namespace University.Controllers
{
    public class CourseController : Controller
    {
        //on vaja kutsuda välja  University constructor 

        private readonly UniversityContext _context;

        public CourseController

            (
                 UniversityContext context
            )
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var course = _context.Courses
                .Include(c => c.Departments)
                .Select(c => new CourseIndexViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Credits = c.Credits,
                    DepartmentId = c.DepartmentId,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments.Name
                    }
                });

            return View(course);

        }


        //UPDATE:
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.Courses
                .Where(c => c.CourseId == id)
                .Select(c => new CourseUpdateViewModel
                {
                    CourseId = c.CourseId,
                    Credits = c.Credits,
                    Title = c.Title,
                    Department = new CourseDepartmentIndexViewModel
                    {
                        DepartmentName = c.Departments != null ? c.Departments.Name : null
                    }
                })
                .FirstOrDefaultAsync();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseId = vm.CourseId,
                    Credits = vm.Credits,
                    Title = vm.Title,
                    Departments = new Department
                    {
                        Name = vm.Department.DepartmentName
                    }
                };
                _context.Update(course);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
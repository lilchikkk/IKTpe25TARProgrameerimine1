using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;

namespace University.Controllers
{
    public class CoursesController : Controller
    {
        //on vaja kututada välja Univercity constructor 
        private readonly UniversityContext _context;
        public CoursesController
         (
             UniversityContext context
         )

        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        { 
            var result = await _context.Courses
            .Include(c => c.Departments)
            .AsNoTracking()
            .ToListAsync();

        return View(result);

        }
    }
}

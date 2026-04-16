using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;

namespace University.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityContext _context;

        public StudentController
            (
                UniversityContext context
            )
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //leiame kõik studentis ja teisendame need StudentIndexModeliks 
            //miks peab kasutama await?
            //kui me kasutame await, siis me ootame kuni päring on lõpetatud 
            //ja saame tulemuse, enne kui me jätkame koodi täistmist
            var result = await _context.Students
                .Select(s => new ViewModel.StudentIndexViewModel
                {
                    Id = s.Id,
                    LastName = s.LastName,
                    FirstMidName = s.FirstMidName,
                    EnrollmentDate = s.EnrollmentDate
                }).ToListAsync();

            return View(result);
        }
        public async Task <IActionResult> Details(int? id)
        {
            //kui id on null, siis tagastame not found 
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            //miks me kasutame ToListAsync?
            //kui me kasutame see, siis me saame tulemuse listina

            //kui student on null, siis tagastame not found 
            if (student == null)
            {
                return NotFound();
            }

            //kui student on leitud, siis tagastame tulemus 
            return View(student);
        }
    }
}

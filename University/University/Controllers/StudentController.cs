using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Models;
using University.ViewModel;

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

            var vm = new ViewModel.StudentDetailsViewModel
            {
                Id = student.Id,
                LastName = student.FirstMidName,
                EnrollmentDate = student.EnrollmentDate
            };

            //miks me kasutame ToListAsync?
            //kui me kasutame see, siis me saame tulemuse listina

            //kui student on null, siis tagastame not found 
            if (student == null)
            {
                return NotFound();
            }

            //kui student on leitud, siis tagastame tulemus 
            return View(vm);
        }

        //GET : student/create
        //see meetod tagastab vaate, kus saab luua uue studenti
        public IActionResult  Create()
        {
            return View();
        }
        //Post : STudent/create
        //seemetod salvestab uue studenti andmebaasi
        [HttpPost]
        //see meetod on kaitstud CSRF rünnakutte eest
        //see meetod on askroonene, mis tähendab, et see metod ei saa
        //olaa samaaegselt mitu kordakäivitatud
        public async Task<IActionResult> Create(StudentCreateViewModel vm)
        {
            //kui model on valiidnesiis loome uue studenti ja salvetame selle andmebaasi
            if (ModelState.IsValid)
            {
                var student = new Models.Student
                {
                    LastName = vm.LastName,
                    FirstMidName = vm.FirstMidName,
                    EnrollmentDate = vm.EnrollmentDate
                };
                //liisame studentis andmebaasi ja salvestame muudatused
                _context.Add(student);
                //miks kasutame await?
                //kui me kasutame await, siiis me ootame kuni salvestamine on lõpetatud
                await _context.SaveChangesAsync();
                //pärast salvestamist suuname kasutaja tagasiIndex vaatese
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }
    }
}

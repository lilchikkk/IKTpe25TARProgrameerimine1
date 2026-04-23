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

            //leiame kõik student'id ja teisendame need StudentIndexViewModel'iks
            //miks peab kasutama await?
            //kui me kasutame await, siis me ootame kuni päring on lõpetatud
            //ja saame tulemuse, enne kui me jätkame koodi kirjutamist
            var result = await _context.Students
                .Select(s => new ViewModel.StudentIndexViewModel
                {
                    Id = s.Id,
                    LastName = s.LastName,
                    FirstMidName = s.FirstMidName,
                    EnrollmentDate = s.EnrollmentDate
                    //miks kasutame ToListAsync()?
                    //kui me kasutame ToListAsync(), siis saame tulemuse listina
                }).ToListAsync();

            return View(result);
        }
        public async Task<IActionResult> Details(int? id)
        {
            //kui id on null, siis tagastame NotFound() tulemuse
            if (id == null)
            {
                return NotFound();
            }

            //leiame student'i id järgi
            var student = await _context.Students
                //Include lubab objekti kasutada objekti sees
                .Include(s => s.Enrollments)
                //Kui tahad uuesti objeti kasutada objekti sees, siis kasutad ThenInclude
                .ThenInclude(e => e.Course)
                //andmeid ei salvestata vähemällu ja ei jälgita
                .AsNoTracking()
                // Leiab esimese elemendi andmetes, mis on tingimuse välja toodud 
                .FirstOrDefaultAsync(m => m.Id == id);

            var vm = new ViewModel.StudentDetailsViewModel
            {
                Id = student.Id,
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                EnrollmentDate = student.EnrollmentDate,
                //Miks kasutame ?? - vaikiva väärtuse annab e default väärtus, kui muutuja on tühi (null)
                //või mitte defineeritud. Annab enne vasakpoolse väärtuse, kui see ei ole null. Kui on null, 
                //siis annab parempoolse väärtuse.
                EnrollmentsVm = (student.Enrollments ?? Enumerable.Empty<Enrollment>())
                .Select(x => new EnrollmentViewModel
                {
                    CourseId = x.CourseId,
                    Grade = x.Grade,
                    CourseVm = new CourseViewModel
                    {
                        CourseId = x.Course?.CourseId ?? 0,
                        Title = x.Course?.Title,
                        Credits = x.Course?.Credits ?? 0
                    }
                    //üks õpilane võib mitu kursust olla läbinud  ja 
                    //selle tulemusel tuleb lõppu panna ToArray
                }).ToArray()

            };

            //kui student on null, siis tagastame NotFound() tulemuse
            if (student == null)
            {
                return NotFound();
            }

            //kui student on leitud, siis tagastame View(student) tulemuse
            return View(vm);
        }
        //GET:Student/Create
        //see meetod salvestab uue student'i andmebaasi
        public IActionResult Create()
        {
            return View();
        }
        //POST: Student/Create
        //see meetod salvestab uue student'i andmebaasi
        [HttpPost]
        //see meetod on kaitstud CSRF rünnakute eest
        //see meetod on asünkoorne, mis tähendab, et see meetod ei saa
        //olla samaaegselt mitu korda käivitatud
        public async Task<IActionResult> Create(StudentCreateViewModel vm)
        {
            //kui model on valiidne, siis loome uue student'i ja salvestame selle andmebaasi
            if (ModelState.IsValid)
            {
                var student = new Models.Student
                {
                    Id = vm.Id,
                    LastName = vm.LastName,
                    FirstMidName = vm.FirstMidName,
                    EnrollmentDate = vm.EnrollmentDate
                };
                //lisame student'i andmebaasi ja salvestame muudatuse
                _context.Add(student);
                //miks kasutame await?
                //kui me kasutame await, siis me ootame kuni salvestamine on lõpetatud
                await _context.SaveChangesAsync();
                //pärast salvestamist suuname kasutaja tagasi Index vaatesse
                return RedirectToAction(nameof(Index));

            }
            return View(vm);
        }

        //Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var student = await _context.Students
               .FirstOrDefaultAsync(m => m.Id == id);


            //kui student on null, siis on not found
            if (student == null)
            {
                return NotFound(); 
            }

            //tuleb teha domaini andmete ülekanne view modeli omasse
            var viewModel = new StudentUpdateViewModel
            {
                Id = student.Id,
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                EnrollmentDate = student.EnrollmentDate
            };

            return View(viewModel);
        }
    }
}

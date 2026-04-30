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

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; 
            ViewData["DateSortParm"] = sortOrder == "Date" ? "name_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var students = await _context.Students
                .Select(s => new StudentIndexViewModel
                {
                    Id = s.Id,
                    LastName = s.LastName,
                    FirstMidName = s.FirstMidName,
                    EnrollmentDate = s.EnrollmentDate
                    //miks kasutame ToListAsync()?
                    //kui me kasutame ToListAsync(), siis saame tulemuse listina
                }).ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                    || s.FirstMidName.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName).ToList();
                    break;

                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate).ToList();
                    break;

                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate).ToList();
                    break;

                default:
                    students = students.OrderBy(s => s.LastName).ToList();
                    break;
            }

            return View(students);
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

        [HttpPost]
        public async Task<IActionResult> Update(StudentUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var student = new Models.Student
                {
                    Id = vm.Id,
                    LastName = vm.LastName,
                    FirstMidName = vm.FirstMidName,
                    EnrollmentDate = vm.EnrollmentDate
                };

                var studentUpdate = student.Id;
                //lisame student'i andmebaasi ja salvestame muudatuse
                _context.Update(student);
                //miks kasutame await?
                //kui me kasutame await, siis me ootame kuni salvestamine on lõpetatud
                await _context.SaveChangesAsync();

                // kui andmed uuendatud , iis suunab tagasi Update vaatesse, kus saab kohe uuesti andmeid uendada.
                //Hetkel suunab <indexi vaatesse peale uuendust
                return RedirectToAction(nameof(Update),  new {id = studentUpdate});
            }

            return RedirectToAction(nameof(Update));
        }

        //tehke Delete Get meetod koos vaatega
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            var vm = new StudentDeleteViewModel
            {
                Id = student.Id,
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                EnrollmentDate = student.EnrollmentDate,
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
                    }).ToArray()
            };

            if (student == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        //tuleb teha ankeedi kustutamise nupp
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                Student delete = new Student()
                {
                    Id = id,
                };

                _context.Students.Remove(delete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new {id = id, saveChangesError = true });
                throw;
            }

            return RedirectToAction(nameof(Delete));

        }

    }
}

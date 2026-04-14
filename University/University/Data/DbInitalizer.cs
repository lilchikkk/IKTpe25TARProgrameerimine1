using University.Models;

namespace University.Data
{
    public static class DbInitalizer
    {
        public static void Initialixen(UniversityContext context)
        {
            //Ensure the database is created
            context.Database.EnsureCreated();
            // Look for any students alredy in the database 
            if (context.Students.Any())
            {
                return; //DB has been seeded
            }
            var students = new Student[]
            {
                new Student{FirstMidName="Carson", LastName="Alexander", EnrollmentDate=DateTime.Parse("2010-09-01")},
                new Student{FirstMidName="meredith", LastName="Alonso", EnrollmentDate=DateTime.Parse("2012-09-01")},


            };
        }
    }
}
 

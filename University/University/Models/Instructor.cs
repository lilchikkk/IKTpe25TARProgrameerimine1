using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        [Column("First Name")]
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        //mis on icollection????
        // ICollection on nimekiri - One-to-Many seos
        // üks õpetaja saab õpetada MITMEID kursusi
        public ICollection<CourseAssigment> CourseAssigments { get; set; }

        //miks ei saa kasutada icollection, vaid lihtsalt officeAssigment
        // One-to-One seos - õpetajal saab olla ainult ÜKS kontor
        // seepärast ei kasuta ICollection-it, vaid lihtsalt objekti
        public OfficeAssigment OfficeAssigments { get; set; }
    }
}

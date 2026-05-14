using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.ViewModel
{
    public class StudentIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        //esimene variant, mis ühendab ees- ja perekonnanime
        [Display(Name = "Full Name")]
        public string FullName => $"{LastName}, {FirstMidName}";

        //teine variant, mis ühendab ees- ja perekonnanime
        //[Display(Name = "Full Name")]
        //public string Fullname
        //{
        //    get
        //    {
        //        return $"{LastName}, {FirstMidName}";
        //    }
        //}
    }
}
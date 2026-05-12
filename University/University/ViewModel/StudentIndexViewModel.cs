using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.ViewModel
{
    public class StudentIndexViewModel
    {
        public int Id { get; set; }

        [StringLenght(50, MinimumLenght = 1)]
        public string LastName { get; set; }
        [StringLenght(50, MinimumLenght = 1)]
        [Column("FirstName")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

    }
}

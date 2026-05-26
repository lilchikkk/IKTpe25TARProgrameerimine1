using System.ComponentModel.DataAnnotations;
namespace University.Models
{
    public class OfficeAssignment
    {
        //kui soovite konkreetselt välja tuua, et Instructor on nii OfficeAssignment
        //peamine võti kui ka võõõrvõti, siis saate kasutada  [Key] ja [ForeingKey] atribute
        [Key]
        public int InstructorId { get; set; }
        public string Location { get; set; } = string.Empty;

        public Instructor Instructor { get; set; }



    }
}

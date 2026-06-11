using System.ComponentModel.DataAnnotations.Schema;
using University.ViewModel.CoursesVM;

namespace University.Models
{
    public class FileToApiDto
    {
        public Guid Id { get; set; }
        public string? ExistingFilepath { get; set; }
        public Guid? CourseId { get; set; }
    }
}

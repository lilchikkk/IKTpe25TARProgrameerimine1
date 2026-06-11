using System.ComponentModel.DataAnnotations.Schema;
using University.ViewModel.CoursesVM;

namespace University.Dto
{
    public class CourseDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentId { get; set; }

        [NotMapped]
        public List<IFormFile> Files { get; set; }

        [NotMapped]
        public IEnumerable<ImageViewModel> FilesToApi { get; set; }
            = new List<ImageViewModel>();
    }
}
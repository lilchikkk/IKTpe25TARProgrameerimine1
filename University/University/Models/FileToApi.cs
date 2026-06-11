namespace University.Models
{
    public class FileToApi
    {
        public Guid Id { get; set; }
        public string? ExistingFilepath { get; set;  }
        public int CourseId { get; set; }
    }
}

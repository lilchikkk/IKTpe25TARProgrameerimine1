using University.Models;


namespace University.ServiceInterface
{
    public interface IFileServices
    {
        Task AddFilesToCourse(List<IFormFile> files, int courseId);
        Task RemoveImagesFromApi(List<FileToApi> files);
        Task RemoveFileFromApi(FileToApi file);
    }
}

using University.Data;
using University.Models;
using University.ServiceInterface;



namespace University.Services
{
    public class FileServices : IFileServices
    {
        private readonly IHostEnvironment _webHost;
        private readonly UniversityContext _context;

        public FileServices
            (
                IHostEnvironment webHost,
                UniversityContext context
            )
        {
            _webHost = webHost;
            _context = context;
        }

        public async Task AddFilesToCourse(List<IFormFile> files, int courseId)
        {
            if (files == null || files.Count == 0)
                return;

            var folder = Path.Combine(
                _webHost.ContentRootPath,
                "wwwroot",
                "multipleFileUpload");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (var file in files)
            {
                string uniqueFileName =
                    Guid.NewGuid() + "-" + file.FileName;

                string filePath = Path.Combine(folder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var entity = new FileToApi
                {
                    Id = Guid.NewGuid(),
                    ExistingFilePath = uniqueFileName,
                    CourseId = courseId
                };

                _context.FileToApis.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveImagesFromApi(List<FileToApi> files)
        {
            if (files == null || files.Count == 0)
                return;

            foreach (var file in files)
            {
                var filePath = Path.Combine(
                    _webHost.ContentRootPath,
                    "wwwroot",
                    "multipleFileUpload",
                    file.ExistingFilePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _context.FileToApis.RemoveRange(files);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFileFromApi(FileToApi file)
        {
            if (file == null)
                return;

            var filePath = Path.Combine(
                _webHost.ContentRootPath,
                "wwwroot",
                "multipleFileUpload",
                file.ExistingFilePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.FileToApis.Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}

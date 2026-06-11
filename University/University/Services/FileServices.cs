using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dto;
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

        public void FilesToApi(CourseDto dto, Course domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                if (!Directory.Exists(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\"))
                {
                    Directory.CreateDirectory(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\");
                }

                foreach (var file in dto.Files)
                {
                    string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
                    string uniqueFileName = Guid.NewGuid().ToString() + " - " + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                        FileToApi path = new FileToApi
                        {
                            Id = Guid.NewGuid(),
                            ExistingFilepath = uniqueFileName,
                            CourseId = domain.CourseId
                        };

                        _context.FileToApi.Add(path);
                    }
                }

                _context.SaveChanges();
            }
        }

        public async Task<FileToApi?> RemoveImageFromApi(FileToApiDto dto)
        {
            var file = await _context.FileToApi.FindAsync(dto.Id);

            if (file != null)
            {
                string filePath = Path.Combine(
                    _webHost.ContentRootPath, "wwwroot", "multipleFileUpload",
                    file.ExistingFilepath ?? "");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.FileToApi.Remove(file);
                await _context.SaveChangesAsync();
            }

            return file;
        }
    }
}
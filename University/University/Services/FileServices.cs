using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Dto;
using University.Models;

namespace University.Services
{
    public class FileServices
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
            //tingimus, kui File ei ole null või on vähemalt rohkem, kui 0 failist, siis hakkab midagi tegema 
            if (dto.Files != null && dto.Files.Count > 0)
            {
                if(!Directory.Exists(_webHost.ContentRootPath + "\\wwroot\\multipleFileUpload\\"))
                {
                    Directory.CreateDirectory(_webHost.ContentRootPath + "\\wwroot\\multipleFileUpload\\)");
                }

                foreach (var file in dto.Files)
                {
                    string uploadsFolder = Path.Combine(_webHost.ContentRootPath + "wwroot", "multipleFileUpload");
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
            }
        }
    }
}

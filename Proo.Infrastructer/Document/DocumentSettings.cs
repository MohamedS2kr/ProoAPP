using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Document
{
    public static class DocumentSettings
    {
        public static string UploadFile(IFormFile file , string folderName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files" , folderName );

            if (!Directory.Exists(folderPath)) 
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{file.FileName}"; // UNIQ File name 

            var filePath = Path.Combine(folderPath, fileName);

            // save file as streams 
            using var fileStream = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fileStream);

            return fileName;
        }

        public static void DeleteFile(string fileName ,  string folderName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName , fileName);

            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}

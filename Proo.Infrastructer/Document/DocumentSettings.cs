using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace Proo.Infrastructer.Document
{
    public static class DocumentSettings
    {
        public static string? UploadFile(IFormFile file , string folderName)
        {
            if (file is not null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{file.FileName}"; // UNIQ File name 

                var filePath = Path.Combine(folderPath, fileName);

                // save file as streams 
                using var fileStream = new FileStream(filePath, FileMode.Create);

                file.CopyTo(fileStream);

                return fileName; 
            }
            return null;
        }

        public static void DeleteFile(string fileName ,  string folderName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName , fileName);

            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }


        // convert color to hexa 
        public static string GetColorName(Color color)
        {
            if (color.IsKnownColor)
                return color.Name;
            return $"Unknown Color ({color.R}, {color.G}, {color.B})"; // Returns RGB value if color name isn't known
        }
    }
}

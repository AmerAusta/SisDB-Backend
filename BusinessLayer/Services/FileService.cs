using Microsoft.AspNetCore.Http;
using System.Linq; 
using System.IO;
namespace BusinessLayer.Services
{
    public class FileService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public async Task<string?> UploadFileAsync(IFormFile file, string folderName, string[] allowedExtensions)
        {
            if (file == null || file.Length == 0) return null;

            // 1. التحقق من امتداد الملف
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return null; // امتداد غير مسموح به
            }

            // 2. إنشاء المسار وتأكيده
            var targetFolder = Path.Combine(_basePath, folderName);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // 3. إنشاء اسم فريد للملف باستخدام Guid
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(targetFolder, uniqueFileName);

            // 4. حفظ الملف على السيرفر
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. إرجاع المسار النسبي المكتوب في قاعدة البيانات
            return $"/{folderName}/{uniqueFileName}";
        }

        public bool DeleteFile(string fileRelativeUrl)
        {
            if (string.IsNullOrWhiteSpace(fileRelativeUrl)) return false;

            var fullPath = Path.Combine(_basePath, fileRelativeUrl.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }
    }
}
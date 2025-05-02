namespace Jammer.Utills
{
    public class FileManage
    {
        public static async Task<string> UploadAsync(IFormFile file, IWebHostEnvironment environment)
        {
            var rootFolder = environment.WebRootPath;
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
            string uploadFolder = Path.Combine(rootFolder, "uploads");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadFolder, fileName + extension);
            using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return "/uploads/" + fileName + extension;
        }
    }
}
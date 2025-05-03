namespace JuanApp.Areas.Manage.Helpers
{
    public static class FileManager
    {
        public static string Save(this IFormFile file, string folder)
        {

            string newfileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), folder, newfileName);
            using var fileStream = new FileStream(path, FileMode.Create);
            file.CopyTo(fileStream);
            return newfileName;
        }
        public static bool CheckType(this IFormFile file)
        {
            return file.ContentType.Contains("image/");
        }
        public static bool CheckSize(this IFormFile file, int size)
        {
            return file.Length > size;
        }
    }
}

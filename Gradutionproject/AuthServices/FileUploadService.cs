namespace Gradutionproject.AuthServices
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string UploadFile(IFormFile file)
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return file.FileName;  // Return the file name to store in DB
        }
    }
}

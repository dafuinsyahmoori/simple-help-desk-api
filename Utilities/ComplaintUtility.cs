namespace SimpleHelpDeskAPI.Utilities
{
    public class ComplaintUtility
    {
        private readonly string[] _allowedFileExtensions = { ".png", ".jpg", ".jpeg", ".mp4", ".mkv" };

        public async Task<string?> SaveComplaintImageOrVideoAsync(IFormFile imageOrVideo, string userId)
        {
            if (imageOrVideo.Length > 0)
            {
                var fileExtension = Path.GetExtension(imageOrVideo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileExtension) || !_allowedFileExtensions.Contains(fileExtension))
                    return null;

                var randomFileName = Path.GetRandomFileName().Split('.')[0];
                var finalFileName = randomFileName + fileExtension;

                if (!Directory.Exists($"Uploads\\{userId}"))
                    Directory.CreateDirectory($"Uploads\\{userId}");

                using var fileStream = File.Create($"Uploads\\{userId}\\{finalFileName}");

                await imageOrVideo.CopyToAsync(fileStream);

                return finalFileName;
            }

            return null;
        }
    }
}
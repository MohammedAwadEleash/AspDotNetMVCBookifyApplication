using Microsoft.AspNetCore.Http;


namespace Bookify.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly IAppEnvironmentService _appEnvironmentService;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;

        public ImageService(IAppEnvironmentService appEnvironmentService)
        {
            _appEnvironmentService = appEnvironmentService;
        }


        public async Task<(bool isUploaded, string? errorMessage)> UploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail)
        {

            var extension = Path.GetExtension(image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                return (isUploaded: false, errorMessage: Errors.NotAllowedExtension);
            }

            if (image.Length > _maxAllowedSize)
            {
                return (isUploaded: false, errorMessage: Errors.MaxSize);

            }


            var path = Path.Combine($"{_appEnvironmentService.GetRoothPath}{folderPath}", imageName);



            using var stream = File.Create(path);
            await image.CopyToAsync(stream);
            stream.Dispose();

            if (hasThumbnail)
            {

                var thumbPath = Path.Combine($"{_appEnvironmentService.GetRoothPath}{folderPath}/thumb", imageName);

                using var loadedImage = Image.Load(image.OpenReadStream());
                var ratio = (float)loadedImage.Width / 200;
                var height = loadedImage.Height / ratio;
                loadedImage.Mutate(i => i.Resize(width: 200, height: (int)height));
                loadedImage.Save(thumbPath);
            }



            return (isUploaded: true, errorMessage: null);


        }
        public void DeleteImage(string imagePath, string? imageThumbnailPath = null)
        {
            var oldImagePath = $"{_appEnvironmentService.GetRoothPath}{imagePath}";

            // Check Whether the Image Exist in Server (App) or not
            if (File.Exists(oldImagePath))
                File.Delete(oldImagePath);
            if (!string.IsNullOrEmpty(imageThumbnailPath))
            {

                var oldThumbPath = $"{_appEnvironmentService.GetRoothPath}{imageThumbnailPath}";
                if (File.Exists(oldThumbPath))
                    File.Delete(oldThumbPath);

            }


        }


    }
}

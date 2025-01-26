using Microsoft.AspNetCore.Http;

namespace Bookify.Application.Comman.Interfaces
{
    public interface IImageService
    {
        Task<(bool isUploaded, string? errorMessage)> UploadAsync(IFormFile image, string imageName, string pathFolder, bool hashThumbnail);

        void DeleteImage(string imagePath, string? imageThumbnailPath = null);


    }




}

using Microsoft.AspNetCore.Http;

namespace DataAccess.Dtos.ImageUploadDto
{
    public class ImageUploadDTO
    {
        public IFormFile Image { get; set; }

    }
}

using DataAccess.Dtos.ImageUploadDto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ImageRepository
{
    public interface IImageRepository
    {
        Task<string> UploadImageAndReturnUrlAsync(IFormFile image);

    }
}

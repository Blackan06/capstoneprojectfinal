using BusinessObjects.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ImageSetting;
using DataAccess.ImageResponse;
using DataAccess.Dtos.ImageUploadDto;

namespace DataAccess.Repositories.ImageRepository
{
    public class ImageRepository : IImageRepository
    {
        private readonly ImgurSettings _imgurSettings;
        private readonly db_a9c31b_capstoneContext _dbContext;

        public ImageRepository(IOptions<ImgurSettings> imgurSettings, db_a9c31b_capstoneContext dbContext)
        {
            _imgurSettings = imgurSettings.Value;
            _dbContext = dbContext;
        }
        public async Task<string> UploadImageAndReturnUrlAsync(IFormFile image)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Client-ID", _imgurSettings.ClientId);

                using (var content = new MultipartFormDataContent())
                {
                    var imageContent = new StreamContent(image.OpenReadStream());
                    content.Add(imageContent, "image", image.FileName);

                    var response = await httpClient.PostAsync("https://api.imgur.com/3/image", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ImgurUploadResponse>(responseContent);

                        string imageUrl = responseData.Data.Link;

                        return imageUrl; // Trả về đường dẫn URL của hình ảnh sau khi tải lên
                    }
                    else
                    {
                        throw new Exception($"Image upload failed with status code: {response.StatusCode}");
                    }
                }
            }
        }
    }
}

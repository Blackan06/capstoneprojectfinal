using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ImageResponse
{
    public class ImgurUploadResponse
    {
        public ImgurImageData Data { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
    }
}

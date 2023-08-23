using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.GoogleAuthSetting
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] RedirectUris { get; set; }
        // Các thuộc tính khác cần thiết

        public string GetValidRedirectUri(string requestedRedirectUri)
        {
            // Kiểm tra xem requestedRedirectUri có nằm trong danh sách RedirectUris hay không
            if (RedirectUris.Contains(requestedRedirectUri))
            {
                return requestedRedirectUri;
            }

            // Mặc định trả về RedirectUri đầu tiên trong danh sách RedirectUris
            return RedirectUris.FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.Users
{
    public class LoginResponseDto
    {
        public Guid PlayerId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string Nickname { get; set; }
        public string Passcode { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TotalPoint { get; set; }
        public double TotalTime { get; set; }
        public bool Isplayer { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string SchoolStatus { get; set; }

    }
}

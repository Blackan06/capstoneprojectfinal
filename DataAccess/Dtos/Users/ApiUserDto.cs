using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.Users
{
    public class ApiUserDto : LoginDto
    {
        public Guid SchoolId { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Fullname { get; set; }
        [EmailAddress]
        public string Email { get; set; }

    }
}

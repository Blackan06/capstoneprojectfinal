using BusinessObjects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.Users
{
    public class UserWithToken : Student
    {
        
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ValidateCode { get;set; }

    public UserWithToken(Student user)
    {
        this.Id = user.Id;
        this.Email = user.Email;
        this.Phonenumber = user.Phonenumber;
        this.Fullname = user.Fullname;
        this.GraduateYear = user.GraduateYear;
    }
}
}

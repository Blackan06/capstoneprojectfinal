using BusinessObjects.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.StudentDto
{
    public class BaseStudentDto
    {
        private Guid schoolId;
        private string fullname;
        private string email;
        private string phonenumber;
        private int graduateYear;
        private string classname;
        private string status;
        private DateTime createdAt;

        [Required]
        public Guid SchoolId
        {
            get { return schoolId; }
            set { schoolId = value; }
        }

        [Required]
        public string Fullname
        {
            get { return fullname; }
            set { fullname = value; }
        }

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Required]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "PhoneNumber must be numeric.")]
        public string Phonenumber
        {
            get { return phonenumber; }
            set { phonenumber = value; }
        }

        [Required]
        public int GraduateYear
        {
            get { return graduateYear; }
            set { graduateYear = value; }
        }

        [Required]
        public string Classname
        {
            get { return classname; }
            set { classname = value; }
        } 
        

        [Required]
        [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        [JsonIgnore]

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

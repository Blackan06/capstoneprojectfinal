using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolDto
{
    public class UpdateSchoolDto : BaseSchoolDto
    {
        private string name;
        private string phoneNumber;
        private string email;
        private string address;
        private string status;

        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "PhoneNumber must be numeric.")]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        [Required]
        [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
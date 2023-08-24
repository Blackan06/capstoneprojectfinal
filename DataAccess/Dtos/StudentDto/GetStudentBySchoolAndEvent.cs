using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.StudentDto
{
    public class GetStudentBySchoolAndEvent
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public string Schoolname { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public long Phonenumber { get; set; }
        public int GraduateYear { get; set; }
        public string Classname { get; set; }
        public string Status { get; set; }
        public string Passcode { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

    }
}

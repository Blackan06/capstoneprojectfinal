using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Student
    {
        public Student()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public long Phonenumber { get; set; }
        public string Classname { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int GraduateYear { get; set; }

        public virtual School School { get; set; }
        public virtual Player Player { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}

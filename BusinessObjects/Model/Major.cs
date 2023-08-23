using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Major
    {
        public Major()
        {
            Questions = new HashSet<Question>();
            Tasks = new HashSet<Task>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}

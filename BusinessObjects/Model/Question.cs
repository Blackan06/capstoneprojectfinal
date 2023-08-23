using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public Guid Id { get; set; }
        public Guid MajorId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Major Major { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}

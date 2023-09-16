using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class SchoolEvent
    {
        public SchoolEvent()
        {
            Prizes = new HashSet<Prize>();
        }

        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid SchoolId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string ApprovalStatus { get; set; }

        public virtual Event Event { get; set; }
        public virtual School School { get; set; }
        public virtual ICollection<Prize> Prizes { get; set; }
    }
}

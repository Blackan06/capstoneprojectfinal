using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class EventTask
    {
        public EventTask()
        {
            PlayerHistories = new HashSet<PlayerHistory>();
        }

        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid EventId { get; set; }
        public double Point { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }

        public virtual Event Event { get; set; }
        public virtual Task Task { get; set; }
        public virtual ICollection<PlayerHistory> PlayerHistories { get; set; }
    }
}

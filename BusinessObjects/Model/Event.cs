using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Event
    {
        public Event()
        {
            EventTasks = new HashSet<EventTask>();
            Players = new HashSet<Player>();
            Prizes = new HashSet<Prize>();
            SchoolEvents = new HashSet<SchoolEvent>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<EventTask> EventTasks { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Prize> Prizes { get; set; }
        public virtual ICollection<SchoolEvent> SchoolEvents { get; set; }
    }
}

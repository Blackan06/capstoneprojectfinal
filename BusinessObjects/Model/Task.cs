using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Task
    {
        public Task()
        {
            EventTasks = new HashSet<EventTask>();
        }

        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public Guid MajorId { get; set; }
        public Guid NpcId { get; set; }
        public Guid? ItemId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Item Item { get; set; }
        public virtual Location Location { get; set; }
        public virtual Major Major { get; set; }
        public virtual Npc Npc { get; set; }
        public virtual ICollection<EventTask> EventTasks { get; set; }
    }
}

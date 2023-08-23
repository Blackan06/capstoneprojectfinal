using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Location
    {
        public Location()
        {
            Tasks = new HashSet<Task>();
        }

        public Guid Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string LocationName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}

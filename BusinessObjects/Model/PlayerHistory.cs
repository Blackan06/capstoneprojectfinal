using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class PlayerHistory
    {
        public Guid Id { get; set; }
        public Guid EventtaskId { get; set; }
        public Guid PlayerId { get; set; }
        public double? CompletedTime { get; set; }
        public double? TaskPoint { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual EventTask Eventtask { get; set; }
        public virtual Player Player { get; set; }
    }
}

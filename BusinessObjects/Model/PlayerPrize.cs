using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class PlayerPrize
    {
        public Guid Id { get; set; }
        public Guid PrizeId { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        public virtual Player Player { get; set; }
        public virtual Prize Prize { get; set; }
    }
}

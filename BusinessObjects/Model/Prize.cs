using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Prize
    {
        public Prize()
        {
            PlayerPrizes = new HashSet<PlayerPrize>();
        }

        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Event Event { get; set; }
        public virtual ICollection<PlayerPrize> PlayerPrizes { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class Inventory
    {
        public Inventory()
        {
            ItemInventories = new HashSet<ItemInventory>();
        }

        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Player Player { get; set; }
        public virtual ICollection<ItemInventory> ItemInventories { get; set; }
    }
}

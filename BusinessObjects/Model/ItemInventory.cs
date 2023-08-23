using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class ItemInventory
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual Item Item { get; set; }
    }
}

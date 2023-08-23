using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.ItemInventoryDto
{
    public abstract class BaseItemInventoryDto
    {
        private Guid inventoryId;
        private Guid itemId;
        private int quantity;
        private DateTime createdAt;

        [Required]
        public Guid InventoryId
        {
            get { return inventoryId; }
            set { inventoryId = value; }
        }

        public Guid ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        [JsonIgnore]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

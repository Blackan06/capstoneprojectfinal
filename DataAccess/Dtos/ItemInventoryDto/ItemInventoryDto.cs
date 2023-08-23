using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.ItemInventoryDto
{
    public class ItemInventoryDto : IBaseDto
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}

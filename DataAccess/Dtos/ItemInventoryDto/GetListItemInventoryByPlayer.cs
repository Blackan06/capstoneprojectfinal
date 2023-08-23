using DataAccess.Dtos.ItemDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.ItemInventoryDto
{
    public class GetListItemInventoryByPlayer 
    {
        public Guid PlayerId { get; set; }
        public Guid InventoryId { get; set; }
        public List<GetListItemDto> ListItem { get; set; }

        public List<GetItemInventoryDto> ListItemInventory { get; set; }
    }
}

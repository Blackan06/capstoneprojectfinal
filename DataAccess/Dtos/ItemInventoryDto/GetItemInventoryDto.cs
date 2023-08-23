using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.ItemInventoryDto
{
    public class GetItemInventoryDto : BaseItemInventoryDto, IBaseDto
    {
        public Guid Id { get; set; }
    }
}

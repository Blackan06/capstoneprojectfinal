using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.InventoryDto
{
    public class InventoryDto : IBaseDto
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }

    }
}

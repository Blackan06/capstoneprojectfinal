using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.ExchangeHistoryDto
{
    public class ExchangeHistoryDto :  IBaseDto
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public string ItemName { get; set; }
        public DateTime ExchangeDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Quantity { get; set; }

    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObjects.Model
{
    public partial class ExchangeHistory
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime ExchangeDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        public virtual Item Item { get; set; }
        public virtual Player Player { get; set; }
    }
}

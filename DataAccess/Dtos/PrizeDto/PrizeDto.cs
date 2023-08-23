using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PrizeDto
{
    public class PrizeDto 
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }
        public string Name { get; set; }
        public string Decription { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }
}

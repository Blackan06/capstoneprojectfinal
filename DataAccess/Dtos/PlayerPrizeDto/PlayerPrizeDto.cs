using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class PlayerPrizeDto 
    {
        public string PrizeName { get; set; }
        public string PlayerName { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }
}


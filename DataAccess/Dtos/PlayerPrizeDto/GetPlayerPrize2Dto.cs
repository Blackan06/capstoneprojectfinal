using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class GetPlayerPrize2Dto
    {
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string SchoolName { get; set; }
        public string PrizeName { get; set; }

        public string EventName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public int PrizeRank { get; set; }
        public DateTime DateReceived { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerHistoryDto
{
    public class PlayerHistoryDto 
    {
        public Guid EventtaskId { get; set; }
        public string PlayerNickName { get; set; }
        public double CompletedTime { get; set; }
        public double TaskPoint { get; set; }
        public string Status { get; set; }
    }
}

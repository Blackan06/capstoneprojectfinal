using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class CreatePlayerPrize2Dto
    {
        private Guid prizeId;
        private DateTime dateReceived;
        private DateTime createdAt;
        private string status;


        [Required]
        public Guid PrizeId
        {
            get { return prizeId; }
            set { prizeId = value; }
        }


       

        [JsonIgnore]

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
        [Required]

        public DateTime DateReceived
        {
            get { return dateReceived; }
            set { dateReceived = value; }
        }
        [RegularExpression("^(RECEIVED|PENDING|NOT RECEIVED)$", ErrorMessage = "Status must be 'RECEIVED' or 'PENDING' or 'NOT RECEIVED'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}

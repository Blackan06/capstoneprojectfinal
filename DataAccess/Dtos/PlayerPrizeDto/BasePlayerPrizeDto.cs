using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class BasePlayerPrizeDto
    {
        private Guid prizeId;
        private Guid playerId;
        private DateTime dateReceived;
        private DateTime createdAt;
        private string status;


        [Required]
        public Guid PrizeId
        {
            get { return prizeId; }
            set { prizeId = value; }
        }
      

        [Required]
        public Guid PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
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

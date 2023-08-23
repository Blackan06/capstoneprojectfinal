using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class BasePlayerPrizeDto
    {
        private Guid prizeId;
        private Guid playerId;
        private DateTime createdAt;

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
    }
}

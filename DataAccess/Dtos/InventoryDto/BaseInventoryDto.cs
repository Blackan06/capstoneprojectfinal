using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.InventoryDto
{
    public abstract class BaseInventoryDto
    {
        private Guid playerId;
        private DateTime createdAt;

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

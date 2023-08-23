using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PrizeDto
{
    public abstract class BasePrizeDto
    {
        private Guid eventId;
        private string name;
        private string description;
        private string status;
        private int quantity;
        private DateTime createdAt;

        [Required]
        public Guid EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Required]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        [Required]
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [JsonIgnore]

        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.PrizeDto
{
    public abstract class BasePrizeDto
    {
        private Guid schooleventId;
        private string name;
        private string description;
        private string status;
        private int quantity;
        private DateTime? createdAt;
        private int prizeRank;

        [Required]
        public int PrizeRank
        {
            get { return prizeRank; }
            set { prizeRank = value; }
        }
        [Required]
        public Guid SchooleventId
        {
            get { return schooleventId; }
            set { schooleventId = value; }
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

        public DateTime? CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

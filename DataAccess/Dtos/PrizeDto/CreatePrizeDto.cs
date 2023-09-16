using BusinessObjects.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PrizeDto
{
    public class CreatePrizeDto
    {
        private string name;
        private string description;
        private string status;
        private int quantity;
        private DateTime? createdAt;
        private int prizeRank;
        private Guid eventId;
        private Guid schoolId;
        [Required]
        public int PrizeRank
        {
            get { return prizeRank; }
            set { prizeRank = value; }
        }
        [Required]
        public Guid EventId
        {
            get { return eventId; }
            set { eventId = value; }
        } 
        [Required]
        public Guid SchoolId
        {
            get { return schoolId; }
            set { schoolId = value; }
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

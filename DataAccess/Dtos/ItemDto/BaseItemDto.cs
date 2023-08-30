using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Dtos.ItemDto
{
    public abstract class BaseItemDto
    {
        private string name;
        private double price;
        private string description;
        private string type;
        private bool? limitExchange;
        private string status;
        public string imageUrl;
        private DateTime createdAt;
        public IFormFile Image { get; set; } 

        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        [Range(1, 1000000000, ErrorMessage = "The Price field must be between 0 and 1 billion.")]
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        [Required]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Required]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool? LimitExchange
        {
            get { return limitExchange; }
            set { limitExchange = value; }
        }

        [Required]
        [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
        [JsonIgnore]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

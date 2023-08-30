using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.ItemDto
{
    public class CreateItemDto 
    {
        private string name;
        private double price;
        private string description;
        private string type;
        private bool? limitExchange;
        private string? status;
        public string imageUrl;
      
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
        public DateTime? CreatedAt { get; set; }
    }

}

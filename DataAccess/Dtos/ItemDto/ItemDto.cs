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
    public class ItemDto : IBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool? LimitExchange { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }


    }
}

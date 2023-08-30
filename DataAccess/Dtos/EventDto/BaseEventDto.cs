using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Dtos.EventDto
{
    public abstract class BaseEventDto
    {
        private string name;       
        private string status;
        private DateTime createdAt;

        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

     

        [Required]
        [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        [JsonIgnore]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

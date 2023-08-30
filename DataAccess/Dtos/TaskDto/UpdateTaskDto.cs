using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.TaskDto
{
    public class UpdateTaskDto 
    {
      
        private string locationName;
        private string majorName;
        private string npcName;
        private string? itemName;
        private string name;
        private string type;
        private string status;
      
      

        [Required]
        public string LocationName
        {
            get { return locationName; }
            set { locationName = value; }
        }

        [Required]
        public string MajorName
        {
            get { return majorName; }
            set { majorName = value; }
        }

        [Required]
        public string NpcName
        {
            get { return npcName; }
            set { npcName = value; }
        }

        public string? ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }
       [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        [Required]
        [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

      
    }
}

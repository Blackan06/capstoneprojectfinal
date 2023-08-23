using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.TaskDto
{
    public class TaskDto : IBaseDto
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; }
        public string MajorName { get; set; }
        public string NpcName { get; set; }
        public string ItemName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

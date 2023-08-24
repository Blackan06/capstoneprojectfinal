using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.EventTaskDto
{
    public class EventTaskDto :  IBaseDto
    {
        public Guid Id { get; set; }

        public string TaskName { get; set; }
        public string EventName { get; set; }
        public double Point { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

    }
}

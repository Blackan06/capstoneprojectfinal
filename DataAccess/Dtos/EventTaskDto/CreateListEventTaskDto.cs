using DataAccess.Dtos.TaskDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.EventTaskDto
{
    public class CreateListEventTaskDto
    {
        public Guid EventId { get; set; }
        public List<Guid> TaskId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Priority { get; set; }
        public double Point { get; set; }
        public string Status { get; set; }

        private DateTime createdAt;
        [JsonIgnore]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set { createdAt = value; }
        }
    }
}

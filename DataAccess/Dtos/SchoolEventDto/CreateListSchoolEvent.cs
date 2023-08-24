using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class CreateListSchoolEvent
    {
        public Guid EventId { get; set; }
        public List<Guid> SchoolIds { get; set; }
        public string Status { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class SchoolEventDto : IBaseDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid SchoolId { get; set; }
        public string EventName { get; set; }
        public string SchoolName { get; set; }
        public string Approvalstatus { get; set; }

        public string Status { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }
}

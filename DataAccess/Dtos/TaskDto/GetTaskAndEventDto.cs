using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.TaskDto
{
    public class GetTaskAndEventDto
    {
        public string EventName { get; set; }
        public IEnumerable<GetTaskRequestDto> TaskDtos { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

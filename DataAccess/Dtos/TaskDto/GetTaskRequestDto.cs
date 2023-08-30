using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.TaskDto
{
    public class GetTaskRequestDto  : IBaseDto
    {
        public Guid Id { get; set; }
        public Guid EventtaskId { get; set; }
        public string LocationName { get; set; }
        public string MajorName { get; set; }
        public Guid MajorId{ get; set; }
        public string NpcName { get; set; }
        public Guid? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string Name { get; set; }
        public double Point { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public TimeSpan Starttime { get; set; }
        public TimeSpan Endtime  { get; set; }
    }
}

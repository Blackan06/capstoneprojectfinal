using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.EventTaskDto
{
    public class UpdateEventTaskDto 
    {
        private Guid taskId;
        private string startTime;
        private string endTime;
        private double point;
        private string status;
        [Required]
        public Guid TaskId
        {
            get { return taskId; }
            set { taskId = value; }
        }    
        [Required]
        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        [Required]
        public string EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }
        [Required]
        public double Point
        {
            get { return point; }
            set { point = value; }
        }
        [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be 'ACTIVE' or 'INACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}

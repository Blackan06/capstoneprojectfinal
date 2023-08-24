using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class TimeSchoolEventDTO
    {
        private DateTime startTime;
        private DateTime endTime;
        [Required]
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        [Required]
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }
    }
}

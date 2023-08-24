using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class UpdateSchoolEventDto
    {
        private Guid schoolId;
        private string status;
        private string approvalStatus;
        private DateTime startTime;
        private DateTime endTime;

     
        [Required]
        public Guid SchoolId
        {
            get { return schoolId; }
            set { schoolId = value; }
        }
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
        [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be 'ACTIVE' or 'INACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        [RegularExpression("^(ACCEPT|REFUSE)$", ErrorMessage = "Status must be 'ACCEPT' or 'REFUSE'.")]
        public string ApprovalStatus
        {
            get { return approvalStatus; }
            set { approvalStatus = value; }
        }

        
    }
}

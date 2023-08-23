using DataAccess.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.QuestionDto
{
    public class QuestionAndAnswerDto 
    {
        private Guid majorId;
        private string name;
        private string status;
        public List<GetAnswerListDto> Answers { get; set; }

        [Required]
        public Guid MajorId
        {
            get { return majorId; }
            set { majorId = value; }
        }

        [Required]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be 'ACTIVE' or 'INACTIVE'.")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        

    }
}

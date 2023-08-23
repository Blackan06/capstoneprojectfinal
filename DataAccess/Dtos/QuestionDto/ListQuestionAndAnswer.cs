using DataAccess.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.QuestionDto
{
    public class ListQuestionAndAnswer
    {
        public Guid Id { get; set; }
        public Guid MajorId { get; set; }
        public string MajorName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<GetAnswerListDto> Answers { get; set; }

        
    }
}

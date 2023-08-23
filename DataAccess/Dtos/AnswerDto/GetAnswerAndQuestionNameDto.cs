using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.AnswerDto
{
    public class GetAnswerAndQuestionNameDto 
    {

        public string QuestionName { get; set; }

        public List<GetAnswerListDto> AnswerDtos { get; set; }
        public int CorrectAnswerIndex { get; set; }
    }
}

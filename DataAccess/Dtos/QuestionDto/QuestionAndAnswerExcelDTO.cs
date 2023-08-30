using DataAccess.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.Dtos.QuestionDto
{
    public class QuestionAndAnswerExcelDTO
    {
        public GetQuestionDto questionDto { get; set; }
        public List<GetAnswerDto> Answers { get; set; }

       
    }
}

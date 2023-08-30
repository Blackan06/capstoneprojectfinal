using DataAccess.Dtos.AnswerDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.QuestionDto
{
    
        // UpdateQuestionDto class
        public class UpdateQuestionDto
        {
            [Required]
            public string MajorName { get; set; }

            [Required]
            public string Name { get; set; }

            [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be 'ACTIVE' or 'INACTIVE'.")]
            public string Status { get; set; }

            public List<GetAnswerData> Answers { get; set; }
         }




}

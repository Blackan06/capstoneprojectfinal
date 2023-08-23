using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.AnswerDto
{
    public class GetAnswerListDto
    {
        public Guid Id { get; set; }
        public string AnswerName { get; set; }
        public bool IsRight { get; set; }
    }
}

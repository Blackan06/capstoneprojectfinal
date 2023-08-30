using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.AnswerDto
{
    public class GetAnswerData
    {
        public Guid AnswerId { get; set; }
        public string AnswerName { get; set; }

    }
}

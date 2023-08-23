using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.QuestionDto
{
    public class QuestionDto : IBaseDto
    {
        public Guid Id { get; set; }

        public string MajorName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}

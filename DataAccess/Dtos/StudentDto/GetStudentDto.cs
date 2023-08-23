using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.StudentDto
{
    public class GetStudentDto : BaseStudentDto, IBaseDto
    {
        public Guid Id { get; set; }
    }
}

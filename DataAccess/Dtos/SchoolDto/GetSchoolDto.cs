using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolDto
{
    public class GetSchoolDto : BaseSchoolDto,IBaseDto
    {
        public Guid Id { get; set; }
    }
}

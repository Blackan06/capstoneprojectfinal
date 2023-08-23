using DataAccess.Dtos.SchoolDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class GetListSchoolByEventIdDTO
    {

        public List<GetSchoolDto> listSchoolDto { get; set; }
    }
}

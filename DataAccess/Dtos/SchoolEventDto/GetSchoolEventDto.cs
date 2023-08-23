using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.SchoolEventDto
{
    public class GetSchoolEventDto : BaseSchoolEventDto , IBaseDto
    {
        public Guid Id { get; set; }
    }
}

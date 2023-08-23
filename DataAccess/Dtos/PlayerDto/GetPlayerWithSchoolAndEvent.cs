using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerDto
{
    public class GetPlayerWithSchoolAndEvent : BasePlayerDto, IBaseDto
    {
        public Guid Id { get ; set ; }

        public Guid schoolId { get ; set ; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.EventTaskDto
{
    public class GetEventTaskDto :BaseEventTaskDto, IBaseDto
    {
        public Guid Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.EventDto
{
    public  class EventDto : BaseEventDto, IBaseDto
    {
        public Guid Id { get; set; }
        

    }
}

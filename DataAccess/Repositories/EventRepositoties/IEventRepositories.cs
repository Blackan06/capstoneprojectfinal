using BusinessObjects.Model;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.EventRepositories
{
    public interface IEventRepositories : IGenericRepository<Event>
    {

        Task<GetTaskAndEventDto> GetTaskAndEventListByTimeNow(Guid schoolId);
    }
}

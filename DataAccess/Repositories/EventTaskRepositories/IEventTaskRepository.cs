using BusinessObjects.Model;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.EventTaskRepositories
{
    public interface IEventTaskRepository : IGenericRepository<EventTask>
    {
        Task<IEnumerable<EventTaskDto>> GetEventTaskByEventId(Guid eventId);
        Task<IEnumerable<EventTaskDto>> GetEventTaskWithTypeByEventId(Guid eventId);
        Task<int> GetPriorityByEventTask(Guid eventId, Guid? majorId);

        Task<IEnumerable<GetTaskByEventIdDto>> GetTaskByEventTaskWithEventId(Guid eventId);

        Task<SchoolEventDto>  GetSchoolEventDto(Guid eventId);
    }
}

using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.TaskDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.EventTaskService
{
    public interface IEventTaskService
    {
        Task<ServiceResponse<IEnumerable<EventTaskDto>>> GetEventTask();
        Task<ServiceResponse<EventTaskDto>> GetEventById(Guid eventId);
        Task<ServiceResponse<EventTaskDto>> GetEventTaskByTaskId(Guid taskId);

        Task<ServiceResponse<Guid>> CreateNewEventTask(CreateEventTaskDto createEventTaskDto);
        Task<ServiceResponse<bool>> UpdateTaskEvent(Guid id,UpdateEventTaskDto eventTaskDto);

        Task<ServiceResponse<IEnumerable<GetTaskByEventIdDto>>> GetTaskByEventTaskWithEventId(Guid eventId);
        Task<ServiceResponse<List<Guid>>> CreateNewEventTasks(CreateListEventTaskDto createEventTaskDtos); 
        Task<ServiceResponse<bool>> DeleteEventTask(Guid id);

    }
}

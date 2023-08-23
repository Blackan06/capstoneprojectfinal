using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.TaskDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.TaskService
{
    public interface ITaskService
    {
        Task<ServiceResponse<IEnumerable<TaskDto>>> GetTask();
        Task<ServiceResponse<GetTaskDto>> GetTaskById(Guid eventId);

        Task<ServiceResponse<Guid>> CreateNewTask(CreateTaskDto createEventDto);
        Task<ServiceResponse<bool>> UpdateTask(Guid id, UpdateTaskDto updateTaskDto);
/*        Task<ServiceResponse<IEnumerable<BusinessObjects.Model.Task>>> GetTaskDoneByMajor(Guid majorId);
*/
        Task<ServiceResponse<bool>> DisableTask(Guid id);

    }
}

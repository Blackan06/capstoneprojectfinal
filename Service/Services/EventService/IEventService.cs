using DataAccess;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.TaskDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.EventService
{
    public interface IEventService 
    {
        Task<ServiceResponse<IEnumerable<GetEventDto>>> GetEvent();
        Task<ServiceResponse<EventDto>> GetEventById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewEvent(CreateEventDto createEventDto);
        Task<ServiceResponse<bool>> UpdateEvent(Guid id,UpdateEventDto eventDto);
        Task<ServiceResponse<IEnumerable<GetTaskAndEventDto>>> GetTaskAndEventListByTimeNow(Guid schoolId);
        Task<ServiceResponse<byte[]>> DownloadExcelTemplate();
        Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file);
        Task<ServiceResponse<PagedResult<EventDto>>> GetEventWithPage(QueryParameters queryParameters);

        Task<ServiceResponse<bool>> DisableEvent(Guid id);    

    }
}

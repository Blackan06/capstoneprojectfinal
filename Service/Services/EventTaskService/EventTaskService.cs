using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.TaskRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.EventTaskService
{
    public class EventTaskService : IEventTaskService
    {
        private readonly IEventTaskRepository _eventTaskRepository;
        private readonly IEventRepositories _eventRepository;
        private readonly ITaskRepositories _taskRepository;
        private readonly IMapper _mapper;
       
        public EventTaskService(IEventTaskRepository eventTaskRepository, IMapper mapper, IEventRepositories eventRepositories, ITaskRepositories taskRepository)
        {
            _eventTaskRepository = eventTaskRepository;
            _eventRepository = eventRepositories;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }
        private DateTime TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            return dateTime;
        }
        public async Task<ServiceResponse<Guid>> CreateNewEventTask(CreateEventTaskDto createEventTaskDto)
        {
            var existingEvent = await _eventRepository.GetAsync(createEventTaskDto.EventId);
            if (existingEvent == null)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Event does not exist.",
                    Success = false,
                    StatusCode = 400
                };
            }

            // Kiểm tra xem có công việc nào trùng EventId và TaskId đã tồn tại trong sự kiện không
            if (await _eventTaskRepository.ExistsAsync(t => t.EventId == createEventTaskDto.EventId && t.Id == createEventTaskDto.TaskId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "A task with the same EventId and TaskId already exists in the event.",
                    Success = false,
                    StatusCode = 400
                };
            }

            // Kiểm tra xem thời gian StartTime và EndTime của công việc nằm trong khoảng thời gian của sự kiện
            if (createEventTaskDto.StartTime >= createEventTaskDto.EndTime)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Task's StartTime and EndTime must be within the event's time range.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if(createEventTaskDto.Point <= 0)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Point must be large than 0.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createEventTaskDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            createEventTaskDto.Status = "ACTIVE";
            var eventTaskcreate = _mapper.Map<EventTask>(createEventTaskDto);
            eventTaskcreate.Id = Guid.NewGuid();

            await _eventTaskRepository.AddAsync(eventTaskcreate);

            return new ServiceResponse<Guid>
            {
                Data = eventTaskcreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<IEnumerable<GetEventTaskDto>>> GetEventTask()
        {
            var eventList = await _eventTaskRepository.GetAllAsync<GetEventTaskDto>();
           
            if (eventList != null)
            {
                eventList = eventList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetEventTaskDto>>
                {
                    Data = eventList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetEventTaskDto>>
                {
                    Data = eventList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<EventTaskDto>> GetEventById(Guid eventId)
        {
            try
            {
                var eventDetail = await _eventTaskRepository.GetAsync<EventTaskDto>(eventId);
                if (eventDetail == null)
                {

                    return new ServiceResponse<EventTaskDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<EventTaskDto>
                {
                    Data = eventDetail,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
      

        public async Task<ServiceResponse<bool>> UpdateTaskEvent(Guid id,UpdateEventTaskDto eventTaskDto)
        {
            var existingEventTask = await _eventTaskRepository.GetAsync(id);
            if (existingEventTask == null)
            {
                return new ServiceResponse<bool>
                {
                    Message = "Task does not exist.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (TimeSpan.Parse(eventTaskDto.StartTime) >= TimeSpan.Parse(eventTaskDto.EndTime))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Task's StartTime and EndTime must be within the event's time range.",
                    Success = false,
                    StatusCode = 400
                };
            }

            if (eventTaskDto.Point <= 0)
            {
                return new ServiceResponse<bool>
                {
                    Message = "Point must be large than 0.",
                    Success = false,
                    StatusCode = 400
                };
            }
            try
            {
                existingEventTask.StartTime = TimeSpan.Parse(eventTaskDto.StartTime);
                existingEventTask.EndTime = TimeSpan.Parse(eventTaskDto.EndTime);
                existingEventTask.Point = eventTaskDto.Point;

                await _eventTaskRepository.UpdateAsync(id, existingEventTask);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success edit",
                    Success = true,
                    StatusCode = 202
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventTaskExists(id))
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Message = "Invalid Record Id",
                        Success = false,
                        StatusCode = 500
                    };
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<bool> EventTaskExists(Guid id)
        {
            return await _eventTaskRepository.Exists(id);
        }

        public async Task<ServiceResponse<EventTaskDto>> GetEventTaskByTaskId(Guid taskId)
        {
            try
            {
                var taskDetail = await _eventTaskRepository.GetByWithCondition(x => x.TaskId == taskId, null, true);
                var taskDetailDto = _mapper.Map<EventTaskDto>(taskDetail);
                if (taskDetail == null)
                {

                    return new ServiceResponse<EventTaskDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<EventTaskDto>
                {
                    Data = taskDetailDto,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetTaskByEventIdDto>>> GetTaskByEventTaskWithEventId(Guid eventId)
        {
            var taskList = await _eventTaskRepository.GetTaskByEventTaskWithEventId(eventId);


            if (taskList.Any())
            {
                return new ServiceResponse<IEnumerable<GetTaskByEventIdDto>>
                {
                    Data = taskList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetTaskByEventIdDto>>
                {
                    Data = taskList,
                    Success = false,
                    Message = "Failed because List task null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<List<Guid>>> CreateNewEventTasks(CreateListEventTaskDto createEventTaskDtos)
        {
            var addedEventTaskIds = new List<Guid>();
            var newEventTasks = new List<EventTask>();

            int index = 0;
            var existingEventTasks = await _eventTaskRepository.GetEventTaskByEventId(createEventTaskDtos.EventId);
            existingEventTasks = existingEventTasks.OrderByDescending(et => et.Priority).ToList();
            var listTaskId = createEventTaskDtos.TaskId.Distinct().ToList();
            foreach (var dto in listTaskId)
            {
                // Check if a task with the same EventId and TaskId already exists
                if (await _eventTaskRepository.ExistsAsync(t => t.EventId == createEventTaskDtos.EventId && t.TaskId == dto))
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "A task with the same EventId and TaskId already exists in the event.",
                        Success = false,
                        StatusCode = 400
                    };
                }               
                if (createEventTaskDtos.Point <= 0)
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "Point must be large than 0.",
                        Success = false,
                        StatusCode = 400
                    };
                }
                // Check if the task's time range overlaps with existing tasks in the event
                var tasksWithinEventTimeRange = await _eventTaskRepository.ExistsAsync(et =>
                    et.EventId == createEventTaskDtos.EventId &&
                    (
                        (TimeSpan.Parse(createEventTaskDtos.StartTime) >= et.StartTime && TimeSpan.Parse(createEventTaskDtos.StartTime) < et.EndTime) ||
                        (TimeSpan.Parse(createEventTaskDtos.EndTime) > et.StartTime && TimeSpan.Parse(createEventTaskDtos.EndTime) <= et.EndTime)
                    ));

                if (tasksWithinEventTimeRange)
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "Task's time range overlaps with existing tasks in the event.",
                        Success = false,
                        StatusCode = 400
                    };
                }
                
                index++;
                int maxExistingPriority = existingEventTasks.Any() ? existingEventTasks.Max(et => et.Priority) : 0;
                createEventTaskDtos.Status = "ACTIVE";
                createEventTaskDtos.Point = createEventTaskDtos.Point;
                createEventTaskDtos.CreatedAt = TimeZoneVietName(createEventTaskDtos.CreatedAt);

                var eventTaskCreate = _mapper.Map<EventTask>(createEventTaskDtos);
                eventTaskCreate.Id = Guid.NewGuid();
                eventTaskCreate.StartTime = TimeSpan.Parse(createEventTaskDtos.StartTime);
                eventTaskCreate.EndTime = TimeSpan.Parse(createEventTaskDtos.EndTime);
                eventTaskCreate.TaskId = dto;
                eventTaskCreate.Priority = maxExistingPriority + index;
                addedEventTaskIds.Add(eventTaskCreate.Id);
                newEventTasks.Add(eventTaskCreate);
            }
            var checkNewEventTasks = newEventTasks.Distinct().ToList();
            await _eventTaskRepository.AddRangeAsync(checkNewEventTasks);

            return new ServiceResponse<List<Guid>>
            {
                Data = addedEventTaskIds,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }
    }
}

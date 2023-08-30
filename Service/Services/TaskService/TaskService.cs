using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.ItemRepositories;
using DataAccess.Repositories.LocationRepositories;
using DataAccess.Repositories.MajorRepositories;
using DataAccess.Repositories.NPCRepository;
using DataAccess.Repositories.QuestionRepositories;
using DataAccess.Repositories.TaskRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task = BusinessObjects.Model.Task;

namespace Service.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepositories _taskRepository;
        private readonly IEventRepositories _eventRepository;
        private readonly IEventTaskRepository _eventTaskRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly INpcRepository _npcRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
       
        public TaskService(IItemRepository itemRepository, ITaskRepositories taskRepository, IMapper mapper,IEventTaskRepository eventTaskRepository , IEventRepositories eventRepositories, IMajorRepository majorRepository, IQuestionRepository questionRepository,ILocationRepository locationRepository,INpcRepository npcRepository)
        {
            _taskRepository = taskRepository;
            _eventRepository = eventRepositories;
            _eventTaskRepository = eventTaskRepository;
            _majorRepository = majorRepository; 
            _questionRepository = questionRepository;
            _locationRepository = locationRepository; 
            _npcRepository = npcRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewTask(CreateTaskDto createTaskDto)
        {
            if (await _taskRepository.ExistsAsync(s => s.Name == createTaskDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Task with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _taskRepository.ExistsAsync(s => s.LocationId == createTaskDto.LocationId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Task with the same location already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _taskRepository.ExistsAsync(s => s.Type == createTaskDto.Type))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Task with the same type already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createTaskDto.Name = createTaskDto.Name.Trim();
            createTaskDto.Type = createTaskDto.Type.Trim();
            createTaskDto.Status = createTaskDto.Status.Trim();
            createTaskDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            var taskcreate = _mapper.Map<Task>(createTaskDto);
            taskcreate.Id = Guid.NewGuid();
            if (taskcreate.Type == "QUESTIONANDANSWER")
            {
                // Kiểm tra nếu ngành đã có câu hỏi đi kèm câu trả lời
                var correspondingMajor = await _majorRepository.GetAsync(taskcreate.MajorId);
                if (correspondingMajor == null)
                {
                    return new ServiceResponse<Guid>
                    {
                        Message = "Can't add jobs. The corresponding branch was not found.",
                        Success = false,
                        StatusCode = 400
                    };
                }

                // Kiểm tra nếu câu hỏi của ngành đã có câu trả lời đi kèm
                var questionWithAnswers = await _questionRepository.GetByMajorIdAsync(correspondingMajor.Id);
                if (questionWithAnswers == null)
                {
                    return new ServiceResponse<Guid>
                    {
                        Message = "Can't add jobs. Industry must have a question with an answer.",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }else if(taskcreate.Type == "EXCHANGEITEM")
            {
                if(createTaskDto.ItemId == null)
                {
                    return new ServiceResponse<Guid>
                    {
                        Message = "Must have item",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }
            await _taskRepository.AddAsync(taskcreate);

            return new ServiceResponse<Guid>
            {
                Data = taskcreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }


    
        public async Task<ServiceResponse<IEnumerable<TaskDto>>> GetTask()
        {
           
            var taskList = await _taskRepository.GetAllAsync<TaskDto>();

            
            if (taskList != null)
            {
                taskList = taskList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<TaskDto>>
                {
                    Data = taskList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<TaskDto>>
                {
                    Data = taskList,
                    Success = false,
                    Message = "Failed because List task null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<GetTaskDto>> GetTaskById(Guid eventId)
        {
            try
            {
                List<Expression<Func<Task, object>>> includes = new List<Expression<Func<Task, object>>>
                {
                    x => x.EventTasks,
                };
                var taskDetail = await _taskRepository.GetByWithCondition(x => x.Id == eventId, includes, true);
                var taskDetailDto = _mapper.Map<GetTaskDto>(taskDetail);
             
                if (taskDetail == null)
                {

                    return new ServiceResponse<GetTaskDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetTaskDto>
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

        public async Task<ServiceResponse<bool>> UpdateTask(Guid id, UpdateTaskDto updateTaskDto)
        {
            
            Guid locationId = await GetLocationIdFromName(updateTaskDto.LocationName);
            Guid majorId = await GetMajorIdFromName(updateTaskDto.MajorName);
            Guid npcId = await GetNpcIdFromName(updateTaskDto.NpcName);
            Guid? itemId = updateTaskDto.ItemName != null ? await GetItemIdFromName(updateTaskDto.ItemName) : (Guid?)null;
            var existingTask = await _taskRepository.GetById(id);

            if (existingTask == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Task not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            var existingTaskWithSameName = await _taskRepository.ExistsAsync(s => s.Name == updateTaskDto.Name && s.Id != id);
            
            if (existingTaskWithSameName)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Task with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingTaskWithSameLocation = await _taskRepository.ExistsAsync(s => s.LocationId == locationId && s.Id != id);

            if (existingTaskWithSameLocation)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Task with the same location already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingTaskWithSameType = await _taskRepository.ExistsAsync(s => s.Type == updateTaskDto.Type && s.Id != id);
            if (existingTaskWithSameLocation)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Task with the same type already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (updateTaskDto.Type == "QUESTIONANDANSWER")
            {
                // Kiểm tra nếu ngành đã có câu hỏi đi kèm câu trả lời
                var correspondingMajor = await _majorRepository.GetAsync(majorId);
                if (correspondingMajor == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Can't add jobs. The corresponding branch was not found.",
                        Success = false,
                        StatusCode = 400
                    };
                }

                // Kiểm tra nếu câu hỏi của ngành đã có câu trả lời đi kèm
                var questionWithAnswers = await _questionRepository.GetByMajorIdAsync(correspondingMajor.Id);
                if (questionWithAnswers == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Can't add jobs. Industry must have a question with an answer.",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }else if(existingTask.Type == "EXCHANGEITEM")
            {
                if(existingTask.Type != updateTaskDto.Type)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Cannot update task with type exchange item ",
                        Success = false,
                        StatusCode = 400
                    };
                }
                if(itemId == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Cannot update task with type exchange item and item not null",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }
            try
            {
                existingTask.LocationId = locationId;
                existingTask.MajorId = majorId;
                existingTask.NpcId = npcId;
                existingTask.ItemId = itemId;
                existingTask.Name = updateTaskDto.Name.Trim();
                existingTask.Type = updateTaskDto.Type.Trim();
                existingTask.Status = updateTaskDto.Status.Trim();
                await _taskRepository.UpdateAsync(id, existingTask);

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
                if (!await CountryExists(id))
                {
                    return new ServiceResponse<bool>
                    {   
                        Data = false,
                        Message = "No rows",
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
        private async Task<bool> CountryExists(Guid id)
        {
            return await _taskRepository.Exists(id);
        }

        public async Task<ServiceResponse<bool>> DisableTask(Guid id)
        {
            var checkEvent = await _taskRepository.GetAsync<TaskDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";
                await _taskRepository.UpdateAsync(id, checkEvent);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
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
        private async Task<Guid> GetLocationIdFromName(string locationName)
        {
            var location = await _locationRepository.GetLocationByName(locationName);
            return location.Id;
        }

        private async Task<Guid> GetMajorIdFromName(string majorName)
        {
            var major = await _majorRepository.GetMajorByName(majorName);
            return major.Id;
        }

        private async Task<Guid> GetNpcIdFromName(string npcName)
        {
            var npc = await _npcRepository.GetNpcDTOByName(npcName);
            return npc.Id;
        } 
        private async Task<Guid> GetItemIdFromName(string itemName)
        {
            var item = await _itemRepository.GetItemByName(itemName);
            return item.Id;
        }

    }
}

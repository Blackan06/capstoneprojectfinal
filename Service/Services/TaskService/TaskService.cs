using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.MajorRepositories;
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
        private readonly IQuestionRepository _questionRepository;

        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public TaskService(ITaskRepositories taskRepository, IMapper mapper,IEventTaskRepository eventTaskRepository , IEventRepositories eventRepositories, IMajorRepository majorRepository, IQuestionRepository questionRepository)
        {
            _taskRepository = taskRepository;
            _eventRepository = eventRepositories;
            _eventTaskRepository = eventTaskRepository;
            _majorRepository = majorRepository; 
            _questionRepository = questionRepository;
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
            createTaskDto.Name = createTaskDto.Name.Trim();
            createTaskDto.Type = createTaskDto.Type.Trim();
            createTaskDto.Status = createTaskDto.Status.Trim();
            TimeZoneVietName(createTaskDto.CreatedAt);

            var mapper = config.CreateMapper();
            var taskcreate = mapper.Map<Task>(createTaskDto);
            taskcreate.Id = Guid.NewGuid();
            if (taskcreate.Type == "questionandanswer")
            {
                // Kiểm tra nếu ngành đã có câu hỏi đi kèm câu trả lời
                var correspondingMajor = await _majorRepository.GetAsync(taskcreate.MajorId);
                if (correspondingMajor == null)
                {
                    return new ServiceResponse<Guid>
                    {
                        Message = "Không thể thêm công việc. Ngành tương ứng không được tìm thấy.",
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
                        Message = "Không thể thêm công việc. Ngành phải có một câu hỏi đi kèm câu trả lời.",
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
                var _mapper = config.CreateMapper();
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
            var existingSchoolWithSameName = await _taskRepository.ExistsAsync(s => s.Name == updateTaskDto.Name && s.Id != id);
            var existingSchoolWithSameEmail = await _taskRepository.ExistsAsync(s => s.LocationId == updateTaskDto.LocationId && s.Id != id);

            if (existingSchoolWithSameName)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Task with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (existingSchoolWithSameEmail)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Task with the same location already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (updateTaskDto.Type == "questionandanswer")
            {
                // Kiểm tra nếu ngành đã có câu hỏi đi kèm câu trả lời
                var correspondingMajor = await _majorRepository.GetAsync(updateTaskDto.MajorId);
                if (correspondingMajor == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Không thể thêm công việc. Ngành tương ứng không được tìm thấy.",
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
                        Message = "Không thể thêm công việc. Ngành phải có một câu hỏi đi kèm câu trả lời.",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }
            try
            {
                updateTaskDto.Name = updateTaskDto.Name.Trim();
                updateTaskDto.Type = updateTaskDto.Type.Trim();
                updateTaskDto.Status = updateTaskDto.Status.Trim();
                await _taskRepository.UpdateAsync(id, updateTaskDto);
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

        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
        }
    }
}

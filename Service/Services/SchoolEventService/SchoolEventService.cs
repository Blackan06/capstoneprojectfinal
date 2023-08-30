using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.SchoolEventRepositories;
using DataAccess.Repositories.SchoolRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Service.Services.SchoolEventService
{
    public class SchoolEventService : ISchoolEventService
    {
        private readonly ISchoolEventRepository _schoolEventRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IEventRepositories _eventRepository;
        private readonly IMapper _mapper;
     
        public SchoolEventService(ISchoolEventRepository roleRepository, IMapper mapper, ISchoolRepository schoolRepository , IEventRepositories eventRepositories)
        {
            _schoolEventRepository = roleRepository;
            _eventRepository = eventRepositories;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewSchoolEvent(CreateSchoolEventDto createSchoolEventDto)
        {
            var allSchoolEvents = await _schoolEventRepository.GetAllAsync();

            var overlappingEvents = allSchoolEvents.Where(se =>
                                                        se.SchoolId == createSchoolEventDto.SchoolId &&
                                                        se.StartTime <= createSchoolEventDto.EndTime &&
                                                        se.EndTime >= createSchoolEventDto.StartTime)
                                                    .ToList();

            if (overlappingEvents.Any())
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Overlapping events detected for the selected school and time range.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (createSchoolEventDto.StartTime >= createSchoolEventDto.EndTime)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "StartTime must be earlier than EndTime.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (createSchoolEventDto.StartTime <= DateTime.Now)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "StartTime cannot be in the past.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createSchoolEventDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            var schoolEventcreate = _mapper.Map<SchoolEvent>(createSchoolEventDto);
            schoolEventcreate.Id = Guid.NewGuid();

            await _schoolEventRepository.AddAsync(schoolEventcreate);

            return new ServiceResponse<Guid>
            {
                Data = schoolEventcreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<IEnumerable<SchoolEventDto>>> GetSchoolEvent()
        {
            var schoolEventList = await _schoolEventRepository.GetAllAsync<SchoolEventDto>();

            if (schoolEventList != null)
            {
                schoolEventList = schoolEventList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<SchoolEventDto>>
                {
                    Data = schoolEventList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<SchoolEventDto>>
                {
                    Data = schoolEventList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
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
        public async Task<ServiceResponse<SchoolEventDto>> GetSchoolEventById(Guid eventId)
        {
            try
            {
                var eventDetail = await _schoolEventRepository.GetAsync<SchoolEventDto>(eventId);
                if (eventDetail == null)
                {

                    return new ServiceResponse<SchoolEventDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<SchoolEventDto>
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

        public async Task<ServiceResponse<bool>> UpdateSchoolEvent(Guid id, UpdateSchoolEventDto schoolEventDto)
        {
            // Kiểm tra trùng lịch sự kiện với sự kiện khác (chỉ khi cập nhật StartTime và EndTime)
            if (schoolEventDto.StartTime != null && schoolEventDto.EndTime != null)
            {
                var allSchoolEvents = await _schoolEventRepository.GetAllAsync();

                var existingSchoolEvents =  allSchoolEvents.Where(se =>
                             se.SchoolId == schoolEventDto.SchoolId &&
                             se.Id != id &&
                             se.StartTime <= schoolEventDto.EndTime &&
                             se.EndTime >= schoolEventDto.StartTime).ToList();


                if (existingSchoolEvents.Any())
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "There are overlapping events within the specified time range.",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }

            // Kiểm tra thời gian bắt đầu và kết thúc
            if (schoolEventDto.StartTime >= schoolEventDto.EndTime)
            {
                return new ServiceResponse<bool>
                {
                    Message = "StartTime must be before EndTime.",
                    Success = false,
                    StatusCode = 400
                };
            }

            // Kiểm tra xem trường học và sự kiện có tồn tại không
            if (!await _schoolRepository.ExistsAsync(s => s.Id == schoolEventDto.SchoolId))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Invalid SchoolId or EventId.",
                    Success = false,
                    StatusCode = 400
                };
            }

            // Kiểm tra sự kiện có tồn tại và không trùng dữ liệu với sự kiện khác
            var existingEvent = await _schoolEventRepository.GetById(id);
            if (existingEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Message = "Invalid Record Id",
                    Success = false,
                    StatusCode = 500
                };
            }
            existingEvent.Status = schoolEventDto.Status;
            existingEvent.StartTime = schoolEventDto.StartTime;
            existingEvent.EndTime = schoolEventDto.EndTime;
            existingEvent.ApprovalStatus = schoolEventDto.ApprovalStatus;
            existingEvent.SchoolId = schoolEventDto.SchoolId;

            await _schoolEventRepository.UpdateAsync(existingEvent);
            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "Success edit",
                Success = true,
                StatusCode = 202
            };
        }

        public async Task<ServiceResponse<bool>> UpdateSchoolEventByStartTimeAndEndTime(Guid id, UpdateSchoolEventDto schoolEventDto)
        {
            if (schoolEventDto.StartTime != null && schoolEventDto.EndTime != null)
            {
                var allSchoolEvents = await _schoolEventRepository.GetAllAsync();

                var existingSchoolEvents = allSchoolEvents.Where(se =>
                             se.SchoolId == schoolEventDto.SchoolId &&
                             se.Id != id &&
                             se.StartTime <= schoolEventDto.EndTime &&
                             se.EndTime >= schoolEventDto.StartTime).ToList();


                if (existingSchoolEvents.Any())
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "There are overlapping events within the specified time range.",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }

            // Kiểm tra thời gian bắt đầu và kết thúc
            if (schoolEventDto.StartTime >= schoolEventDto.EndTime)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,

                    Message = "StartTime must be earlier than EndTime.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (schoolEventDto.StartTime <= DateTime.Now)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "StartTime cannot be in the past.",
                    Success = false,
                    StatusCode = 400
                };
            }
            try
            {
                var existingSchoolEvent = await _schoolEventRepository.GetById(id);
                existingSchoolEvent.StartTime = schoolEventDto.StartTime;
                existingSchoolEvent.EndTime = schoolEventDto.EndTime;
            
                await _schoolEventRepository.UpdateAsync(id, existingSchoolEvent);
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
            return await _schoolEventRepository.Exists(id);
        }
        public async Task<ServiceResponse<List<GetSchoolByEventIdDto>>> GetSchoolByEventId(Guid eventid)
        {
            var schoolList = await _schoolEventRepository.GetSchoolByEventId(eventid);

            if (schoolList != null)
            {
                return new ServiceResponse<List<GetSchoolByEventIdDto>>
                {
                    Data = schoolList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<List<GetSchoolByEventIdDto>>
                {
                    Data = schoolList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<List<Guid>>> CreateNewSchoolEventList(CreateListSchoolEvent createSchoolEventDtos)
        {
            var addedEventTaskIds = new List<Guid>();
            var newEventTasks = new List<SchoolEvent>();


            foreach (var schoolId in createSchoolEventDtos.SchoolIds)
            {
                var existingEvents = await _schoolEventRepository.GetAllAsync();

                var conflictingEvents = existingEvents.Where(se => se.SchoolId == schoolId &&
                                                   se.StartTime <= createSchoolEventDtos.EndTime &&
                                                   se.EndTime >= createSchoolEventDtos.StartTime);
                if (conflictingEvents.Any())
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = $"There is an existing event for schoolId {schoolId} within the specified time range.",
                        Success = false,
                        StatusCode = 400
                    };
                }
                // Check if StartTime is earlier than EndTime
                if (createSchoolEventDtos.StartTime >= createSchoolEventDtos.EndTime ||
                    createSchoolEventDtos.StartTime.AddHours(2) > createSchoolEventDtos.EndTime)
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "Task's StartTime must be earlier than EndTime.",
                        Success = false,
                        StatusCode = 400
                    };
                }
                createSchoolEventDtos.ApprovalStatus = "ACCEPT";
                createSchoolEventDtos.Status = "ACTIVE";
                createSchoolEventDtos.CreatedAt =  TimeZoneVietName(DateTime.UtcNow);
                var schoolEventCreate = _mapper.Map<SchoolEvent>(createSchoolEventDtos);
                schoolEventCreate.Id = Guid.NewGuid();
                schoolEventCreate.SchoolId = schoolId;
                addedEventTaskIds.Add(schoolEventCreate.Id);
                newEventTasks.Add(schoolEventCreate);
            
            };


            // Tiếp tục xử lý và thêm dữ liệu vào cơ sở dữ liệu

            await _schoolEventRepository.AddRangeAsync(newEventTasks);

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

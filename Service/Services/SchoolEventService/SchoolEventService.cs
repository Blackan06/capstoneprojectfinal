using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Repositories.SchoolEventRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.SchoolEventService
{
    public class SchoolEventService : ISchoolEventService
    {
        private readonly ISchoolEventRepository _schoolEventRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public SchoolEventService(ISchoolEventRepository roleRepository, IMapper mapper)
        {
            _schoolEventRepository = roleRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewSchoolEvent(CreateSchoolEventDto createSchoolEventDto)
        {
            if (await _schoolEventRepository.ExistsAsync(se => se.EventId == createSchoolEventDto.EventId && se.SchoolId == createSchoolEventDto.SchoolId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: School Event with the same EventId and SchoolId already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            TimeZoneVietName(createSchoolEventDto.CreatedAt);

            var mapper = config.CreateMapper();
            var eventTaskcreate = mapper.Map<SchoolEvent>(createSchoolEventDto);
            eventTaskcreate.Id = Guid.NewGuid();

            await _schoolEventRepository.AddAsync(eventTaskcreate);

            return new ServiceResponse<Guid>
            {
                Data = eventTaskcreate.Id,
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
        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
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
            if (await _schoolEventRepository.ExistsAsync(se => se.EventId == schoolEventDto.EventId && se.SchoolId == schoolEventDto.SchoolId && se.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: School Event with the same EventId and SchoolId already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingSchoolEvent = await _schoolEventRepository.GetById(id);

            if (existingSchoolEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Inventory not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {
                existingSchoolEvent.Status = schoolEventDto.Status;
                existingSchoolEvent.StartTime = schoolEventDto.StartTime;
                existingSchoolEvent.EndTime = schoolEventDto.EndTime;

                await _schoolEventRepository.UpdateAsync(id, schoolEventDto);
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
        public async Task<ServiceResponse<List<GetSchoolDto>>> GetSchoolByEventId(Guid eventid)
        {
            var schoolList = await _schoolEventRepository.GetSchoolByEventId(eventid);

            if (schoolList != null)
            {
                return new ServiceResponse<List<GetSchoolDto>>
                {
                    Data = schoolList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<List<GetSchoolDto>>
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
            var mapper = config.CreateMapper();

            var newEventTasks = new List<SchoolEvent>();

            foreach (var dto in createSchoolEventDtos.SchoolIds)
            {
                // Check if a task with the same EventId and TaskId already exists
                if (await _schoolEventRepository.ExistsAsync(t => t.EventId == createSchoolEventDtos.EventId && t.SchoolId == dto))
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "A task with the same EventId and TaskId already exists in the event.",
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
                createSchoolEventDtos.Approvalstatus = "ACCEPT";
                createSchoolEventDtos.Status = "ACTIVE";
                TimeZoneVietName(createSchoolEventDtos.CreatedAt);
                var eventTaskCreate = mapper.Map<SchoolEvent>(createSchoolEventDtos);
                eventTaskCreate.Id = Guid.NewGuid();
                eventTaskCreate.SchoolId = dto;
                addedEventTaskIds.Add(eventTaskCreate.Id);
                newEventTasks.Add(eventTaskCreate);

            }
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

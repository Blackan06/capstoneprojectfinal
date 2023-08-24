using AutoMapper;
using BusinessObjects.Model;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Exceptions;
using DataAccess.Repositories.EventRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IEventRepositories _eventRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public EventService(IEventRepositories eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
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
        public async Task<ServiceResponse<Guid>> CreateNewEvent(CreateEventDto createEventDto)
        {
           
            if (await _eventRepository.ExistsAsync(e => e.Name == createEventDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: An event with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createEventDto.Name = createEventDto.Name.Trim();
            createEventDto.Status = createEventDto.Status.Trim();
            createEventDto.CreatedAt = TimeZoneVietName(createEventDto.CreatedAt);
            var eventcreate = _mapper.Map<Event>(createEventDto);
            eventcreate.Id = Guid.NewGuid();
            await _eventRepository.AddAsync(eventcreate);

            return new ServiceResponse<Guid>
            {
                Data = eventcreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }
       

        public async Task<ServiceResponse<IEnumerable<GetEventDto>>> GetEvent()
        {
            var eventList = await _eventRepository.GetAllAsync();
            var _mapper = config.CreateMapper();
            var lstDto = _mapper.Map<List<GetEventDto>>(eventList);
            if (eventList != null)
            {
                lstDto = lstDto.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetEventDto>>
                {
                    Data = lstDto,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetEventDto>>
                {
                    Data = lstDto,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<EventDto>> GetEventById(Guid eventId)
        {
            try
            {
                List<Expression<Func<Event, object>>> includes = new List<Expression<Func<Event, object>>>
                {
                    x => x.SchoolEvents,
                    x => x.EventTasks,
                };
                var eventDetail = await _eventRepository.GetByWithCondition(x => x.Id == eventId, includes, true);
                var _mapper = config.CreateMapper();
                var eventDetailDto = _mapper.Map<EventDto>(eventDetail);
                if (eventDetail == null)
                {

                    return new ServiceResponse<EventDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<EventDto>
                {
                    Data = eventDetailDto,
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

       
        public async Task<ServiceResponse<bool>> UpdateEvent(Guid id, UpdateEventDto eventDto)
        {
            var existingEvent = await _eventRepository.GetById(id);

            if (existingEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Event not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            if (await _eventRepository.ExistsAsync(e => e.Name == eventDto.Name && e.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: An event with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            try
            {
                existingEvent.Name = eventDto.Name.Trim();
                existingEvent.Status = eventDto.Status.Trim();
                await _eventRepository.UpdateAsync(id, existingEvent);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Update Success",
                    Success = true,
                    StatusCode = 200
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
            return await _eventRepository.Exists(id);
        }


        public async Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file)
        {

            if (file == null || file.Length <= 0)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Message = "No file uploaded.",
                    Success = false,
                    StatusCode = 400
                };
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Message = "Invalid file format. Only Excel files are allowed.",
                    Success = false,
                    StatusCode = 400
                };
            }
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null || worksheet.Dimension == null || worksheet.Dimension.Rows <= 1)
                        {
                            return new ServiceResponse<string>
                            {
                                Data = null,
                                Message = "Excel file does not contain data starting from row 2.",
                                Success = false,
                                StatusCode = 400
                            };
                        }
                        else 
                        {
                            var rowCount = worksheet.Dimension.Rows;
                            var dataList = new List<EventDto>();
                            var errorMessages = new List<string>();

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var data = new EventDto
                                {
                                    Id = Guid.NewGuid(),
                                    Name = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                                    Status = worksheet.Cells[row, 2].Value?.ToString().Trim()
                                };
                                data.CreatedAt = TimeZoneVietName(data.CreatedAt);

                                if (string.IsNullOrEmpty(data.Name))
                                {
                                    errorMessages.Add($"Row {row}: Fullname is required.");
                                }
                                dataList.Add(data);
                            }

                            // Start from row 2 to skip the header row

                            var mapper = config.CreateMapper();
                            var locations = _mapper.Map<List<Event>>(dataList);
                            await _eventRepository.AddRangeAsync(locations);
                            await _eventRepository.SaveChangesAsync();

                        }
                    }
                }
                return new ServiceResponse<string>
                {
                    Data = "Upload successful.",
                    Success = true,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Data = ex.Message,
                    Message = "Failed to process uploaded file.",
                    Success = false,
                    StatusCode = 500
                };
            }
        }
        public byte[] GenerateExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SampleDataEvent");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "Event Name";
                worksheet.Cells[1, 2].Value = "Status";

                

                // Lưu file Excel vào MemoryStream
                var stream = new MemoryStream(package.GetAsByteArray());
                return stream.ToArray();
            }
        }






        public async Task<ServiceResponse<byte[]>> DownloadExcelTemplate()
        {
            byte[] fileContents;
            try
            {
                fileContents = GenerateExcelTemplate();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return new ServiceResponse<byte[]>
                {
                    Data = null,
                    Message = "Failed to generate Excel template.",
                    Success = false,
                    StatusCode = 500
                };
            }

            // Trả về file Excel dưới dạng byte array
            return new ServiceResponse<byte[]>
            {
                Data = fileContents,
                Success = true,
                StatusCode = 200
            };
        }


      
        public async Task<ServiceResponse<PagedResult<EventDto>>> GetEventWithPage(QueryParameters queryParameters)
        {
            var pagedsResult = await _eventRepository.GetAllAsync<EventDto>(queryParameters);
            return new ServiceResponse<PagedResult<EventDto>>
            {
                Data = pagedsResult,
                Message = "Successfully",
                StatusCode = 200,
                Success = true
            };
        }


        public async Task<ServiceResponse<GetTaskAndEventDto>> GetTaskAndEventListByTimeNow(Guid schoolId)
        {
            var events = await _eventRepository.GetTaskAndEventListByTimeNow(schoolId);
            if(events == null)
            {
                return new ServiceResponse<GetTaskAndEventDto>
                {
                    Data = null,
                    Message = "Failed Data null",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                return new ServiceResponse<GetTaskAndEventDto>
                {
                    Data = events,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

        public async Task<ServiceResponse<bool>> DisableEvent(Guid id)
        {
            var checkEvent = await _eventRepository.GetAsync<EventDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";

                await _eventRepository.UpdateAsync(id, checkEvent);
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
    }
}

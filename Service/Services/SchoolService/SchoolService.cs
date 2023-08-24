using AutoMapper;
using BusinessObjects.Model;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Repositories.SchoolEventRepositories;
using DataAccess.Repositories.SchoolRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service.Services.SchoolService
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;
      
        public SchoolService(ISchoolRepository schoolRepository, IMapper mapper)
        {
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewSchool(CreateSchoolDto createSchoolDto)
        {
            // Kiểm tra xem dữ liệu đã tồn tại trong cơ sở dữ liệu hay chưa
            if (await _schoolRepository.ExistsAsync(s => s.Name == createSchoolDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: School with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            } 
            if (await _schoolRepository.ExistsAsync(s => s.Address == createSchoolDto.Address))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: School with the same address already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createSchoolDto.Address = createSchoolDto.Address.Trim();
            createSchoolDto.Name = createSchoolDto.Name.Trim();
            createSchoolDto.Email = createSchoolDto.Email.Trim();
            createSchoolDto.CreatedAt = TimeZoneVietName(createSchoolDto.CreatedAt);

            // Tiếp tục tiến hành tạo mới dữ liệu
            var eventTaskcreate = _mapper.Map<School>(createSchoolDto);
            eventTaskcreate.Id = Guid.NewGuid();
            await _schoolRepository.AddAsync(eventTaskcreate);

            return new ServiceResponse<Guid>
            {
                Data = eventTaskcreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
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

        public async Task<ServiceResponse<IEnumerable<GetSchoolDto>>> GetSchool()
        {
            var schoolList = await _schoolRepository.GetAllAsync<GetSchoolDto>();

            if (schoolList != null)
            {
                schoolList = schoolList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetSchoolDto>>
                {
                    Data = schoolList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetSchoolDto>>
                {
                    Data = schoolList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<SchoolDto>> GetSchoolById(Guid eventId)
        {
            try
            {

                var eventDetail = await _schoolRepository.GetAsync<SchoolDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<SchoolDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<SchoolDto>
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
        public async Task<ServiceResponse<PagedResult<SchoolDto>>> GetSchoolWithPage(QueryParameters queryParameters)
        {
            var pagedsResult = await _schoolRepository.GetAllAsync<SchoolDto>(queryParameters);
            return new ServiceResponse<PagedResult<SchoolDto>>
            {
                Data = pagedsResult,
                Message = "Successfully",
                StatusCode = 200,
                Success = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateSchool(Guid id, UpdateSchoolDto schoolDto)
        {
            // Kiểm tra xem dữ liệu đã tồn tại trong cơ sở dữ liệu hay chưa
            var existingSchoolWithSameName = await _schoolRepository.ExistsAsync(s => s.Name == schoolDto.Name && s.Id != id);
            var existingSchoolWithSameEmail = await _schoolRepository.ExistsAsync(s => s.Email == schoolDto.Email && s.Id != id);

            if (existingSchoolWithSameName)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: School with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (existingSchoolWithSameEmail)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: School with the same email already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingSchool = await _schoolRepository.GetById(id);

            if (existingSchool == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "School not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {

                existingSchool.Address = schoolDto.Address.Trim();
                existingSchool.Name = schoolDto.Name.Trim();
                existingSchool.Email = schoolDto.Email.Trim();
                existingSchool.Status = schoolDto.Status.Trim();
                await _schoolRepository.UpdateAsync(existingSchool);
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
            return await _schoolRepository.Exists(id);
        }
        public async Task<ServiceResponse<bool>> DisableSchool(Guid id)
        {
            var checkEvent = await _schoolRepository.GetAsync<SchoolDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Failed",
                    StatusCode = 400,
                    Success = false
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";

                await _schoolRepository.UpdateAsync(id, checkEvent);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

       

        public async Task<ServiceResponse<IEnumerable<School>>> GetSchoolByName(string schoolname)
        {
            var schoolList = await _schoolRepository.GetSchoolByName(schoolname.Trim());

            if (schoolList != null)
            {
                return new ServiceResponse<IEnumerable<School>>
                {
                    Data = schoolList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<School>>
                {
                    Data = schoolList,
                    Success = false,
                    Message = "Faile because List school null",
                    StatusCode = 200
                };
            }
        }
    }
}

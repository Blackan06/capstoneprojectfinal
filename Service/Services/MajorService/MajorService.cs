using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.MajorRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Service.Services.MajorService
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IMapper _mapper;
    
        public MajorService(IMajorRepository majorRepository, IMapper mapper)
        {
            _majorRepository = majorRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewMajor(CreateMajorDto createMajorDto)
        {
            if (await _majorRepository.ExistsAsync(s => s.Name == createMajorDto.Name ))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Major with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createMajorDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            createMajorDto.Status = createMajorDto.Status.Trim();
            createMajorDto.Description = createMajorDto.Description.Trim();
            createMajorDto.Name = createMajorDto.Name.Trim();
            var createMajor = _mapper.Map<Major>(createMajorDto);
            createMajor.Id = Guid.NewGuid();
      
            await _majorRepository.AddAsync(createMajor);

            return new ServiceResponse<Guid>
            {
                Data = createMajor.Id,
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
        public async Task<ServiceResponse<MajorDto>> GetMajorById(Guid majorId)
        {
            try
            {

                var eventDetail = await _majorRepository.GetAsync<MajorDto>(majorId);
               
                if (eventDetail == null)
                {

                    return new ServiceResponse<MajorDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<MajorDto>
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

        public async Task<ServiceResponse<IEnumerable<GetMajorDto>>> GetMajor()
        {
            var majorList = await _majorRepository.GetAllAsync<GetMajorDto>();

            if (majorList != null)
            {
                majorList = majorList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetMajorDto>>
                {
                    Data = majorList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetMajorDto>>
                {
                    Data = majorList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }
        public async Task<ServiceResponse<bool>> UpdateMajor(Guid id, UpdateMajorDto updateMajorDto)
        {
            if (await _majorRepository.ExistsAsync(s => s.Name == updateMajorDto.Name && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Major with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingMajor = await _majorRepository.GetById(id);

            if (existingMajor == null)
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
                existingMajor.Status = updateMajorDto.Status.Trim();
                existingMajor.Description = updateMajorDto.Description.Trim();
                existingMajor.Name = updateMajorDto.Name.Trim();
                await _majorRepository.UpdateAsync(id, existingMajor);
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
            return await _majorRepository.Exists(id);
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
                            var dataList = new List<MajorDto>();
                            var errorMessages = new List<string>();
                            var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns];
                            var expectedHeaders = new List<string>
                            {
                                "Major Name", "Description"
                            };

                            // Kiểm tra tên cột trong tệp Excel
                            foreach (var cell in headerRow)
                            {
                                if (!expectedHeaders.Contains(cell.Text.Trim()))
                                {
                                    return new ServiceResponse<string>
                                    {
                                        Data = null,
                                        Message = "Invalid column names in the Excel file.",
                                        Success = false,
                                        StatusCode = 400
                                    };
                                }
                            }
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var data = new MajorDto
                                {
                                    Id = Guid.NewGuid(),
                                    Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                    Description = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                    Status = "ACTIVE",
                                };
                                data.CreatedAt = TimeZoneVietName(data.CreatedAt);

                                if (string.IsNullOrEmpty(data.Name))
                                {
                                    errorMessages.Add($"Row {row}: Fullname is required.");
                                }
                                dataList.Add(data);
                            }

                            // Start from row 2 to skip the header row

                            var locations = _mapper.Map<List<Major>>(dataList);
                            await _majorRepository.AddRangeAsync(locations);
                            await _majorRepository.SaveChangesAsync();

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
                    Data = null,
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
                var worksheet = package.Workbook.Worksheets.Add("SampleDataMajor");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "Major Name";
                worksheet.Cells[1, 2].Value = "Description";
               

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

        public async Task<string> getMajorName(Guid id)
        {
            var eventDetail = await _majorRepository.GetAsync<MajorDto>(id);
            if(eventDetail == null)
            {
                return null;
            }
            else
            {
                return eventDetail.Name;

            }

        }

        public async Task<ServiceResponse<bool>> DisableMajor(Guid id)
        {
            var checkEvent = await _majorRepository.GetAsync<MajorDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Failed",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";

                await _majorRepository.UpdateAsync(id, checkEvent);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
    }
}

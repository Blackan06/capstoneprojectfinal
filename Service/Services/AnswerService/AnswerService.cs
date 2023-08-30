using AutoMapper;
using BusinessObjects.Model;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Repositories.AnswerRepositories;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.NPCRepository;
using DataAccess.Repositories.QuestionRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.AnswerService
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        private readonly INpcRepository _npcRepository;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public AnswerService(IAnswerRepository answerRepository, IMapper mapper, IQuestionRepository questionRepository,INpcRepository npcRepository)
        {
            _answerRepository = answerRepository;
            _mapper = mapper;
            _questionRepository = questionRepository;
            _npcRepository = npcRepository;
        }
        public async Task<ServiceResponse<Guid>> CreateNewAnswer(CreateAnswerDto createAnswerDto)
        {
            
            createAnswerDto.AnswerName = createAnswerDto.AnswerName.Trim();
            createAnswerDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            var answerCreate = _mapper.Map<Answer>(createAnswerDto);
            answerCreate.Id = Guid.NewGuid();

            await _answerRepository.AddAsync(answerCreate);

            return new ServiceResponse<Guid>
            {
                Data = answerCreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        

        public async Task<ServiceResponse<IEnumerable<AnswerDto>>> GetAnswer()
        {
            var answerList = await _answerRepository.GetAllAsync<AnswerDto>();
           

            if (answerList != null)
            {
                answerList = answerList.OrderByDescending(e => e.CreatedAt).ToList();
                return new ServiceResponse<IEnumerable<AnswerDto>>
                {
                    Data = answerList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<AnswerDto>>
                {
                    Data = null,
                    Success = true,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<AnswerDto>> GetAnswerById(Guid eventId)
        {
            try
            {

                var eventDetail = await _answerRepository.GetAsync<AnswerDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<AnswerDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<AnswerDto>
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

      

        public async Task<ServiceResponse<PagedResult<AnswerDto>>> GetAnswerWithPage(QueryParameters queryParameters)
        {
            var pagedsResult = await _answerRepository.GetAllAsync<AnswerDto>(queryParameters);
            return new ServiceResponse<PagedResult<AnswerDto>>
            {
                Data = pagedsResult,
                Message = "Successfully",
                StatusCode = 200,
                Success = true
            };
        }

       

        public async Task<ServiceResponse<IEnumerable<GetAnswerAndQuestionNameDto>>> GetListQuestionByMajorIdAsync(Guid majorId)
        {
            var listAnswer = await _answerRepository.GetListQuestionByMajorIdAsync(majorId);
            if(listAnswer == null)
            {
                return new ServiceResponse<IEnumerable<GetAnswerAndQuestionNameDto>>
                {
                    Data = null,
                    Success = false,
                    Message = "No answers found for the specified NPC name.",
                    StatusCode = 404
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetAnswerAndQuestionNameDto>>
                {
                    Data = listAnswer,
                    Success = true,
                    Message = "Successfully",
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

        public async Task<ServiceResponse<bool>> UpdateAnswer(Guid id, UpdateAnswerDto answerDto)
        {
            try
            {
                answerDto.AnswerName = answerDto.AnswerName.Trim();

                await _answerRepository.UpdateAsync(id, answerDto);
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
                if (!await AnswerExists(id))
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

        private async Task<bool> AnswerExists(Guid id)
        {
            return await _answerRepository.Exists(id);
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
                        if (worksheet != null)
                        {
                            var rowCount = worksheet.Dimension.Rows;
                            var dataList = new List<GetAnswerDto>();

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var rawIsRightValue = worksheet.Cells[row, 2].Value?.ToString();
                                bool isRight;

                                if (!string.IsNullOrEmpty(rawIsRightValue) && bool.TryParse(rawIsRightValue, out isRight))
                                {
                                    var data = new GetAnswerDto
                                    {
                                        Id = Guid.NewGuid(),
                                        AnswerName = worksheet.Cells[row, 1].Value?.ToString(),
                                        IsRight = isRight
                                    };

                                    dataList.Add(data);
                                }
                                else
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

                            var mapper = config.CreateMapper();
                            var locations = _mapper.Map<List<Answer>>(dataList);
                            await _answerRepository.AddRangeAsync(locations);
                            await _answerRepository.SaveChangesAsync();
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
                var worksheet = package.Workbook.Worksheets.Add("SampleDataAnswer");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "Answer Name";
                worksheet.Cells[1, 2].Value = "isRight";


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
    }
}

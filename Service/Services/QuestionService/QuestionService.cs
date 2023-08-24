using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.MajorDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Dtos.PrizeDto;
using DataAccess.Dtos.QuestionDto;
using DataAccess.Repositories.AnswerRepositories;
using DataAccess.Repositories.MajorRepositories;
using DataAccess.Repositories.QuestionRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IMapper _mapper;
     
        public QuestionService(IQuestionRepository questionRepository, IMapper mapper, IMajorRepository majorRepository, IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
            _answerRepository = answerRepository;
            _majorRepository = majorRepository;
        }
        public async Task<ServiceResponse<Guid>> CreateNewQuestion(CreateQuestionDto createQuestionDto)
        {
            if (await _questionRepository.ExistsAsync(s => s.Name == createQuestionDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Question with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            TimeZoneVietName(createQuestionDto.CreatedAt);

            createQuestionDto.Name = createQuestionDto.Name.Trim();
            createQuestionDto.Status = createQuestionDto.Status.Trim();
            createQuestionDto.CreatedAt = TimeZoneVietName(createQuestionDto.CreatedAt);
            var createQuestion = _mapper.Map<Question>(createQuestionDto);
            createQuestion.Id = Guid.NewGuid();
            await _questionRepository.AddAsync(createQuestion);

            return new ServiceResponse<Guid>
            {
                Data = createQuestion.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }


        public async Task<ServiceResponse<Guid>> CreateNewQuestionAndAnswer(QuestionAndAnswerDto createQuestionDto)
        {
            var isFirstAnswer = true;

            if (await _questionRepository.ExistsAsync(q => q.Name == createQuestionDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Question with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }

            createQuestionDto.Name = createQuestionDto.Name.Trim();
            createQuestionDto.Status = createQuestionDto.Status.Trim();

            if (createQuestionDto.Answers == null || createQuestionDto.Answers.Count < 2 && createQuestionDto.Answers.Count > 5)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "You must provide at least 2 answers for each question.",
                    Success = false,
                    StatusCode = 400
                };
            }

            // Check for duplicate answer names
            var distinctAnswerNames = createQuestionDto.Answers.Select(a => a.AnswerName).Distinct();
            if (distinctAnswerNames.Count() < createQuestionDto.Answers.Count)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicate answer names are not allowed.",
                    Success = false,
                    StatusCode = 400
                };
            }

            var correctAnswerCount = createQuestionDto.Answers.Count(a => a.IsRight);

            if (correctAnswerCount != 1)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "There must be exactly one correct answer.",
                    Success = false,
                    StatusCode = 400
                };
            }

            var newQuestion = new CreateQuestionDto
            {
                MajorId = createQuestionDto.MajorId,
                Name = createQuestionDto.Name.Trim(),
                Status = createQuestionDto.Status.Trim(),
            };
            var question = _mapper.Map<Question>(newQuestion);

            question.Id = Guid.NewGuid();
            await _questionRepository.AddAsync(question);

            foreach (var answerText in createQuestionDto.Answers)
            {
                var answer = _mapper.Map<Answer>(answerText);
                answer.Id = Guid.NewGuid();
                answer.QuestionId = question.Id;
                answer.AnswerName = answerText.AnswerName;
                answer.IsRight = isFirstAnswer && answerText.IsRight; 
            
                await _answerRepository.AddAsync(answer);
                
                isFirstAnswer = false; 

            }

            return new ServiceResponse<Guid>
            {
                Data = question.Id,
                Message = "Successfully created question with user-provided answer options.",
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

        public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestion()
        {
            var majorList = await _questionRepository.GetAllAsync<QuestionDto>();

            
            if (majorList != null)
            {
                return new ServiceResponse<IEnumerable<QuestionDto>>
                {
                    Data = majorList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<QuestionDto>>
                {
                    Data = majorList,
                    Success = false,
                    Message = "Faile because List question null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<QuestionDto>> GetQuestionById(Guid eventId)
        {
            try
            {

                var eventDetail = await _questionRepository.GetAsync<QuestionDto>(eventId);
               
                if (eventDetail == null)
                {

                    return new ServiceResponse<QuestionDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<QuestionDto>
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
       

        public async Task<ServiceResponse<bool>> UpdateQuestion(Guid id, UpdateQuestionDto updateQuestionDto)
        {
            if (await _questionRepository.ExistsAsync(s => s.Name == updateQuestionDto.Name && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Question with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            try
            {
                updateQuestionDto.Name = updateQuestionDto.Name.Trim();
                updateQuestionDto.Status = updateQuestionDto.Status.Trim();
                await _questionRepository.UpdateAsync(id, updateQuestionDto);
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
            return await _questionRepository.Exists(id);
        }

        public async Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file)
        {
            Dictionary<string, Guid?> majorDictionary = new Dictionary<string, Guid?>();

            var majors = await _majorRepository.GetAllAsync<GetMajorDto>();

            foreach (var major in majors)
            {
                majorDictionary.Add(major.Name, major.Id);
            }

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
                            var dataList = new List<QuestionDto>();

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var data = new QuestionDto
                                {
                                    Id = Guid.NewGuid(),
                                    Name = worksheet.Cells[row, 1].Value.ToString(),
                                    MajorName = worksheet.Cells[row, 2].Value.ToString(),

                                };

                                dataList.Add(data);
                            }

                            // Start from row 2 to skip the header row

                            var locations = _mapper.Map<List<Question>>(dataList);
                            await _questionRepository.AddRangeAsync(locations);
                            await _questionRepository.SaveChangesAsync();

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
        public byte[] GenerateExcelTemplate(List<GetMajorDto> majors)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SampleDataQuestion");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "Question Name";
                worksheet.Cells[1, 2].Value = "Major Name";

                // Điền dữ liệu Major vào tệp Excel
               
                
                var majorNameColumn = worksheet.Cells[2, 2, majors.Count + 1, 2];
                var validation = majorNameColumn.DataValidation.AddListDataValidation();
                validation.Formula.ExcelFormula = $"=MajorList";

                // Tạo Name Range cho danh sách Major Name
                var majorListRange = worksheet.Names.Add("MajorList", worksheet.Cells[2, 2, majors.Count + 1, 2]);

                // Thiết lập công thức để trích xuất danh sách Major Name
                majorListRange.Formula = $"='{worksheet.Name}'!$B$2:$B${majors.Count + 1}";

                // Đặt các tên tương ứng với danh sách Major Name
                for (int i = 0; i < majors.Count; i++)
                {
                    var majorCell = worksheet.Cells[i + 2, 2];
                    majorCell.Value = majors[i].Name;
                }

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
                var majors = await _majorRepository.GetAllAsync<GetMajorDto>();
                fileContents = GenerateExcelTemplate(majors);
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
        public async Task<ServiceResponse<bool>> DisableQuestion(Guid id)
        {
            var checkEvent = await _questionRepository.GetAsync<QuestionDto>(id);

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
                var question = _mapper.Map<Question>(checkEvent);

                await _questionRepository.UpdateAsync(id, question);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ListQuestionAndAnswer>>> GetQuestionAndAnswersAsync()
        {
            var listQuestionAndAnswer = await _questionRepository.GetQuestionAndAnswersAsync();
            if(listQuestionAndAnswer == null)
            {
                return new ServiceResponse<IEnumerable<ListQuestionAndAnswer>>
                {
                    Message = "Data Null",
                    StatusCode = 200,
                    Success = true
                };
            }
            return new ServiceResponse<IEnumerable<ListQuestionAndAnswer>>
            {
                Data = listQuestionAndAnswer,
                Message = "Success",
                StatusCode = 200,
                Success = true
            };
        }
    }
}

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
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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

                        var rowCount = worksheet.Dimension.Rows;
                        var questionsWithAnswers = new List<(GetQuestionDto question, List<GetAnswerDto> answers)>();
                        var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns];
                        var expectedHeaders = new List<string>
                            {
                                "Question Name", "Major Name","Answer A","Is Correct","Answer B","Is Correct","Answer C","Is Correct","Answer D","Is Correct"
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
                            var questionName = worksheet.Cells[row, 1].Value?.ToString();
                            var majorName = worksheet.Cells[row, 2].Value?.ToString();
                            var major = await _majorRepository.GetMajorByName(majorName);
                            if (string.IsNullOrWhiteSpace(questionName))
                            {
                                // Skip rows with empty question names
                                continue;
                            }

                            var question = new GetQuestionDto
                            {
                                Id = Guid.NewGuid(),
                                Name = questionName,
                                MajorId = major.Id,
                                Status = "ACTIVE",
                                CreatedAt = TimeZoneVietName(DateTime.UtcNow),
                            };

                            var answers = new List<GetAnswerDto>();

                            for (int col = 3; col <= 10; col += 2)
                            {
                                var answerName = worksheet.Cells[row, col].Value?.ToString();
                                var isCorrectText = worksheet.Cells[row, col + 1].Value?.ToString()?.ToLower();

                                if (!string.IsNullOrWhiteSpace(answerName) && !string.IsNullOrWhiteSpace(isCorrectText))
                                {
                                    bool isCorrect = isCorrectText == "true";
                                    var answer = new GetAnswerDto
                                    {
                                        Id = Guid.NewGuid(),
                                        AnswerName = answerName,
                                        IsRight = isCorrect,
                                        QuestionId = question.Id,
                                        CreatedAt = TimeZoneVietName(DateTime.UtcNow)
                                    };
                                    answers.Add(answer);
                                }
                            }
                            if (answers.Count(answer => answer.IsRight) > 1)
                            {
                                return new ServiceResponse<string>
                                {
                                    Data = null,
                                    Message = "Multiple correct answers found for a question.",
                                    Success = false,
                                    StatusCode = 400
                                };
                            }

                            if (answers.Any())
                            {
                                questionsWithAnswers.Add((question, answers));
                            }
                        }

                        foreach (var (question, answers) in questionsWithAnswers)
                        {
                            // Save question and its answers to the database
                            var questionEntity = _mapper.Map<Question>(question);
                            await _questionRepository.AddAsync(questionEntity);

                            var answerEntities = _mapper.Map<List<Answer>>(answers);
                            foreach (var answerEntity in answerEntities)
                            {
                                answerEntity.QuestionId = questionEntity.Id;
                            }

                            await _answerRepository.AddRangeAsync(answerEntities);
                        }

                        await _answerRepository.SaveChangesAsync();
                        await _questionRepository.SaveChangesAsync();

                        return new ServiceResponse<string>
                        {
                            Data = "Upload successful.",
                            Success = true,
                            StatusCode = 200
                        };
                    }
                }
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


        public byte[] GenerateExcelTemplate(List<GetMajorDto> majors)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("QuestionAndAnswerTemplate");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "Question Name";
                worksheet.Cells[1, 2].Value = "Major Name";
                worksheet.Cells[1, 3].Value = "Answer A";
                worksheet.Cells[1, 4].Value = "Is Correct";
                worksheet.Cells[1, 5].Value = "Answer B";
                worksheet.Cells[1, 6].Value = "Is Correct";
                worksheet.Cells[1, 7].Value = "Answer C";
                worksheet.Cells[1, 8].Value = "Is Correct";
                worksheet.Cells[1, 9].Value = "Answer D";
                worksheet.Cells[1, 10].Value = "Is Correct";
                for (int col = 4; col <= 10; col += 2)
                {
                    var isCorrectColumn = worksheet.Cells[2, col, majors.Count + 1, col];
                    var isCorrectValidation = isCorrectColumn.DataValidation.AddListDataValidation();
                    isCorrectValidation.Formula.Values.Add("TRUE");
                    isCorrectValidation.Formula.Values.Add("FALSE");
                    isCorrectValidation.HideDropDown = false;
                    isCorrectValidation.Prompt = "Choose an option from the list";
                }
                var majorNames = majors.Select(major => major.Name).ToList();

                // Tạo dropdown list cho cột Major Name
                var majorNameColumn = worksheet.Cells[2, 2, majors.Count + 1, 2];
                var majorValidation = majorNameColumn.DataValidation.AddListDataValidation();
                foreach (var option in majorNames)
                {
                    majorValidation.Formula.Values.Add(option); // Replace with the appropriate property
                }
                // Tạo công thức danh sách từ danh sách tên chuyên ngành

                majorValidation.HideDropDown = false; // Hiển thị dropdown
                majorValidation.Prompt = "Choose a major from the list";

                // Save the Excel package to a stream
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
                if (majors == null || majors.Count == 0)
                {
                    return new ServiceResponse<byte[]>
                    {
                        Data = null,
                        Message = "Failed to generate Excel template.",
                        Success = false,
                        StatusCode = 500
                    };
                }
                else
                {
                    fileContents = GenerateExcelTemplate(majors);

                }
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
            if (listQuestionAndAnswer == null)
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
        public async Task<ServiceResponse<bool>> UpdateQuestionAndAnswer(Guid id, UpdateQuestionDto updateQuestionDto)
        {
            var question = await _questionRepository.GetById(id);

            if (question == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Question not found",
                    Success = false,
                    StatusCode = 404
                };
            }

            if (await _questionRepository.ExistsAsync(q => q.Name == updateQuestionDto.Name && q.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicated data: Question with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var major = await _majorRepository.GetMajorByName(updateQuestionDto.MajorName);
            if (major == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Not found major",
                    Success = false,
                    StatusCode = 400
                };
            }

            question.MajorId = major.Id;
            question.Name = updateQuestionDto.Name;
            question.Status = updateQuestionDto.Status;

            await _questionRepository.UpdateAsync(question);

            // Update or add answers
            foreach (var answer in updateQuestionDto.Answers)
            {
                var existingAnswer = await _answerRepository.GetById(answer.AnswerId);

                if (existingAnswer != null)
                {
                    existingAnswer.AnswerName = answer.AnswerName;
                    await _answerRepository.UpdateAsync(answer.AnswerId, existingAnswer);
                }
               
            }

            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "Successfully updated question and answer options.",
                Success = true,
                StatusCode = 200
            };
        }


    }
}

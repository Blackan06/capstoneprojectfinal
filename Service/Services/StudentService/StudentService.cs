using AutoMapper;
using BusinessObjects.Model;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Dtos.LocationDto;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.Repositories.SchoolEventRepositories;
using DataAccess.Repositories.SchoolRepositories;
using DataAccess.Repositories.StudentRepositories;
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

namespace Service.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepositories _studentRepository;
        private readonly ISchoolEventRepository _schoolEventRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepositories studentRepository, IMapper mapper, ISchoolEventRepository schoolEventRepository, ISchoolRepository schoolRepository)
        {
            _studentRepository = studentRepository;
            _schoolEventRepository = schoolEventRepository;
            _mapper = mapper;
            _schoolRepository = schoolRepository;
        }

        public async Task<ServiceResponse<Guid>> CreateNewStudent(CreateStudentDto createStudentDto)
        {
            if (await _studentRepository.ExistsAsync(s => s.Email == createStudentDto.Email))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Student with the same email already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _studentRepository.ExistsAsync(s => s.Phonenumber.ToString() == createStudentDto.Phonenumber))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Student with the same phone number already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createStudentDto.Email = createStudentDto.Email.Trim();
            createStudentDto.GraduateYear = createStudentDto.GraduateYear;
            createStudentDto.Classname = createStudentDto.Classname.Trim();
            createStudentDto.Fullname = createStudentDto.Fullname.Trim();
            createStudentDto.Status = createStudentDto.Status.Trim();
            createStudentDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            var createStudent = _mapper.Map<Student>(createStudentDto);
            createStudent.Id = Guid.NewGuid();
            await _studentRepository.AddAsync(createStudent);

            return new ServiceResponse<Guid>
            {
                Data = createStudent.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<bool>> DisableStudent(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<StudentDto>>> GetStudent()
        {
            var studentList = await _studentRepository.GetAllAsync<StudentDto>();

            if (studentList != null)
            {
                studentList = studentList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<StudentDto>>
                {
                    Data = studentList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<StudentDto>>
                {
                    Data = studentList,
                    Success = false,
                    Message = "Faile because List student null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<StudentDto>> GetStudentById(Guid studentId)
        {
            try
            {

                var eventDetail = await _studentRepository.GetAsync<StudentDto>(studentId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<StudentDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<StudentDto>
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

        public async Task<ServiceResponse<PagedResult<StudentDto>>> GetStudentWithPage(QueryParameters queryParameters)
        {
            var pagedsResult = await _studentRepository.GetAllAsync<StudentDto>(queryParameters);
            return new ServiceResponse<PagedResult<StudentDto>>
            {
                Data = pagedsResult,
                Message = "Successfully",
                StatusCode = 200,
                Success = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateStudent(Guid id, UpdateStudentDto studentDto)
        {
            if (await _studentRepository.ExistsAsync(s => s.Email == studentDto.Email && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Student with the same email already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _studentRepository.ExistsAsync(s => s.Phonenumber.ToString() == studentDto.Phonenumber && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Student with the same Phone Number already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingStudent = await _studentRepository.GetById(id);

            if (existingStudent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Student not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {

                existingStudent.Email = studentDto.Email.Trim();
                existingStudent.GraduateYear = studentDto.GraduateYear;
                existingStudent.Classname = studentDto.Classname.Trim();
                existingStudent.Fullname = studentDto.Fullname.Trim();
                existingStudent.Status = studentDto.Status.Trim();
                await _studentRepository.UpdateAsync(id, existingStudent);
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
                if (!await StudentExists(id))
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

        public async Task<ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>> GetStudentBySchoolId(Guid id)
        {
            var studentList = await _studentRepository.GetStudentBySchoolId(id);

            if (studentList != null)
            {
                return new ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>
                {
                    Data = studentList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>
                {
                    Data = studentList,
                    Success = false,
                    Message = "Faile because List student null",
                    StatusCode = 200
                };
            }
        }
        private async Task<bool> StudentExists(Guid id)
        {
            return await _studentRepository.Exists(id);
        }
        public byte[] GenerateExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SampleDataStudent");

                // Thiết lập header cho các cột
                worksheet.Cells[1, 1].Value = "FULL NAME";
                worksheet.Cells[1, 2].Value = "EMAIL";
                worksheet.Cells[1, 3].Value = "GRADUATE YEAR";
                worksheet.Cells[1, 4].Value = "PHONE NUMBER";
                worksheet.Cells[1, 5].Value = "CLASS NAME";
                // Định dạng cột Class Name thành kiểu "Text"
                worksheet.Column(1).Style.Numberformat.Format = "@";
                worksheet.Column(2).Style.Numberformat.Format = "@";
                worksheet.Column(3).Style.Numberformat.Format = "0";
                worksheet.Column(4).Style.Numberformat.Format = "0";
                worksheet.Column(5).Style.Numberformat.Format = "@";
                worksheet.Column(6).Style.Numberformat.Format = "@";
              
                // Lưu file Excel vào MemoryStream
                var stream = new MemoryStream(package.GetAsByteArray());
                return stream.ToArray();
            }
        }
        public async Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file, Guid schoolEventId)
        {
            var checkSchoolEventId = await _schoolEventRepository.GetSchoolByEventIdCheck(schoolEventId);
            var dateTimeNow = TimeZoneVietName(DateTime.UtcNow);
            if (checkSchoolEventId == null || dateTimeNow >= checkSchoolEventId.StartTime && dateTimeNow <= checkSchoolEventId.EndTime)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Message = "Not found school event or event school is active",
                    Success = false,
                    StatusCode = 400
                };
            }

            else
            {
                // Kiểm tra nếu không có file hoặc kích thước file <= 0
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

                // Kiểm tra định dạng file
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
                                var dataList = new List<GetStudentDto>();
                                var errorMessages = new List<string>();

                                var importedEmails = new HashSet<string>();
                                var importedPhoneNumbers = new HashSet<string>();
                                var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns];
                                var expectedHeaders = new List<string>
                                {
                                "FULL NAME", "EMAIL",  "GRADUATE YEAR", "PHONE NUMBER", "CLASS NAME"
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
                                    var data = new GetStudentDto
                                    {
                                        // Khởi tạo dữ liệu từ các ô trong tệp Excel
                                        Id = Guid.NewGuid(),
                                        SchoolId = checkSchoolEventId.SchoolId,
                                        Fullname = worksheet.Cells[row, 1].Text.Trim(),
                                        Email = worksheet.Cells[row, 2].Text.Trim(),
                                        GraduateYear = int.Parse(worksheet.Cells[row, 3].Text.Trim()),
                                        Phonenumber = worksheet.Cells[row, 4].Text.Trim(),
                                        Classname = worksheet.Cells[row, 5].Text.Trim(),
                                        CreatedAt = TimeZoneVietName(DateTime.UtcNow),
                                        Status = "ACTIVE"
                                    };

                                    // Kiểm tra tính hợp lệ của dữ liệu
                                    if (string.IsNullOrEmpty(data.Fullname))
                                    {
                                        errorMessages.Add($"Row {row}: Fullname is required.");
                                    }
                                    var emailExists = await _studentRepository.ExistsAsync(s => s.Email == data.Email);

                                    if (string.IsNullOrEmpty(data.Email) || !IsValidEmail(data.Email) || importedEmails.Equals(data.Email) || emailExists)
                                    {
                                        errorMessages.Add($"Row {row}: Invalid or duplicate email.");
                                    }
                                    else
                                    {
                                        importedEmails.Add(data.Email);
                                    }
                                    var phoneNumberExists = await _studentRepository.ExistsAsync(s => s.Phonenumber == long.Parse(data.Phonenumber));

                                    if (string.IsNullOrEmpty(data.Phonenumber) || importedPhoneNumbers.Equals(data.Phonenumber) || phoneNumberExists)
                                    {
                                        errorMessages.Add($"Row {row}: Invalid or duplicate phone number.");
                                    }
                                    else
                                    {
                                        importedPhoneNumbers.Add(data.Phonenumber);
                                    }

                                    // Xử lý dữ liệu và thêm vào danh sách
                                    dataList.Add(data);
                                }

                                // Kiểm tra lỗi
                                if (errorMessages.Any())
                                {
                                    return new ServiceResponse<string>
                                    {
                                        Data = null,
                                        Message = string.Join("\n", errorMessages),
                                        Success = false,
                                        StatusCode = 400
                                    };
                                }

                                // Tiến hành lưu dữ liệu vào cơ sở dữ liệu
                                var locations = _mapper.Map<List<Student>>(dataList);
                                await _studentRepository.AddRangeAsync(locations);
                                await _studentRepository.SaveChangesAsync();

                                return new ServiceResponse<string>
                                {
                                    Data = "Upload successful.",
                                    Success = true,
                                    StatusCode = 200
                                };
                            }
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
           
        }

        private bool IsValidEmail(string email)
        {
            // Implement email validation logic based on your requirements
            // You can use regular expressions or other methods to validate the email format
            // For simplicity, we will just check if the email contains "@" symbol.
            return email.Contains("@");
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

        public async Task<ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>> GetStudentByEventSchoolId(Guid schoolEventId)
        {
            try
            {

                var eventDetail = await _studentRepository.GetStudentByEventSchoolId(schoolEventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>
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

        public async Task<byte[]> ExportDataToExcelStudent(Guid schoolId)
        {
            var dataList = await _studentRepository.GetStudentsBySchoolIdAsync(schoolId); // Replace with your repository method to get all locations

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");

                // Add header row
                worksheet.Cells[1, 1].Value = "FULL NAME";
                worksheet.Cells[1, 2].Value = "EMAIL";
                worksheet.Cells[1, 3].Value = "CLASS NAME";
                worksheet.Cells[1, 4].Value = "GRADUATE YEAR";
                worksheet.Cells[1, 5].Value = "SCHOOL NAME";
                worksheet.Cells[1, 6].Value = "PHONE NUMBER";
                worksheet.Cells[1, 7].Value = "PASSCODE";
                int row = 2;

                foreach (var student in dataList)
                {
                    worksheet.Cells[row, 1].Value = student.Fullname;
                    worksheet.Cells[row, 2].Value = student.Email;
                    worksheet.Cells[row, 3].Value = student.Classname;
                    worksheet.Cells[row, 4].Value = student.GraduateYear;
                    worksheet.Cells[row, 5].Value = student.Schoolname;
                    worksheet.Cells[row, 6].Value = student.Phonenumber;
                    worksheet.Cells[row, 7].Value = student.Passcode;

                    row++;
                }

                // Convert the package to a byte array
                byte[] excelData = package.GetAsByteArray();
                return excelData;
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

        public async Task<ServiceResponse<string>> ImportDataFromExcelStudentWithSchoolId(IFormFile file, Guid schoolId)
        {
            var checkSchoolId = await _schoolRepository.GetById(schoolId);
            var dateTimeNow = TimeZoneVietName(DateTime.UtcNow);
            if (checkSchoolId == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Message = "Not found school",
                    Success = false,
                    StatusCode = 400
                };
            }

            else
            {
                // Kiểm tra nếu không có file hoặc kích thước file <= 0
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

                // Kiểm tra định dạng file
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
                                var dataList = new List<GetStudentDto>();
                                var errorMessages = new List<string>();

                                var importedEmails = new HashSet<string>();
                                var importedPhoneNumbers = new HashSet<string>();
                                var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns];
                                var expectedHeaders = new List<string>
                                {
                                "FULL NAME", "EMAIL",  "GRADUATE YEAR", "PHONE NUMBER", "CLASS NAME"
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
                                    var data = new GetStudentDto
                                    {
                                        // Khởi tạo dữ liệu từ các ô trong tệp Excel
                                        Id = Guid.NewGuid(),
                                        SchoolId = checkSchoolId.Id,
                                        Fullname = worksheet.Cells[row, 1].Text.Trim(),
                                        Email = worksheet.Cells[row, 2].Text.Trim(),
                                        GraduateYear = int.Parse(worksheet.Cells[row, 3].Text.Trim()),
                                        Phonenumber = worksheet.Cells[row, 4].Text.Trim(),
                                        Classname = worksheet.Cells[row, 5].Text.Trim(),
                                        CreatedAt = TimeZoneVietName(DateTime.UtcNow),
                                        Status = "ACTIVE"
                                    };

                                    // Kiểm tra tính hợp lệ của dữ liệu
                                    if (string.IsNullOrEmpty(data.Fullname))
                                    {
                                        errorMessages.Add($"Row {row}: Fullname is required.");
                                    }
                                    var emailExists = await _studentRepository.ExistsAsync(s => s.Email == data.Email);

                                    if (string.IsNullOrEmpty(data.Email) || !IsValidEmail(data.Email) || importedEmails.Equals(data.Email) || emailExists)
                                    {
                                        errorMessages.Add($"Row {row}: Invalid or duplicate email.");
                                    }
                                    else
                                    {
                                        importedEmails.Add(data.Email);
                                    }
                                    var phoneNumberExists = await _studentRepository.ExistsAsync(s => s.Phonenumber == long.Parse(data.Phonenumber));

                                    if (string.IsNullOrEmpty(data.Phonenumber) || importedPhoneNumbers.Equals(data.Phonenumber) || phoneNumberExists)
                                    {
                                        errorMessages.Add($"Row {row}: Invalid or duplicate phone number.");
                                    }
                                    else
                                    {
                                        importedPhoneNumbers.Add(data.Phonenumber);
                                    }

                                    // Xử lý dữ liệu và thêm vào danh sách
                                    dataList.Add(data);
                                }

                                // Kiểm tra lỗi
                                if (errorMessages.Any())
                                {
                                    return new ServiceResponse<string>
                                    {
                                        Data = null,
                                        Message = string.Join("\n", errorMessages),
                                        Success = false,
                                        StatusCode = 400
                                    };
                                }

                                // Tiến hành lưu dữ liệu vào cơ sở dữ liệu
                                var locations = _mapper.Map<List<Student>>(dataList);
                                await _studentRepository.AddRangeAsync(locations);
                                await _studentRepository.SaveChangesAsync();

                                return new ServiceResponse<string>
                                {
                                    Data = "Upload successful.",
                                    Success = true,
                                    StatusCode = 200
                                };
                            }
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

        }
    }
}

using DataAccess.Dtos.SchoolDto;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Dtos.StudentDto;
using BusinessObjects.Model;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Service.Services.StudentService
{
    public interface IStudentService
    {
        Task<ServiceResponse<IEnumerable<StudentDto>>> GetStudent();
        Task<ServiceResponse<StudentDto>> GetStudentById(Guid studentId);
        Task<ServiceResponse<Guid>> CreateNewStudent(CreateStudentDto createStudentDto);
        Task<ServiceResponse<bool>> UpdateStudent(Guid id, UpdateStudentDto studentDto);
        Task<ServiceResponse<PagedResult<StudentDto>>> GetStudentWithPage(QueryParameters queryParameters);
        Task<ServiceResponse<bool>> DisableStudent(Guid id);
        Task<ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>> GetStudentBySchoolId(Guid id);
        Task<ServiceResponse<byte[]>> DownloadExcelTemplate();
        Task<ServiceResponse<string>> ImportDataFromExcel(IFormFile file, Guid schoolEventId);
        Task<ServiceResponse<string>> ImportDataFromExcelStudentWithSchoolId(IFormFile file, Guid schoolId);
        Task<ServiceResponse<IEnumerable<GetStudentBySchoolAndEvent>>> GetStudentByEventSchoolId(Guid schoolEventId);

        Task<byte[]> ExportDataToExcelStudent(Guid schoolId);
    }
}

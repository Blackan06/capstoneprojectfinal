using AutoMapper;
using DataAccess.Dtos.SchoolDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Services.SchoolService;
using Service.Services.StudentService;
using System.Threading.Tasks;
using System;
using DataAccess.Dtos.StudentDto;
using DataAccess;
using Microsoft.AspNetCore.Authorization;

namespace FPTHCMAdventuresAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsController(IMapper mapper, IStudentService studentService)
        {
            this._mapper = mapper;
            _studentService = studentService;
        }
        [HttpGet(Name = "GetStudent")]

        public async Task<ActionResult<ServiceResponse<StudentDto>>> GetStudentList()
        {
            try
            {
                var res = await _studentService.GetStudent();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("GetStudentBySchoolId/{schoolId}")]

        public async Task<ActionResult<ServiceResponse<GetStudentBySchoolAndEvent>>> GetStudentListBySchoolId(Guid schoolId)
        {
            try
            {
                var res = await _studentService.GetStudentBySchoolId(schoolId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("student/pagination", Name = "GetStudentListWithPagination")]

        public async Task<ActionResult<ServiceResponse<StudentDto>>> GetStudentListWithPage([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var pagedsResult = await _studentService.GetStudentWithPage(queryParameters);
                return Ok(pagedsResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(Guid id)
        {
            var eventDetail = await _studentService.GetStudentById(id);
            return Ok(eventDetail);
        }
        [HttpGet]
        [Route("getstudentbyeventschoolId/{eventschoolId}")]
        public async Task<ActionResult<GetStudentBySchoolAndEvent>> GetStudentByEventSchoolId(Guid eventschoolId)
        {
            var schoolEvent = await _studentService.GetStudentByEventSchoolId(eventschoolId);
            return Ok(schoolEvent);
        }
      
        [HttpPost("student", Name = "CreateNewStudent")]

        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> CreateNewStudent( CreateStudentDto studentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _studentService.CreateNewStudent(studentDto);
                if (!res.Success)
                {
                    return BadRequest(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]

        public async Task<ActionResult<ServiceResponse<GetStudentDto>>> UpdateStudent(Guid id, [FromBody] UpdateStudentDto studentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _studentService.UpdateStudent(id, studentDto);
                if (!res.Success)
                {
                    return BadRequest(res);
                }
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("uploadfileexcelstudent")]
        public async Task<IActionResult> UploadExcel(IFormFile file,Guid schooleventId)
        {
            var serviceResponse = await _studentService.ImportDataFromExcel(file, schooleventId);

            if (serviceResponse.Success)
            {
                // Xử lý thành công
                return Ok(serviceResponse.Message);
            }
            else
            {
                // Xử lý lỗi
                return BadRequest(serviceResponse.Message);
            }
        }
        [HttpPost]
        [Route("uploadfileexcelstudentwithschoolId")]
        public async Task<IActionResult> UploadExcelStudentWithSchoolId(IFormFile file,Guid schoolId)
        {
            var serviceResponse = await _studentService.ImportDataFromExcelStudentWithSchoolId(file, schoolId);

            if (serviceResponse.Success)
            {
                // Xử lý thành công
                return Ok(serviceResponse.Message);
            }
            else
            {
                // Xử lý lỗi
                return BadRequest(serviceResponse.Message);
            }
        }

        [HttpGet("excel-template-student")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var serviceResponse = await _studentService.DownloadExcelTemplate();

            if (serviceResponse.Success)
            {
                // Trả về file Excel dưới dạng response
                return new FileContentResult(serviceResponse.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "SampleDataStudent.xlsx"
                };
            }
            else
            {
                // Xử lý lỗi nếu có
                return BadRequest(serviceResponse.Message);
            }
        }

        [HttpGet("export/{schoolId}")]
        public async Task<IActionResult> ExportLocationsToExcel(Guid schoolId)
        {
            var excelData = await _studentService.ExportDataToExcelStudent(schoolId);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
        }
    }
}

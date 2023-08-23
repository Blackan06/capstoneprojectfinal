using AutoMapper;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.EventTaskService;
using Service;
using System.Threading.Tasks;
using System;
using DataAccess.Repositories.AnswerRepositories;
using DataAccess.Dtos.AnswerDto;
using Service.Services.AnswerService;
using Microsoft.AspNetCore.Authorization;
using DataAccess;

namespace FPTHCMAdventuresAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly IMapper _mapper;

        public AnswersController(IMapper mapper, IAnswerService answerService)
        {
            this._mapper = mapper;
            _answerService = answerService;
        }


        [HttpGet(Name = "GetAnswerList")]

        public async Task<ActionResult<ServiceResponse<AnswerDto>>> GetAnswerList()
        {
            try
            {
                var res = await _answerService.GetAnswer();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("GetAnswers/{majorId}", Name = "GetAnswersByMajor")]

        public async Task<ActionResult<ServiceResponse<GetAnswerAndQuestionNameDto>>> GetAnswerListWithQuestionId(Guid majorId)
        {
            try
            {
                var res = await _answerService.GetListQuestionByMajorIdAsync(majorId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("/pagination" ,Name = "GetAnswerListWithPagination")]

        public async Task<ActionResult<ServiceResponse<AnswerDto>>> GetAnswerListWithPage([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var pagedsResult = await _answerService.GetAnswerWithPage(queryParameters);
                return Ok(pagedsResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerDto>> GetAnswerById(Guid id)
        {
            var eventDetail = await _answerService.GetAnswerById(id);
            return Ok(eventDetail);
        }

        [HttpPost("answer", Name = "CreateNewAnswer")]

        public async Task<ActionResult<ServiceResponse<GetAnswerDto>>> CreateNewanswer(CreateAnswerDto answerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _answerService.CreateNewAnswer(answerDto);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPut("{id}")]

        public async Task<ActionResult<ServiceResponse<GetAnswerDto>>> Updateanswer(Guid id, [FromBody] UpdateAnswerDto eventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _answerService.UpdateAnswer(id, eventDto);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("upload-excel-answer")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var serviceResponse = await _answerService.ImportDataFromExcel(file);

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

        [HttpGet("excel-template-answer")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var serviceResponse = await _answerService.DownloadExcelTemplate();

            if (serviceResponse.Success)
            {
                // Trả về file Excel dưới dạng response
                return new FileContentResult(serviceResponse.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "SampleDataMajor.xlsx"
                };
            }
            else
            {
                // Xử lý lỗi nếu có
                return BadRequest(serviceResponse.Message);
            }
        }

    }
}
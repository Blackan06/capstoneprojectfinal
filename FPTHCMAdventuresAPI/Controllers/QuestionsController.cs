using AutoMapper;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.MajorDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.MajorService;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.QuestionService;
using DataAccess.Dtos.QuestionDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IMapper _mapper;

        public QuestionsController(IMapper mapper, IQuestionService questionService)
        {
            this._mapper = mapper;
            _questionService = questionService;
        }


        [HttpGet(Name = "GetQuestionList")]

        public async Task<ActionResult<ServiceResponse<QuestionDto>>> GetQuestionList()
        {
            try
            {
                var res = await _questionService.GetQuestion();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        } 
        [HttpGet]
        [Route("GetQuestionListAndAnswer")]

        public async Task<ActionResult<ServiceResponse<ListQuestionAndAnswer>>> GetQuestionListAndAnswer()
        {
            try
            {
                var res = await _questionService.GetQuestionAndAnswersAsync();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MajorDto>> GetEventTaskById(Guid id)
        {
            var eventDetail = await _questionService.GetQuestionById(id);
            return Ok(eventDetail);
        }

        [HttpPost("question", Name = "CreateQuestion")]

        public async Task<ActionResult<ServiceResponse<GetQuestionDto>>> CreateNewQuestion(CreateQuestionDto createQuestionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _questionService.CreateNewQuestion(createQuestionDto);
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
        [HttpPost("question/answer", Name = "CreateQuestionAnswer")]

        public async Task<ActionResult<ServiceResponse<QuestionAndAnswerDto>>> CreateNewQuestionAndAnswer(QuestionAndAnswerDto createQuestionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _questionService.CreateNewQuestionAndAnswer(createQuestionDto);
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

        public async Task<ActionResult<ServiceResponse<GetQuestionDto>>> UpdateQuestion(Guid id, [FromBody] UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _questionService.UpdateQuestion(id, updateQuestionDto);
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

        [HttpPost("upload-excel-question")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var serviceResponse = await _questionService.ImportDataFromExcel(file);

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

        [HttpGet("excel-template-question")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var serviceResponse = await _questionService.DownloadExcelTemplate();

            if (serviceResponse.Success)
            {
                // Trả về file Excel dưới dạng response
                return new FileContentResult(serviceResponse.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "SampleDataQuestion.xlsx"
                };
            }
            else
            {
                // Xử lý lỗi nếu có
                return BadRequest(serviceResponse.Message);
            }
        }

        [HttpDelete("disablequestion")]
        public async Task<ActionResult<ServiceResponse<QuestionDto>>> DisableStatusQuestion(Guid id)
        {
            try
            {
                var disableEvent = await _questionService.DisableQuestion(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}


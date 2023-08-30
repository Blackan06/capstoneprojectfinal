using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Model;
using DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Service;
using System.Net;
using Service.Services.EventService;
using AutoMapper;
using DataAccess.Dtos.EventDto;
using DataAccess.Exceptions;
using DataAccess;
using DataAccess.Dtos.TaskDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.AnswerDto;

namespace XavalorAdventuresAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]

    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public EventsController(IMapper mapper, IEventService eventService)
        {
            this._mapper = mapper;
            _eventService = eventService;
        }


        //Phan cua kiet
        [HttpGet(Name = "GetEventList")]

        public async Task<ActionResult<ServiceResponse<GetEventDto>>> GetEventList()
        {
            try
            {
                var res = await _eventService.GetEvent();
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEventById(Guid id)
        {
            var eventDetail = await _eventService.GetEventById(id);
            return Ok(eventDetail);
        }

        [Authorize]

        [HttpPost("event", Name = "CreateNewEvent")]

        public async Task<ActionResult<ServiceResponse<GetEventDto>>> CreateNewEvent( CreateEventDto eventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var res = await _eventService.CreateNewEvent(eventDto);
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
        [Authorize]

        [HttpPut("{id}")]

        public async Task<ActionResult<ServiceResponse<GetEventDto>>> UpdateNewEvent(Guid id, [FromBody] UpdateEventDto eventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _eventService.UpdateEvent(id, eventDto);
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
        [HttpGet("event/pagination", Name = "GetEventListWithPagination")]

        public async Task<ActionResult<ServiceResponse<EventDto>>> GetLocationListWithPage([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var pagedsResult = await _eventService.GetEventWithPage(queryParameters);
                return Ok(pagedsResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("events/time/{schoolId}", Name = "GetEventListWithTime")]

        public async Task<ActionResult<ServiceResponse<GetTaskAndEventDto>>> GetEventListWithTimeNow(Guid schoolId)
        {
            try
            {
                var events = await _eventService.GetTaskAndEventListByTimeNow(schoolId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [Authorize]

        [HttpPost("upload-excel-event")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var serviceResponse = await _eventService.ImportDataFromExcel(file);

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
        [Authorize]

        [HttpDelete("disableevent")]
        public async Task<ActionResult<ServiceResponse<GetTaskAndEventDto>>> DisableStatusEvent(Guid id)
        {
            try
            {
                var disableEvent = await _eventService.DisableEvent(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        [Authorize]

        [HttpGet("excel-template-event")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var serviceResponse = await _eventService.DownloadExcelTemplate();

            if (serviceResponse.Success)
            {
                // Trả về file Excel dưới dạng response
                return new FileContentResult(serviceResponse.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "SampleDataEvent.xlsx"
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
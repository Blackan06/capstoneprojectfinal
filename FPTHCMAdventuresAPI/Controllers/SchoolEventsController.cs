using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.SchoolEventService;
using DataAccess.Dtos.SchoolEventDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.PlayerDto;
using System.Collections.Generic;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SchoolEventsController : ControllerBase
    {
        private readonly ISchoolEventService _schoolEventService;
        private readonly IMapper _mapper;

        public SchoolEventsController(IMapper mapper, ISchoolEventService schoolEventService)
        {
            this._mapper = mapper;
            _schoolEventService = schoolEventService;
        }


        [HttpGet(Name = "GetSchoolEvent")]

        public async Task<ActionResult<ServiceResponse<SchoolEventDto>>> GetSchoolEventList()
        {
            try
            {
                var res = await _schoolEventService.GetSchoolEvent();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SchoolEventDto>> GetSchoolEventById(Guid id)
        {
            var eventDetail = await _schoolEventService.GetSchoolEventById(id);
            return Ok(eventDetail);
        }

        [HttpPost("schoolevent", Name = "CreateNewSchoolEvent")]

        public async Task<ActionResult<ServiceResponse<GetSchoolEventDto>>> CreateNewSchoolEvent( CreateSchoolEventDto createSchoolEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _schoolEventService.CreateNewSchoolEvent(createSchoolEventDto);
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
      /*  [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<GetSchoolEventDto>>> UpdateSchoolEvent(Guid id, [FromBody] UpdateSchoolEventDto updateSchoolEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _schoolEventService.UpdateSchoolEvent(id, updateSchoolEventDto);
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
        }*/
        [HttpPut]
        [Route("Schooleventupdate/{id}")]
        public async Task<ActionResult<ServiceResponse<GetSchoolEventDto>>> UpdateSchoolEventStartTimeAndEndTime(Guid id, [FromBody] UpdateSchoolEventDto updateSchoolEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _schoolEventService.UpdateSchoolEventByStartTimeAndEndTime(id, updateSchoolEventDto);
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
        [HttpGet("GetSchoolByEventId/{eventId}")]

        public async Task<ActionResult<ServiceResponse<GetSchoolByEventIdDto>>> GetSchoolByEventId(Guid eventId)
        {
            try
            {
                var res = await _schoolEventService.GetSchoolByEventId(eventId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpDelete("deleteschoolevent")]
        public async Task<ActionResult<ServiceResponse<ItemDto>>> DeleteSchoolEvent(Guid id)
        {
            try
            {
                var disableEvent = await _schoolEventService.DeleteSchoolEvent(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        [HttpPost("createlistschoolevent")]
        public async Task<IActionResult> CreateListEventTask([FromBody] CreateListSchoolEvent createListschoolEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _schoolEventService.CreateNewSchoolEventList(createListschoolEvent);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}

using AutoMapper;
using DataAccess.Dtos.EventDto;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service;
using Service.Services.EventService;
using Service.Services.EventTaskService;
using System.Threading.Tasks;
using System;
using DataAccess.Dtos.EventTaskDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.TaskDto;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class EventTasksController : ControllerBase
    {
        private readonly IEventTaskService _eventTaskService;
        private readonly IMapper _mapper;

        public EventTasksController(IMapper mapper,IEventTaskService eventTaskService)
        {
            this._mapper = mapper;
            _eventTaskService = eventTaskService;
        }


        [HttpGet(Name = "GetEventTaskList")]

        public async Task<ActionResult<ServiceResponse<EventTaskDto>>> GetEventTaskList()
        {
            try
            {
                
                var res = await _eventTaskService.GetEventTask();

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EventTaskDto>> GetEventTaskById(Guid id)
        {
            var eventDetail = await _eventTaskService.GetEventById(id);
            return Ok(eventDetail);
        }

        [HttpGet("eventtask/{taskId}")]
        public async Task<ActionResult<EventTaskDto>> GetEventTaskByTaskId(Guid taskId)
        {
            var eventDetail = await _eventTaskService.GetEventTaskByTaskId(taskId);
            return Ok(eventDetail);
        }
        [HttpPost("createlisteventtask")]
        public async Task<IActionResult> CreateListEventTask([FromBody] CreateListEventTaskDto createListEventTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _eventTaskService.CreateNewEventTasks(createListEventTask);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost("eventtask", Name = "CreateNewEventTask")]

        public async Task<ActionResult<ServiceResponse<GetEventTaskDto>>> CreateNewEventTask( CreateEventTaskDto createEventTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _eventTaskService.CreateNewEventTask(createEventTaskDto);
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

        public async Task<ActionResult<ServiceResponse<GetEventTaskDto>>> UpdateEvent(Guid id, [FromBody] UpdateEventTaskDto updateEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _eventTaskService.UpdateTaskEvent(id ,updateEventDto);
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
      

        [HttpGet("GetTaskByEventTaskWithEventId/{eventId}")]

        public async Task<ActionResult<ServiceResponse<GetTaskByEventIdDto>>> GetTaskByEventTaskWithEventId(Guid eventId)
        {
            try
            {
                var res = await _eventTaskService.GetTaskByEventTaskWithEventId(eventId);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("deleteeventtask")]
        public async Task<ActionResult<ServiceResponse<ItemDto>>> DeleteEventTask(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
               
                var disableEvent = await _eventTaskService.DeleteEventTask(id);
                if (!disableEvent.Success)
                {
                    return BadRequest(disableEvent);
                }
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}

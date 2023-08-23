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
using AutoMapper;
using Service.Services.EventService;
using Service.Services.TaskService;
using DataAccess.Dtos.EventDto;
using Service;
using DataAccess.Dtos.TaskDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TasksController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }


        [HttpGet(Name = "GetTaskList")]

        public async Task<ActionResult<ServiceResponse<TaskDto>>> GetTaskList()
        {
            try
            {
                var res = await _taskService.GetTask();
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<GetTaskDto>> GetTaskById(Guid id)
        {
            var eventDetail = await _taskService.GetTaskById(id);
            return Ok(eventDetail);
        }
       /* [HttpGet("/gettaskdonebymajor")]
        public async Task<ActionResult<ServiceResponse<TaskDto>>> GetTaskDoneByMajor(Guid majorid)
        {
            try
            {
                var res = await _taskService.GetTaskDoneByMajor(majorid);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }*/

        [HttpPost("task", Name = "CreateNewTask")]

        public async Task<ActionResult<ServiceResponse<GetTaskDto>>> CreateNewTask(CreateTaskDto taskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _taskService.CreateNewTask(taskDto);
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

        public async Task<ActionResult<ServiceResponse<GetTaskDto>>> UpdateNewTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _taskService.UpdateTask(id, updateTaskDto);
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

        [HttpDelete("disabletask")]
        public async Task<ActionResult<ServiceResponse<TaskDto>>> DisableStatusTask(Guid id)
        {
            try
            {
                var disableEvent = await _taskService.DisableTask(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}
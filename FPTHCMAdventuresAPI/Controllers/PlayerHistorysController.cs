using AutoMapper;
using DataAccess.Dtos.ItemDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.ItemService;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.PlayerHistoryService;
using DataAccess.Dtos.PlayerHistoryDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.TaskDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PlayerHistorysController : ControllerBase
    {
        private readonly IPlayerHistoryService _playerHistoryService;
        private readonly IMapper _mapper;

        public PlayerHistorysController(IMapper mapper, IPlayerHistoryService playerHistoryService)
        {
            this._mapper = mapper;
            _playerHistoryService = playerHistoryService;
        }


        [HttpGet(Name = "GetPlayerHistory")]

        public async Task<ActionResult<ServiceResponse<PlayerHistoryDto>>> GetPlayerHistoryList()
        {
            try
            {
                var res = await _playerHistoryService.GetPlayerHistory();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerHistoryDto>> GetItemById(Guid id)
        {
            var eventDetail = await _playerHistoryService.GetPlayerHistoryById(id);
            return Ok(eventDetail);
        }
        [HttpGet("task/{EventTaskId}")]
        public async Task<ActionResult<PlayerHistoryDto>> GetPlayerHistoryByEventTaskId(Guid EventTaskId)
        {
            var eventDetail = await _playerHistoryService.GetPlayerHistoryByEventTaskId(EventTaskId);
            return Ok(eventDetail);
        } 
        [HttpGet("task/{taskId}/{playerId}")]
        public async Task<ActionResult<PlayerHistoryDto>> GetPlayerHistoryWithEventTaskIdAndPlayerId(Guid taskId, Guid playerId)
        {
            var eventDetail = await _playerHistoryService.GetPlayerHistoryByEventTaskIdAndPlayerId(taskId, playerId);
            return Ok(eventDetail);
        }

        [HttpPost("playerhistory", Name = "CreateNewPlayerHistory")]

        public async Task<ActionResult<ServiceResponse<GetPlayerHistoryDto>>> CreateNewPlayerHistory(CreatePlayerHistoryDto createPlayerHistoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _playerHistoryService.CreateNewPlayerHistory(createPlayerHistoryDto);
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

        public async Task<ActionResult<ServiceResponse<GetPlayerHistoryDto>>> UpdatePlayerHistory(Guid id, [FromBody] UpdatePlayerHistoryDto updatePlayerHistoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _playerHistoryService.UpdatePlayerHistory(id, updatePlayerHistoryDto);
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
        [HttpDelete("disableplayerhistory")]
        public async Task<ActionResult<ServiceResponse<PlayerHistoryDto>>> DisableStatusPlayerHistory(Guid id)
        {
            try
            {
                var disableEvent = await _playerHistoryService.DisablePlayerHistory(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.PlayerService;
using DataAccess.Dtos.PlayerDto;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using DataAccess.Dtos.StudentDto;

namespace FPTHCMAdventuresAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;

        public PlayersController(IMapper mapper, IPlayerService playerService)
        {
            this._mapper = mapper;
            _playerService = playerService;
        }


        [HttpGet(Name = "GetPlayer")]

        public async Task<ActionResult<ServiceResponse<PlayerDto>>> GetPlayerList()
        {
            try
            {
                var res = await _playerService.GetPlayer();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPlayerWithNickName")]

        public async Task<ActionResult<ServiceResponse<PlayerDto>>> GetPlayerListWithNickName()
        {
            try
            {
                var res = await _playerService.GetPlayerWithNickName();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet]
        [Route("filterdatawithschoolandevent")]
        public async Task<ActionResult<GetPlayerWithSchoolAndEvent>> FilterDataBySchoolIdAndEventId(Guid? schoolId, Guid? eventId)
        {
            var eventDetail = await _playerService.filterData(schoolId, eventId);
            return Ok(eventDetail);
        }
        /*   [HttpGet("players/listPlayer-username", Name = "GetPlayerWithUserNames")]

           public async Task<ActionResult<ServiceResponse<GetPlayerDto>>> GetPlayerListWithUserName()
           {
               try
               {
                   var res = await _playerService.GetPlayerWithUserName();
                   return Ok(res);
               }
               catch (Exception ex)
               {
                   return StatusCode(500, "Internal server error: " + ex.Message);
               }
           }*/
       
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlayerDto>> GetPlayerById(Guid id)
        {
            var eventDetail = await _playerService.GetPlayerById(id);
            return Ok(eventDetail);
        } 
        [HttpGet("playerschool/{playerId}")]
        public async Task<ActionResult<GetPlayerDto>> GetSchoolByPlayerId(Guid playerId)
        {
            var eventDetail = await _playerService.GetSchoolByPlayerId(playerId);
            return Ok(eventDetail);
        }

        [HttpGet("user/{studentId}")]
        public async Task<ActionResult<GetPlayerDto>> GetPlayerByStudentId(Guid studentId)
        {
            var eventDetail = await _playerService.GetPlayerByStudentId(studentId);
            return Ok(eventDetail);
        }
        [HttpGet("player/{nickname}")]
        public async Task<ActionResult<GetPlayerDto>> GetPlayerByNickName(string nickname)
        {
            var eventDetail = await _playerService.CheckPlayerByNickName(nickname);
            return Ok(eventDetail);
        }
        /*  [HttpGet("player/{username}")]
          public async Task<ActionResult<PlayerDto>> GetPlayerByUserName(string username)
          {
              var eventDetail = await _playerService.CheckPlayerByUserName(username);
              return Ok(eventDetail);
          }*/
        [HttpPost("createlistplayer")]
        public async Task<IActionResult> CreatePlayers([FromBody]CreateListPlayerDto players)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _playerService.CreateNewPlayers(players);
            return Ok(response);
        }
        [Authorize]

        [HttpPost("player", Name = "CreateNewPlayer")]

        public async Task<ActionResult<ServiceResponse<GetPlayerDto>>> CreateNewPlayer( CreatePlayerDto createPlayerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _playerService.CreateNewPlayer(createPlayerDto);
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

        public async Task<ActionResult<ServiceResponse<GetPlayerDto>>> UpdatePlayer(Guid id, [FromBody] UpdatePlayerDto updatePlayerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _playerService.UpdatePlayer(id, updatePlayerDto);
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
    }
}
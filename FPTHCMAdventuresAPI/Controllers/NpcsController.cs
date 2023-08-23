using AutoMapper;
using DataAccess.Dtos.LocationDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.LocationServoce;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.NpcService;
using DataAccess.Dtos.NPCDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class NpcsController : ControllerBase
    {
        private readonly INpcService _npcService;
        private readonly IMapper _mapper;

        public NpcsController(IMapper mapper, INpcService npcService)
        {
            this._mapper = mapper;
            _npcService = npcService;
        }


        [HttpGet(Name = "GetNpcList")]

        public async Task<ActionResult<ServiceResponse<GetNpcDto>>> GetNpcList()
        {
            try
            {
                var res = await _npcService.GetNpc();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NpcDto>> GetNpcById(Guid id)
        {
            var eventDetail = await _npcService.GetNpcById(id);
            return Ok(eventDetail);
        }

        [HttpPost("npc", Name = "CreateNpc")]

        public async Task<ActionResult<ServiceResponse<GetNpcDto>>> CreateNpc( CreateNpcDto createNpcDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _npcService.CreateNewNpc(createNpcDto);
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

        public async Task<ActionResult<ServiceResponse<GetNpcDto>>> UpdateNpc(Guid id, [FromForm] UpdateNpcDto updateNpcDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _npcService.UpdateNpc(id, updateNpcDto);
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

        [HttpDelete("disablenpc")]
        public async Task<ActionResult<ServiceResponse<NpcDto>>> DisableStatusMajor(Guid id)
        {
            try
            {
                var disableEvent = await _npcService.DisableNpc(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}



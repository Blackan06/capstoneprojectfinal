using AutoMapper;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.ExchangeHistoryDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.ExchangeHistoryService;
using Service;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Service.Services.PrizeService;
using DataAccess.Dtos.PrizeDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PrizesController : ControllerBase
    {
        private readonly IPrizeService _prizeService;
        private readonly IMapper _mapper;

        public PrizesController(IMapper mapper, IPrizeService prizeService)
        {
            this._mapper = mapper;
            _prizeService = prizeService;
        }


        [HttpGet(Name = "GetGift")]

        public async Task<ActionResult<ServiceResponse<GetPrizeDto>>> GetPrizeList()
        {
            try
            {
                var res = await _prizeService.GetPrize();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("GetTotalGift")]

        public async Task<ActionResult<ServiceResponse<string>>> GetTotalPrize()
        {
            try
            {
                var res = await _prizeService.GetTotalPrize();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrizeDto>> GetGiftById(Guid id)
        {
            var eventDetail = await _prizeService.GetPrizeById(id);
            return Ok(eventDetail);
        }

        [HttpPost("Gift", Name = "CreateNewGift")]

        public async Task<ActionResult<ServiceResponse<GetPrizeDto>>> CreateNewGift( CreatePrizeDto createPrizeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _prizeService.CreateNewPrize(createPrizeDto);
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

        public async Task<ActionResult<ServiceResponse<GetPrizeDto>>> UpdateGift(Guid id, [FromBody] UpdatePrizeDto updatePrizeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _prizeService.UpdatePrize(id, updatePrizeDto);
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

        [HttpDelete("disablegift")]
        public async Task<ActionResult<ServiceResponse<PrizeDto>>> DisableStatusGift(Guid id)
        {
            try
            {
                var disableEvent = await _prizeService.DisablePrize(id);
                return Ok(disableEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
    }
}

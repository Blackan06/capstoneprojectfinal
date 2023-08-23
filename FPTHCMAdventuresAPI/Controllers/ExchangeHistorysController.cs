using AutoMapper;
using DataAccess.Dtos.AnswerDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.AnswerService;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.ExchangeHistoryService;
using DataAccess.Dtos.ExchangeHistoryDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Dtos.EventTaskDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ExchangeHistorysController : ControllerBase
    {
        private readonly IExchangeHistoryService _exchangeHistoryService;
        private readonly IMapper _mapper;

        public ExchangeHistorysController(IMapper mapper, IExchangeHistoryService exchangeHistoryService)
        {
            this._mapper = mapper;
            _exchangeHistoryService = exchangeHistoryService;
        }

        [HttpGet("exchangehistory/byname/{itemName}")]
        public async Task<ActionResult<ItemInventoryDto>> GetItemInventoryByItemName(string itemName)
        {
            var eventDetail = await _exchangeHistoryService.GetExchangeByItemName(itemName);
            return Ok(eventDetail);
        }
        [HttpGet(Name = "GetExchangHistory")]

        public async Task<ActionResult<ServiceResponse<ExchangeHistoryDto>>> GetExchangeHistoryList()
        {
            try
            {
                var res = await _exchangeHistoryService.GetExchangeHistory();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetExchangeHistoryDto>> GetExchangeHistoryById(Guid id)
        {
            var eventDetail = await _exchangeHistoryService.GetExchangeHistoryById(id);
            return Ok(eventDetail);
        }

        [HttpPost("exchangeHistory", Name = "CreateNewExchangeHistory")]

        public async Task<ActionResult<ServiceResponse<GetExchangeHistoryDto>>> CreateNewExchangeHistory( CreateExchangeHistoryDto createExchangeHistoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var res = await _exchangeHistoryService.CreateNewExchangeHistory(createExchangeHistoryDto);
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

        public async Task<ActionResult<ServiceResponse<GetExchangeHistoryDto>>> UpdateExchangeHistory(Guid id, [FromBody] UpdateExchangeHistoryDto updateExchangeHistoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _exchangeHistoryService.UpdateExchangeHistory(id, updateExchangeHistoryDto);
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

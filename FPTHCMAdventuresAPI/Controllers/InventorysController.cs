using AutoMapper;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.ExchangeHistoryDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.InventoryService;
using DataAccess.Dtos.InventoryDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.EventDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class InventorysController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;

        public InventorysController(IMapper mapper, IInventoryService inventoryService)
        {
            this._mapper = mapper;
            _inventoryService = inventoryService;
        }


        [HttpGet(Name = "GetInventoryList")]

        public async Task<ActionResult<ServiceResponse<InventoryDto>>> GetInventoryList()
        {
            try
            {
                var res = await _inventoryService.GetInventory();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetInventoryById(Guid id)
        {
            var eventDetail = await _inventoryService.GetInventoryById(id);
            return Ok(eventDetail);
        }

        [HttpPost("inventory", Name = "CreateNewInventory")]

        public async Task<ActionResult<ServiceResponse<GetInventoryDto>>> CreateNewInventory(CreateInventoryDto createInventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _inventoryService.CreateNewInventory(createInventoryDto);
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

        public async Task<ActionResult<ServiceResponse<GetInventoryDto>>> UpdateInventory(Guid id, [FromBody] UpdateInventoryDto updateInventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _inventoryService.UpdateInventory(id, updateInventoryDto);
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

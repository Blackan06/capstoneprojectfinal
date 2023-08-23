using AutoMapper;
using DataAccess.Dtos.InventoryDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.InventoryService;
using Service;
using System.Threading.Tasks;
using System;
using Service.Services.ItemInventoryService;
using DataAccess.Dtos.ItemInventoryDto;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Dtos.EventDto;

namespace FPTHCMAdventuresAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ItemInventorysController : ControllerBase
    {
        private readonly IItemInventoryService _itemInventoryService;
        private readonly IMapper _mapper;

        public ItemInventorysController(IMapper mapper, IItemInventoryService itemInventoryService)
        {
            this._mapper = mapper;
            _itemInventoryService = itemInventoryService;
        }


        [HttpGet(Name = "GetItemInventory")]

        public async Task<ActionResult<ServiceResponse<ItemInventoryDto>>> GetItemInventoryList()
        {
            try
            {
                var res = await _itemInventoryService.GetItemInventory();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemInventoryDto>> GetItemInventoryById(Guid id)
        {
            var eventDetail = await _itemInventoryService.GetItemInventoryById(id);
            return Ok(eventDetail);
        }
        [HttpGet("iteminventory/{PlayerNickName}")]
        public async Task<ActionResult<ItemInventoryDto>> GetItemInventoryByPlayerId(string PlayerNickName)
        {
            var eventDetail = await _itemInventoryService.getListItemInventoryByPlayer(PlayerNickName);
            return Ok(eventDetail);
        }
        [HttpGet("iteminventory/byname/{itemName}")]
        public async Task<ActionResult<ItemInventoryDto>> GetItemInventoryByItemName(string itemName)
        {
            var eventDetail = await _itemInventoryService.GetItemByItemName(itemName);
            return Ok(eventDetail);
        }
        [HttpPost("itemInventory", Name = "CreateNewItemInventory")]

        public async Task<ActionResult<ServiceResponse<GetItemInventoryDto>>> CreateNewItemInventory(CreateItemInventoryDto createItemInventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _itemInventoryService.CreateNewItemInventory(createItemInventoryDto);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPut("{id}")]

        public async Task<ActionResult<ServiceResponse<GetItemInventoryDto>>> UpdateItemInventory(Guid id, [FromBody] UpdateItemInventoryDto updateItemInventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _itemInventoryService.UpdateItemInventory(id, updateItemInventoryDto);
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
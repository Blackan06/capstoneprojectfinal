using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.InventoryDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Repositories.InventoryRepositories;
using DataAccess.Repositories.ItemInventoryRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ItemInventoryService
{
    public class ItemInventoryService : IItemInventoryService
    {
        private readonly IItemInventoryRepositories _itemInventoryRepository;
        private readonly IMapper _mapper;
       
        public ItemInventoryService(IItemInventoryRepositories itemInventoryRepository, IMapper mapper)
        {
            _itemInventoryRepository = itemInventoryRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewItemInventory(CreateItemInventoryDto createItemInventoryDto)
        {
            createItemInventoryDto.CreatedAt = TimeZoneVietName(createItemInventoryDto.CreatedAt);
            var itemInventoryCreate = _mapper.Map<ItemInventory>(createItemInventoryDto);
            itemInventoryCreate.Id = Guid.NewGuid();
            await _itemInventoryRepository.AddAsync(itemInventoryCreate);

            return new ServiceResponse<Guid>
            {
                Data = itemInventoryCreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<ItemInventoryDto>> GetItemByItemName(string itemName)
        {
            var item = await _itemInventoryRepository.getItemByItemName(itemName);
            if(item == null)
            {
                return new ServiceResponse<ItemInventoryDto>
                {
                    Message = "No rows",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                return new ServiceResponse<ItemInventoryDto>
                {
                    Data = item,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetItemInventoryDto>>> GetItemInventory()
        {
            var itemInventorylist = await _itemInventoryRepository.GetAllAsync<GetItemInventoryDto>();

            if (itemInventorylist != null)
            {
                itemInventorylist = itemInventorylist.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetItemInventoryDto>>
                {
                    Data = itemInventorylist,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetItemInventoryDto>>
                {
                    Data = itemInventorylist,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<ItemInventoryDto>> GetItemInventoryById(Guid eventId)
        {
            try
            {

                var eventDetail = await _itemInventoryRepository.GetAsync<ItemInventoryDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<ItemInventoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<ItemInventoryDto>
                {
                    Data = eventDetail,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceResponse<GetListItemInventoryByPlayer>> getListItemInventoryByPlayer(string PlayerNickName)
        {
            try
            {

                var eventDetail = await _itemInventoryRepository.GetListItemInventoryByPlayer(PlayerNickName);

                if (eventDetail == null)
                {

                    return new ServiceResponse<GetListItemInventoryByPlayer>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetListItemInventoryByPlayer>
                {
                    Data = eventDetail,
                    Message = "Successfully",
                    StatusCode = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        private DateTime TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            return dateTime;
        }
        public async Task<ServiceResponse<bool>> UpdateItemInventory(Guid id, UpdateItemInventoryDto ItemInventoryDto)
        {
            var existingItemInventory = await _itemInventoryRepository.GetById(id);

            if (existingItemInventory == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Inventory not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {
                existingItemInventory.Quantity = ItemInventoryDto.Quantity;
                await _itemInventoryRepository.UpdateAsync(existingItemInventory);
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success edit",
                    Success = true,
                    StatusCode = 202
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventTaskExists(id))
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Message = "Invalid Record Id",
                        Success = false,
                        StatusCode = 500
                    };
                }
                else
                {
                    throw;
                }
            }
        }
        private async Task<bool> EventTaskExists(Guid id)
        {
            return await _itemInventoryRepository.Exists(id);
        }
       

       
    }
}

using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.InventoryDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Repositories.InventoryRepositories;
using DataAccess.Repositories.ItemInventoryRepositories;
using DataAccess.Repositories.ItemRepositories;
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
            var checkExist = await _itemInventoryRepository.GetByItemId(createItemInventoryDto.ItemId, createItemInventoryDto.InventoryId);
            if (checkExist == null)
            {
                createItemInventoryDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
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
            else
            {
                checkExist.Quantity += 1;
                await _itemInventoryRepository.UpdateAsync(checkExist.Id, checkExist);
                return new ServiceResponse<Guid>
                {
                    Message = "Successfully",
                    Success = true,
                    StatusCode = 201
                };
            }



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

        public async Task<ServiceResponse<GetListItemInventoryByPlayer>> GetListItemInventoryByPlayer(string PlayerNickName)
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
                existingItemInventory.ItemId = ItemInventoryDto.ItemId;
                existingItemInventory.Quantity = ItemInventoryDto.Quantity;
                await _itemInventoryRepository.UpdateAsync(id, existingItemInventory);
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

        public async Task<ServiceResponse<bool>> UpdateItemInventoryByPlayer1ToPlayer2(string playerNickName1, string playerNickName2, Guid itemId) 
        {
            var existingItemInventory = await _itemInventoryRepository.GetItemInventoryByPlayer(playerNickName1, itemId);
            var existingItemInventory2 = await _itemInventoryRepository.GetItemInventoryByPlayer(playerNickName2, itemId);

            if (existingItemInventory == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Item in inventory 1 not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            else
            {
                if (existingItemInventory.Quantity > 1)
                {
                    existingItemInventory.Quantity -= 1;
                    await _itemInventoryRepository.UpdateAsync(existingItemInventory.Id, existingItemInventory);
                    var updateItemInventoryDto = _mapper.Map<UpdateItemInventoryDto>(existingItemInventory);
                    var checkUpdate = await UpdateItemInventory(existingItemInventory.Id, updateItemInventoryDto);
                    if (checkUpdate.Data) 
                    {
                        if (existingItemInventory2 == null)
                        {
                            var inventoryId = await _itemInventoryRepository.GetItemInventoryByPlayerNotItem(playerNickName2);
                            var createItemInventory = new CreateItemInventoryDto
                            {
                                InventoryId = inventoryId.Id,
                                ItemId = itemId,
                                Quantity = 1,
                            };
                            await CreateNewItemInventory(createItemInventory);
                            return new ServiceResponse<bool>
                            {
                                Data = true,
                                Message = "Success Add New Item",
                                Success = true,
                                StatusCode = 202
                            };
                        }
                        else
                        {
                            existingItemInventory2.Quantity += 1;
                            var updateItemInventoryDto2 = _mapper.Map<UpdateItemInventoryDto>(existingItemInventory2);

                            var check = await UpdateItemInventory(existingItemInventory2.Id, updateItemInventoryDto2);

                            if (check.Data)
                            {
                                return new ServiceResponse<bool>
                                {
                                    Data = true,
                                    Message = "Success edit a 2",
                                    Success = true,
                                    StatusCode = 202
                                };
                            }
                            else
                            {
                                return new ServiceResponse<bool>
                                {
                                    Data = false,
                                    Message = "failed edit a 2",
                                    Success = false,
                                    StatusCode = 400
                                };
                            }
                        }
                    }
                    else
                    {
                        return new ServiceResponse<bool>
                        {
                            Data = false,
                            Message = "failed edit",
                            Success = false,
                            StatusCode = 400
                        };
                    }
                    
                }
                else
                {
                    await _itemInventoryRepository.DeleteAsync(existingItemInventory.Id);
                    if (existingItemInventory2 == null)
                    {
                        var inventoryId = await _itemInventoryRepository.GetItemInventoryByPlayerNotItem(playerNickName2);

                        var createItemInventory = new CreateItemInventoryDto
                        {
                            InventoryId = inventoryId.Id,
                            ItemId = itemId,
                            Quantity = 1,
                        };
                        await CreateNewItemInventory(createItemInventory);
                        return new ServiceResponse<bool>
                        {
                            Data = true,
                            Message = "Success Add New Item",
                            Success = true,
                            StatusCode = 202
                        };
                    }
                    else
                    {
                        existingItemInventory2.Quantity += 1;
                        var updateItemInventoryDto2 = _mapper.Map<UpdateItemInventoryDto>(existingItemInventory2);

                        var check = await UpdateItemInventory(existingItemInventory2.Id, updateItemInventoryDto2);

                        if (check.Data)
                        {
                            return new ServiceResponse<bool>
                            {
                                Data = true,
                                Message = "Success edit b 2",
                                Success = true,
                                StatusCode = 202
                            };
                        }
                        else
                        {
                            return new ServiceResponse<bool>
                            {
                                Data = false,
                                Message = "failed edit b 2",
                                Success = false,
                                StatusCode = 400
                            };
                        }
                    }
                }
               

            }
           
        }
           
        
    }
}

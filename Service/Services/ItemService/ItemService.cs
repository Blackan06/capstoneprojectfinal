using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.ImageUploadDto;
using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Repositories.ImageRepository;
using DataAccess.Repositories.ItemInventoryRepositories;
using DataAccess.Repositories.ItemRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ItemService
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
      
        public ItemService(IItemRepository itemRepository, IMapper mapper, IImageRepository imageRepository)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
        }
        public async Task<ServiceResponse<Guid>> CreateNewItem(CreateItemDto createItemDto)
        {
            if (await _itemRepository.ExistsAsync(s => s.Name == createItemDto.Name ))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Item with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _itemRepository.ExistsAsync(s => s.ImageUrl == createItemDto.ImageUrl))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Item with the same ImageUrl already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createItemDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            createItemDto.Name = createItemDto.Name.Trim();
            createItemDto.Description = createItemDto.Description.Trim();
            createItemDto.Type = createItemDto.Type.Trim();
            createItemDto.Status = "ACTIVE";
            var itemCreate = _mapper.Map<Item>(createItemDto);
            itemCreate.Id = Guid.NewGuid();
          
           

            // Tải lên hình ảnh và lưu URL
            string uploadedImageUrl = await _imageRepository.UploadImageAndReturnUrlAsync(createItemDto.Image);

            itemCreate.ImageUrl = uploadedImageUrl; // Gán URL của hình ảnh cho thuộc tính ImageUrl

            await _itemRepository.AddAsync(itemCreate);

            return new ServiceResponse<Guid>
            {
                Data = itemCreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
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
        public async Task<ServiceResponse<string>> DisableStausItem(Guid id)
        {
            var checkEvent = await _itemRepository.GetAsync<ItemDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<string>
                {
                    Data = "null",
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";
                var itemData = _mapper.Map<Item>(checkEvent);

                await _itemRepository.UpdateAsync(id, itemData);
                return new ServiceResponse<string>
                {
                    Data = checkEvent.Status,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ItemDto>>> GetItem()
        {
            var itemList = await _itemRepository.GetAllAsync<ItemDto>();

            if (itemList != null)
            {
                itemList = itemList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<ItemDto>>
                {
                    Data = itemList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<ItemDto>>
                {
                    Data = itemList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<ItemDto>> GetItemById(Guid eventId)
        {
            try
            {

                var eventDetail = await _itemRepository.GetAsync<ItemDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<ItemDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<ItemDto>
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

        public async Task<ServiceResponse<bool>> UpdateItem(Guid id, UpdateItemDto updateItemDto)
        {
            if (await _itemRepository.ExistsAsync(s => s.Name == updateItemDto.Name && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Item with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _itemRepository.ExistsAsync(s => s.ImageUrl == updateItemDto.ImageUrl && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Item with the same ImageUrl already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingItem = await _itemRepository.GetById(id);
            if (existingItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Item not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {
                
                existingItem.Name = updateItemDto.Name.Trim();
                existingItem.Description = updateItemDto.Description.Trim();
                existingItem.Type = updateItemDto.Type.Trim();
                existingItem.Status = updateItemDto.Status.Trim();


                if (updateItemDto.NewImage != null)
                {
                    string uploadedImageUrl = await _imageRepository.UploadImageAndReturnUrlAsync(updateItemDto.NewImage);

                    if (uploadedImageUrl == null)
                    {
                        return new ServiceResponse<bool>
                        {
                            Message = "Failed to upload image.",
                            Success = false,
                            StatusCode = 500
                        };
                    }

                    existingItem.ImageUrl = uploadedImageUrl;
                }

                await _itemRepository.UpdateAsync(id, existingItem);
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
            return await _itemRepository.Exists(id);
        }
       
    }
}

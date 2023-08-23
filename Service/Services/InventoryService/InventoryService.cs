using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.InventoryDto;
using DataAccess.Repositories.InventoryRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewInventory(CreateInventoryDto createInventoryDto)
        {
            if (await _inventoryRepository.ExistsAsync(inv => inv.PlayerId == createInventoryDto.PlayerId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "An Inventory with the same playerId already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var mapper = config.CreateMapper();
            TimeZoneVietName(createInventoryDto.CreatedAt);

            var inventoryCreate = mapper.Map<Inventory>(createInventoryDto);
            inventoryCreate.Id = Guid.NewGuid();
            await _inventoryRepository.AddAsync(inventoryCreate);

            return new ServiceResponse<Guid>
            {
                Data = inventoryCreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }
        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
        }
        public async Task<ServiceResponse<IEnumerable<GetInventoryDto>>> GetInventory()
        {
            var inventoryList = await _inventoryRepository.GetAllAsync<GetInventoryDto>();

            if (inventoryList != null)
            {
                inventoryList = inventoryList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetInventoryDto>>
                {
                    Data = inventoryList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetInventoryDto>>
                {
                    Data = inventoryList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<InventoryDto>> GetInventoryById(Guid eventId)
        {
            try
            {

                var eventDetail = await _inventoryRepository.GetAsync<InventoryDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<InventoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<InventoryDto>
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

        public async Task<ServiceResponse<bool>> UpdateInventory(Guid id, UpdateInventoryDto updateInventoryDto)
        {
            if (await _inventoryRepository.ExistsAsync(inv => inv.PlayerId == updateInventoryDto.PlayerId))
            {
                return new ServiceResponse<bool>
                {
                    Message = "An Inventory with the same playerId already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingInventory = await _inventoryRepository.GetById(id);

            if (existingInventory == null)
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
                await _inventoryRepository.UpdateAsync(id, updateInventoryDto);
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
            return await _inventoryRepository.Exists(id);
        }

    }
}

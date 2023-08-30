using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.AnswerDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.LocationDto;
using DataAccess.Repositories.ExchangeHistoryRepositories;
using DataAccess.Repositories.LocationRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ExchangeHistoryService
{
    public class ExchangHistoryService : IExchangeHistoryService
    {
        private readonly IExchangeHistoryRepository _exchangeHistoryRepository;
        private readonly IMapper _mapper;
     
        public ExchangHistoryService(IExchangeHistoryRepository exchangeHistoryRepository, IMapper mapper)
        {
            _exchangeHistoryRepository = exchangeHistoryRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewExchangeHistory(CreateExchangeHistoryDto createExchangeHistoryDto)
        {
            createExchangeHistoryDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow); 
            createExchangeHistoryDto.ExchangeDate = TimeZoneVietName(DateTime.UtcNow);
            createExchangeHistoryDto.Status = "SUCCESS";
            var exchangeHistoryCreate = _mapper.Map<ExchangeHistory>(createExchangeHistoryDto);
            exchangeHistoryCreate.Id = Guid.NewGuid();
           
            await _exchangeHistoryRepository.AddAsync(exchangeHistoryCreate);

            return new ServiceResponse<Guid>
            {
                Data = exchangeHistoryCreate.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<ExchangeHistoryDto>> GetExchangeByItemName(string itemName)
        {
            var exchange = await _exchangeHistoryRepository.getExchangeByItemName(itemName);
           
            if(exchange != null)
            {
                return new ServiceResponse<ExchangeHistoryDto>
                {
                    Data = exchange,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<ExchangeHistoryDto>
                {
                    Data = exchange,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
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
        public async Task<ServiceResponse<IEnumerable<ExchangeHistoryDto>>> GetExchangeHistory()
        {
            var exchangeHistoryList = await _exchangeHistoryRepository.GetAllAsync<ExchangeHistoryDto>();

            if (exchangeHistoryList != null)
            {
                exchangeHistoryList = exchangeHistoryList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<ExchangeHistoryDto>>
                {
                    Data = exchangeHistoryList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<ExchangeHistoryDto>>
                {
                    Data = exchangeHistoryList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        

        public async Task<ServiceResponse<GetExchangeHistoryDto>> GetExchangeHistoryById(Guid eventId)
        {
            try
            {

                var eventDetail = await _exchangeHistoryRepository.GetAsync<GetExchangeHistoryDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<GetExchangeHistoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetExchangeHistoryDto>
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

        public async Task<ServiceResponse<bool>> UpdateExchangeHistory(Guid id, UpdateExchangeHistoryDto exchangeHistoryDto)
        {
            try
            {
                var existingExchangeHistory = await _exchangeHistoryRepository.GetById(id);

                if (existingExchangeHistory == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Message = "Event not found",
                        Success = false,
                        StatusCode = 404
                    };
                }
                existingExchangeHistory.PlayerId = exchangeHistoryDto.PlayerId;
                existingExchangeHistory.ItemId = exchangeHistoryDto.ItemId;
                existingExchangeHistory.Quantity = exchangeHistoryDto.Quantity;
                await _exchangeHistoryRepository.UpdateAsync(id, existingExchangeHistory);
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
            return await _exchangeHistoryRepository.Exists(id);
        }
    }
}

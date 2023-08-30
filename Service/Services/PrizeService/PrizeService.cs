using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.Dtos.PrizeDto;
using DataAccess.Repositories.PrizeRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.PrizeService
{
    public class PrizeService : IPrizeService
    {
        private readonly IPrizeRepository _prizeRepository;
        private readonly IMapper _mapper;
        
        public PrizeService(IPrizeRepository prizeRepository, IMapper mapper)
        {
            _prizeRepository = prizeRepository;
            _mapper = mapper;
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
        public async Task<ServiceResponse<Guid>> CreateNewPrize(CreatePrizeDto createPrizeDto)
        {
            if (await _prizeRepository.ExistsAsync(s => s.Name == createPrizeDto.Name))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Prize with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }

            createPrizeDto.Description = createPrizeDto.Description.Trim();
            createPrizeDto.Name = createPrizeDto.Name.Trim();
            createPrizeDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
            var createPrize = _mapper.Map<Prize>(createPrizeDto);
            createPrize.Id = Guid.NewGuid();
           
            await _prizeRepository.AddAsync(createPrize);

            return new ServiceResponse<Guid>
            {
                Data = createPrize.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<IEnumerable<PrizeDto>>> GetPrize()
        {
            var prizeList = await _prizeRepository.GetAllAsync<PrizeDto>();

            if (prizeList != null)
            {
                prizeList = prizeList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<PrizeDto>>
                {
                    Data = prizeList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<PrizeDto>>
                {
                    Data = prizeList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<string>> GetTotalPrize()
        {
            var context = new db_a9c31b_capstoneContext();
            try
            {
                long total = context.Prizes.Count();
                return new ServiceResponse<string>
                {
                    Data = total.ToString(),
                    Message = "Success!",
                    Success = true,
                    StatusCode = 202
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new ServiceResponse<string>
                {
                    Message = ex.ToString(),
                    Success = false,
                    StatusCode = 500
                };
            }

        }

        public async Task<ServiceResponse<PrizeDto>> GetPrizeById(Guid eventId)
        {
            try
            {

                var eventDetail = await _prizeRepository.GetAsync<PrizeDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<PrizeDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<PrizeDto>
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

        public async Task<ServiceResponse<bool>> UpdatePrize(Guid id, UpdatePrizeDto updatePrizeDto)
        {
            if (await _prizeRepository.ExistsAsync(s => s.Name == updatePrizeDto.Name && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Prize with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingPrize = await _prizeRepository.GetById(id);

            if (existingPrize == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Prize not found",
                    Success = false,
                    StatusCode = 404
                };
            }
            try
            {
                existingPrize.Quantity = updatePrizeDto.Quantity;
                existingPrize.Description = updatePrizeDto.Description.Trim();
                existingPrize.Name = updatePrizeDto.Name.Trim();
                existingPrize.Status = updatePrizeDto.Status.Trim();
                await _prizeRepository.UpdateAsync(id, existingPrize);
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
                if (!await PrizeExists(id))
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

       

        private async Task<bool> PrizeExists(Guid id)
        {
            return await _prizeRepository.Exists(id);
        }

        public async Task<ServiceResponse<bool>> DisablePrize(Guid id)
        {
            var checkEvent = await _prizeRepository.GetAsync<PrizeDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Failed",
                    StatusCode = 400,
                    Success = true
                };
            }
            else
            {
                checkEvent.Status = "INACTIVE";
               await _prizeRepository.UpdateAsync(id, checkEvent);
               return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
    }
}
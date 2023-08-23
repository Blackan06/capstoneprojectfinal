using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.Repositories.PlayerHistoryRepositories;
using DataAccess.Repositories.PlayerPrizeRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.PlayerPrizeService
{
    public class PlayerPrizeService : IPlayerPrizeService
    {
        private readonly IPlayerPrizeRepositories _playerPrizeRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public PlayerPrizeService(IPlayerPrizeRepositories playerPrizeRepository, IMapper mapper)
        {
            _playerPrizeRepository = playerPrizeRepository;
            _mapper = mapper;
        }
        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
        }
        public async Task<ServiceResponse<Guid>> CreateNewPlayerPrize(CreatePlayerPrizeDto createPlayerPrizeDto)
        {
            if (await _playerPrizeRepository.ExistsAsync(s => s.PlayerId == createPlayerPrizeDto.PlayerId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Prize with the same player already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            TimeZoneVietName(createPlayerPrizeDto.CreatedAt);

            var mapper = config.CreateMapper();
            var createPlayerPrize = mapper.Map<PlayerPrize>(createPlayerPrizeDto);
            createPlayerPrize.Id = Guid.NewGuid();
            await _playerPrizeRepository.AddAsync(createPlayerPrize);

            return new ServiceResponse<Guid>
            {
                Data = createPlayerPrize.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<IEnumerable<PlayerPrizeDto>>> GetPlayerPrize()
        {
            var playerPrizeList = await _playerPrizeRepository.GetAllAsync<PlayerPrizeDto>();

            if (playerPrizeList != null)
            {
                playerPrizeList = playerPrizeList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<PlayerPrizeDto>>
                {
                    Data = playerPrizeList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<PlayerPrizeDto>>
                {
                    Data = playerPrizeList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<GetPlayerPrizeDto>> GetPlayerPrizeById(Guid prizeId)
        {
            try
            {

                var eventDetail = await _playerPrizeRepository.GetAsync<GetPlayerPrizeDto>(prizeId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<GetPlayerPrizeDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerPrizeDto>
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

        public async Task<ServiceResponse<bool>> UpdatePlayerPrize(Guid id, UpdatePlayerPrizeDto playerPrizeDto)
        {
            var existingPlayerPrize = await _playerPrizeRepository.GetById(id);

            if (existingPlayerPrize == null)
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
                existingPlayerPrize.PrizeId = playerPrizeDto.PrizeId;
                existingPlayerPrize.PlayerId = playerPrizeDto.PlayerId;
                await _playerPrizeRepository.UpdateAsync(id, existingPlayerPrize);
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
                if (!await PlayerPrizeExists(id))
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

        private async Task<bool> PlayerPrizeExists(Guid id)
        {
            return await _playerPrizeRepository.Exists(id);
        }
    }
}

using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.NPCDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Repositories.ItemRepositories;
using DataAccess.Repositories.PlayerHistoryRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.PlayerHistoryService
{
    public class PlayerHistoryService : IPlayerHistoryService
    {
        private readonly IPlayerHistoryRepository _playerHistoryRepository;
        private readonly IMapper _mapper;
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MapperConfig());
        });
        public PlayerHistoryService(IPlayerHistoryRepository playerHistoryRepository, IMapper mapper)
        {
            _playerHistoryRepository = playerHistoryRepository;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<Guid>> CreateNewPlayerHistory(CreatePlayerHistoryDto createPlayerHistoryDto)
        {
            var playerHistoryDtos = await _playerHistoryRepository.GetAllAsync<GetPlayerHistoryDto>();
            if (playerHistoryDtos != null)
            {
               var playerHistory = playerHistoryDtos.FirstOrDefault(x => x.PlayerId == createPlayerHistoryDto.PlayerId && x.EventtaskId == createPlayerHistoryDto.EventtaskId);
                if(playerHistory != null)
                {
                    return new ServiceResponse<Guid>
                    {
                        Message = "Failed taskId have exists",
                        Success = false,
                        StatusCode = 400
                    };
                }
                else
                {
                    createPlayerHistoryDto.Status = createPlayerHistoryDto.Status.Trim();
                    TimeZoneVietName(createPlayerHistoryDto.CreatedAt);

                    var mapper = config.CreateMapper();
                    var createPlayerHistory = mapper.Map<PlayerHistory>(createPlayerHistoryDto);
                    createPlayerHistory.Id = Guid.NewGuid();
                    await _playerHistoryRepository.AddAsync(createPlayerHistory);

                    return new ServiceResponse<Guid>
                    {
                        Data = createPlayerHistory.Id,
                        Message = "Successfully",
                        Success = true,
                        StatusCode = 201
                    };
                }
            }
            else
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Null",
                    Success = false,
                    StatusCode = 400
                };
            }
            
        }

        public async Task<ServiceResponse<bool>> DisablePlayerHistory(Guid id)
        {
            var checkEvent = await _playerHistoryRepository.GetAsync<PlayerHistoryDto>(id);

            if (checkEvent == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Failed",
                    StatusCode = 400,
                    Success = false
                };
            }
            else
            {
                /*checkEvent.Status = "INACTIVE";
                await _playerHistoryRepository.UpdateAsync(id, checkEvent);*/
                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetPlayerHistoryDto>>> GetPlayerHistory()
        {
            var playerhistoryList = await _playerHistoryRepository.GetAllAsync<GetPlayerHistoryDto>();

            if (playerhistoryList != null)
            {
                playerhistoryList = playerhistoryList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<GetPlayerHistoryDto>>
                {
                    Data = playerhistoryList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetPlayerHistoryDto>>
                {
                    Data = playerhistoryList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }

        public async Task<ServiceResponse<GetPlayerHistoryDto>> GetPlayerHistoryById(Guid eventId)
        {
            try
            {   
                var playerHistoryDto = await _playerHistoryRepository.GetAsync<GetPlayerHistoryDto>(eventId);

                if (playerHistoryDto == null)
                {

                    return new ServiceResponse<GetPlayerHistoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerHistoryDto>
                {
                    Data = playerHistoryDto,
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

        public async Task<ServiceResponse<PlayerHistoryDto>> GetPlayerHistoryByEventTaskId(Guid eventTaskId)
        {
            try
            {
             
                var playerHistory = await _playerHistoryRepository.GetByWithCondition(x => x.EventtaskId == eventTaskId, null, true);
                var _mapper = config.CreateMapper();
                var playerHistoryDto = _mapper.Map<PlayerHistoryDto>(playerHistory);
               
                if (playerHistoryDto == null)
                {

                    return new ServiceResponse<PlayerHistoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<PlayerHistoryDto>
                {
                    Data = playerHistoryDto,
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

        public async Task<ServiceResponse<GetPlayerHistoryDto>> GetPlayerHistoryByEventTaskIdAndPlayerId(Guid taskId, Guid PlayerId)
        {
            try
            {
                var playerHistory = await _playerHistoryRepository.GetPlayerHistoryByEventTaskIdAndPlayerId(taskId, PlayerId);
               
                if (playerHistory == null)
                {

                    return new ServiceResponse<GetPlayerHistoryDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerHistoryDto>
                {
                    Data = playerHistory,
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

        public async Task<ServiceResponse<bool>> UpdatePlayerHistory(Guid id, UpdatePlayerHistoryDto PlayerHistoryDto)
        {
            var existingPlayerHistory = await _playerHistoryRepository.GetById(id);

            if (existingPlayerHistory == null)
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
                existingPlayerHistory.TaskPoint = PlayerHistoryDto.TaskPoint;
                existingPlayerHistory.PlayerId = PlayerHistoryDto.PlayerId;
                existingPlayerHistory.EventtaskId = PlayerHistoryDto.EventtaskId;
                existingPlayerHistory.CompletedTime = PlayerHistoryDto.CompletedTime;
                existingPlayerHistory.Status = PlayerHistoryDto.Status.Trim();
                await _playerHistoryRepository.UpdateAsync(id, existingPlayerHistory);
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
        private void TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
        }
        private async Task<bool> EventTaskExists(Guid id)
        {
            return await _playerHistoryRepository.Exists(id);
        }

        
    }
}

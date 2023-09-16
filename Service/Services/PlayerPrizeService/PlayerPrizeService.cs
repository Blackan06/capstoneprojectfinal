using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.Repositories.PlayerHistoryRepositories;
using DataAccess.Repositories.PlayerPrizeRepositories;
using DataAccess.Repositories.PlayerRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Email;
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
        private readonly IPlayerRepository _playerRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
      
        public PlayerPrizeService(IPlayerPrizeRepositories playerPrizeRepository, IMapper mapper, IEmailSender emailSender,IPlayerRepository playerRepository)
        {
            _playerPrizeRepository = playerPrizeRepository;
            _playerRepository = playerRepository;
            _emailSender = emailSender;
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
            createPlayerPrizeDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);

            var createPlayerPrize = _mapper.Map<PlayerPrize>(createPlayerPrizeDto);
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
        public async Task<ServiceResponse<IEnumerable<RankPlayer>>> GetRankedPlayer(Guid eventId, Guid schoolId)
        {
            try
            {

                var eventDetail = await _playerRepository.GetRankedPlayer(eventId, schoolId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<IEnumerable<RankPlayer>>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<IEnumerable<RankPlayer>>
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
        private async Task<bool> PlayerPrizeExists(Guid id)
        {
            return await _playerPrizeRepository.Exists(id);
        }

        public async Task<ServiceResponse<GetPlayerPrizeDto>> GetPlayerPrizeByEventIdAndSchoolId(Guid eventId, Guid schoolId)
        {
            var playerPrize = await _playerPrizeRepository.GetPlayerPrizeByEventIdAndSchoolId(eventId, schoolId);

            if(playerPrize == null)
            {
                return new ServiceResponse<GetPlayerPrizeDto>
                {
                    Data = null,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
            return new ServiceResponse<GetPlayerPrizeDto>
            {
                Data = playerPrize,
                Message = "Success",
                StatusCode = 200,
                Success = true
            };
        }

        public async Task<ServiceResponse<Guid>> CreateNewPlayerPrizeWithplayerId(Guid PlayerId, CreatePlayerPrize2Dto createPlayerPrizeDto)
        {
            if (await _playerPrizeRepository.ExistsAsync(s => s.PlayerId == PlayerId))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Prize with the same player already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            createPlayerPrizeDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);

            var createPlayerPrize = _mapper.Map<PlayerPrize>(createPlayerPrizeDto);
            createPlayerPrize.Id = Guid.NewGuid();
            createPlayerPrize.PlayerId = PlayerId;
            await _playerPrizeRepository.AddAsync(createPlayerPrize);

            bool emailSent = await SendPlayerPrizeEmail(PlayerId, createPlayerPrizeDto);

            if (!emailSent)
            {
                // Xử lý trường hợp gửi email thất bại (có thể làm ghi log hoặc thử lại sau)
                return new ServiceResponse<Guid>
                {
                    Message = "Successfully created player prize, but failed to send email notification.",
                    Success = true,
                    StatusCode = 201
                };
            }

            return new ServiceResponse<Guid>
            {
                Data = createPlayerPrize.Id,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };

        }

        public async Task<bool> SendPlayerPrizeEmail(Guid playerId, CreatePlayerPrize2Dto playerPrizeDto)
        {
            try
            {
                var playerPrize = await _playerPrizeRepository.GetPlayerPrizeByPlayerId(playerId);
                // Tạo nội dung email
                var subject = "Thông báo nhận phần thưởng";
                var body = $"Chào bạn {playerPrize.StudentName} của trường {playerPrize.SchoolName}, bạn đã nhận phần thưởng '{playerPrize.PrizeName}' vào ngày {playerPrizeDto.DateReceived} của sự kiện {playerPrize.EventName}.";

                // Gửi email thông báo cho người chơi
                var recipientEmail = playerPrize.StudentEmail; // Thay bằng địa chỉ email của người chơi
                var emailMessage = new EmailMessage
                {
                    To = recipientEmail,
                    Subject = subject,
                    Body = body,
                };
                await _emailSender.SendEmailAsync(emailMessage);

                // Trả về true nếu email được gửi thành công
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi gửi email ở đây (có thể ghi log)
                return false;
            }
        }
    }
}

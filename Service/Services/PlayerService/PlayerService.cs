using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.Repositories.InventoryRepositories;
using DataAccess.Repositories.PlayerRepositories;
using DataAccess.Repositories.SchoolRepositories;
using DataAccess.Repositories.StudentRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Service.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IStudentRepositories _studentRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
    
        public PlayerService(IPlayerRepository playerRepository, IMapper mapper, IStudentRepositories studentRepository, ISchoolRepository schoolRepository,IInventoryRepository inventoryRepository)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _schoolRepository = schoolRepository;   
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ServiceResponse<Guid>> CreateNewPlayer(CreatePlayerDto createPlayerDto)
        {
            if (await _playerRepository.ExistsAsync(s => s.Passcode == createPlayerDto.Passcode))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "Duplicated data: Player with the same passcode already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingPlayerForStudent = await _playerRepository.GetPlayerByStudentId(createPlayerDto.StudentId);
            if (existingPlayerForStudent != null &&  existingPlayerForStudent.Passcode != null)
            {
                existingPlayerForStudent.TotalPoint = 0;
                existingPlayerForStudent.TotalTime = 0;
                await _playerRepository.UpdateAsync(existingPlayerForStudent.Id, existingPlayerForStudent);

                await _studentRepository.UpdateStudentStatusAsync(existingPlayerForStudent.StudentId, "ACTIVE");
                return new ServiceResponse<Guid>
                {
                    Data = existingPlayerForStudent.Id,
                    Message = "Successfully",
                    Success = true,
                    StatusCode = 201
                };
            }
            else
            {
                createPlayerDto.Nickname = "null";
                createPlayerDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow);
                createPlayerDto.TotalPoint = 0;
                createPlayerDto.TotalTime = 0;
                createPlayerDto.Passcode = Guid.NewGuid().ToString("N").Substring(0, 8);
                createPlayerDto.IsPlayer = false;
                var _player = _mapper.Map<Player>(createPlayerDto);
                _player.Id = Guid.NewGuid();
                await _playerRepository.AddAsync(_player);

                return new ServiceResponse<Guid>
                {
                    Data = _player.Id,
                    Message = "Successfully",
                    Success = true,
                    StatusCode = 201
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
        public async Task<ServiceResponse<List<Guid>>> CreateNewPlayers(CreateListPlayerDto listPlayerDto)
        {
            var addedPlayerIds = new List<Guid>();
            var newPlayers = new List<Player>();

            foreach (var studentId in listPlayerDto.StudentId)
            {
                if (await _playerRepository.ExistsAsync(t => t.EventId == listPlayerDto.EventId && t.StudentId == studentId))
                {
                    return new ServiceResponse<List<Guid>>
                    {
                        Message = "A task with the same EventId and TaskId already exists in the event.",
                        Success = false,
                        StatusCode = 400
                    };
                }

                int desiredLength = new Random().Next(8, 11); 
                string guidString = Guid.NewGuid().ToString("N"); 
                string passcode = guidString.Substring(0, desiredLength);

                var playerdto = new PlayerDto
                {
                    StudentId = studentId,
                    Nickname = "null",
                    Isplayer = false,
                    Passcode = passcode,
                    TotalPoint = 0,
                    TotalTime = 0,
                };

                listPlayerDto.CreatedAt = TimeZoneVietName(DateTime.UtcNow); 
                addedPlayerIds.Add(playerdto.Id);
                var player = _mapper.Map<Player>(playerdto);
                newPlayers.Add(player);
            }

            await _playerRepository.AddRangeAsync(newPlayers);

            return new ServiceResponse<List<Guid>>
            {
                Data = addedPlayerIds,
                Message = "Successfully",
                Success = true,
                StatusCode = 201
            };
        }

        public async Task<ServiceResponse<GetPlayerDto>> CheckPlayerByNickName(string nickName)
        {
            try
            {
                List<Expression<Func<Player, object>>> includes = new List<Expression<Func<Player, object>>>
                {
                  
                    x => x.Inventories,
                    x => x.ExchangeHistories,
                    
                };
                var taskDetail = await _playerRepository.GetByWithCondition(x => x.Nickname.Equals(nickName), null, true);
                var taskDetailDto = _mapper.Map<GetPlayerDto>(taskDetail);
                if (taskDetail == null)
                {

                    return new ServiceResponse<GetPlayerDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerDto>
                {
                    Data = taskDetailDto,
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
        public async Task<ServiceResponse<IEnumerable<PlayerDto>>> GetPlayer()
        {
            var playerList = await _playerRepository.GetAllAsync<PlayerDto>();

            if (playerList != null)
            {
                playerList = playerList.OrderByDescending(e => e.CreatedAt).ToList();

                return new ServiceResponse<IEnumerable<PlayerDto>>
                {
                    Data = playerList,
                    Success = true,
                    Message = "Successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse<IEnumerable<PlayerDto>>
                {
                    Data = playerList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }
        public async Task<ServiceResponse<IEnumerable<PlayerDto>>> GetPlayerWithNickName()
        {
            var playerList = await _playerRepository.GetAllAsync<PlayerDto>();

            if (playerList != null)
            {
                playerList = playerList.OrderByDescending(e => e.CreatedAt).ToList();
                var validPlayers = playerList.Where(player => !string.IsNullOrEmpty(player.Nickname)).ToList();

                if (validPlayers != null)
                {
                    return new ServiceResponse<IEnumerable<PlayerDto>>
                    {
                        Data = validPlayers,
                        Success = true,
                        Message = "Successfully",
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ServiceResponse<IEnumerable<PlayerDto>>
                    {
                        Data = playerList,
                        Success = false,
                        Message = "Faile because List event null",
                        StatusCode = 200
                    };
                }
            }
            else
            {
                return new ServiceResponse<IEnumerable<PlayerDto>>
                {
                    Data = playerList,
                    Success = false,
                    Message = "Faile because List event null",
                    StatusCode = 200
                };
            }
        }
        public async Task<ServiceResponse<GetPlayerDto>> GetPlayerById(Guid eventId)
        {
            try
            {

                var eventDetail = await _playerRepository.GetAsync<GetPlayerDto>(eventId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<GetPlayerDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerDto>
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

        public async Task<ServiceResponse<GetPlayerDto>> GetPlayerByStudentId(Guid studentId)
        {
            try
            {
                List<Expression<Func<Player, object>>> includes = new List<Expression<Func<Player, object>>>
                {
                    x => x.Inventories,
                    x => x.ExchangeHistories,
                };
                var taskDetail = await _playerRepository.GetByWithCondition(x => x.StudentId == studentId, includes, true);
                var taskDetailDto = _mapper.Map<GetPlayerDto>(taskDetail);
                if (taskDetail == null)
                {

                    return new ServiceResponse<GetPlayerDto>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<GetPlayerDto>
                {
                    Data = taskDetailDto,
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

     
        
        public async Task<ServiceResponse<bool>> UpdatePlayer(Guid id, UpdatePlayerDto updatePlayerDto)
        {
            if (await _playerRepository.ExistsAsync(s => s.Nickname == updatePlayerDto.Nickname && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Player with the same name already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            if (await _playerRepository.ExistsAsync(s => s.Passcode == updatePlayerDto.Passcode && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Player with the same passcode already exists.",
                    Success = false,
                    StatusCode = 400
                };
            } 
            if (await _playerRepository.ExistsAsync(s => s.StudentId == updatePlayerDto.StudentId && s.Id != id))
            {
                return new ServiceResponse<bool>
                {
                    Message = "Duplicated data: Player with the same student already exists.",
                    Success = false,
                    StatusCode = 400
                };
            }
            var existingPlayer = await _playerRepository.GetById(id);

            if (existingPlayer == null)
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
                existingPlayer.Nickname = updatePlayerDto.Nickname.Trim();
                existingPlayer.Passcode = updatePlayerDto.Passcode.Trim();
                existingPlayer.TotalPoint = updatePlayerDto.TotalPoint;
                existingPlayer.TotalTime = updatePlayerDto.TotalTime;
                existingPlayer.Isplayer = updatePlayerDto.IsPlayer;

                await _playerRepository.UpdateAsync(id, existingPlayer);

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
            return await _playerRepository.Exists(id);
        }
        public async Task<ServiceResponse<IEnumerable<Player>>> GetTop5PlayerInRank()
        {
            try
            {
                var context = new db_a9c31b_capstoneContext();
                List<Player> top5playerlist = context.Players.OrderByDescending(x => x.TotalPoint).ThenBy(x => x.TotalTime).Take(5).ToList();
                return new ServiceResponse<IEnumerable<Player>>
                {
                    Data = top5playerlist,
                    Message = "Success edit",
                    Success = true,
                    StatusCode = 202
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ServiceResponse<IEnumerable<Player>>
                {
                    Message = "Invalid Record Id",
                    Success = false,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<PlayerDto>>> GetRankedPlayer(Guid eventId, Guid schoolId)
        {
            try
            {

                var eventDetail = await _playerRepository.GetRankedPlayer(eventId, schoolId);

                if (eventDetail == null)
                {

                    return new ServiceResponse<IEnumerable<PlayerDto>>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<IEnumerable<PlayerDto>>
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

        public async Task<ServiceResponse<Guid>> GetSchoolByPlayerId(Guid playerId)
        {
            try
            {
                var schoolId = await _playerRepository.GetSchoolByPlayerId(playerId);

                if (schoolId == Guid.Empty)
                {

                    return new ServiceResponse<Guid>
                    {
                        Message = "No rows",
                        StatusCode = 200,
                        Success = true
                    };
                }
                return new ServiceResponse<Guid>
                {
                    Data = schoolId,
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

        public async Task<ServiceResponse<GetPlayerDto>> GetPlayerByEventId(Guid eventId)
        {
            var player = await _playerRepository.GetPlayerByEventId(eventId);
            if(player == null)
            {
                return new ServiceResponse<GetPlayerDto>
                {
                    Message = "No rows",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                return new ServiceResponse<GetPlayerDto>
                {
                    Data = player,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
        public async Task<ServiceResponse<IEnumerable<GetPlayerWithSchoolAndEvent>>> filterData(Guid? schoolId, Guid? eventId)
        {
            var listStudentFilterData = await _playerRepository.filterData(schoolId, eventId);
            if (listStudentFilterData == null)
            {
                return new ServiceResponse<IEnumerable<GetPlayerWithSchoolAndEvent>>
                {
                    Data = null,
                    Message = "DATA NULL",
                    StatusCode = 200,
                    Success = false
                };
            }
            return new ServiceResponse<IEnumerable<GetPlayerWithSchoolAndEvent>>
            {
                Data = listStudentFilterData,
                Message = "Successfully",
                StatusCode = 200,
                Success = false
            };
        }
        public async Task<ServiceResponse<GetPlayerDto>> GetPlayerBySchoolId(Guid schoolId)
        {
            var player = await _playerRepository.GetPlayerBySchoolId(schoolId);
            if (player == null)
            {
                return new ServiceResponse<GetPlayerDto>
                {
                    Message = "No rows",
                    StatusCode = 200,
                    Success = true
                };
            }
            else
            {
                return new ServiceResponse<GetPlayerDto>
                {
                    Data = player,
                    Message = "Success",
                    StatusCode = 200,
                    Success = true
                };
            }
        }
    }
}

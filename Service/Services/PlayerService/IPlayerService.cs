using BusinessObjects.Model;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.StudentDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.PlayerService
{
    public interface IPlayerService
    {
        Task<ServiceResponse<IEnumerable<PlayerDto>>> GetPlayer();
        Task<ServiceResponse<GetPlayerDto>> GetPlayerById(Guid eventId);
        Task<ServiceResponse<GetPlayerDto>> GetPlayerByStudentId(Guid studentId);        
        Task<ServiceResponse<GetPlayerDto>> CheckPlayerByNickName(string nickName);
        Task<ServiceResponse<Guid>> CreateNewPlayer(CreatePlayerDto createPlayerDto);
        Task<ServiceResponse<bool>> UpdatePlayer(Guid id, UpdatePlayerDto PlayerDto);
        Task<ServiceResponse<IEnumerable<Player>>> GetTop5PlayerInRank();
        Task<ServiceResponse<IEnumerable<PlayerDto>>> GetRankedPlayer(Guid eventId, Guid schoolId);
        Task<ServiceResponse<IEnumerable<GetPlayerWithSchoolAndEvent>>> filterData(Guid? schoolId, Guid? eventId);
        Task<ServiceResponse<IEnumerable<PlayerDto>>> GetPlayerWithNickName();
        Task<ServiceResponse<Guid>> GetSchoolByPlayerId(Guid playerId);
        Task<ServiceResponse<GetPlayerDto>> GetPlayerByEventId(Guid eventId);
        Task<ServiceResponse<GetPlayerDto>> GetPlayerBySchoolId(Guid schoolId);

        Task<ServiceResponse<List<Guid>>> CreateNewPlayers(CreateListPlayerDto players);
    }
}

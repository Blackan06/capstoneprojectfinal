using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.Dtos.PlayerPrizeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.PlayerPrizeService
{
    public interface IPlayerPrizeService
    {
        Task<ServiceResponse<IEnumerable<PlayerPrizeDto>>> GetPlayerPrize();
        Task<ServiceResponse<GetPlayerPrizeDto>> GetPlayerPrizeById(Guid prizeId);
        Task<ServiceResponse<Guid>> CreateNewPlayerPrize(CreatePlayerPrizeDto createPlayerPrizeDto);
        Task<ServiceResponse<Guid>> CreateNewPlayerPrizeWithplayerId(Guid PlayerId,CreatePlayerPrize2Dto createPlayerPrizeDto);
        Task<ServiceResponse<bool>> UpdatePlayerPrize(Guid id, UpdatePlayerPrizeDto PlayerPrizeDto);
        Task<ServiceResponse<GetPlayerPrizeDto>> GetPlayerPrizeByEventIdAndSchoolId(Guid eventId, Guid schoolId);
        Task<ServiceResponse<IEnumerable<RankPlayer>>> GetRankedPlayer(Guid eventId, Guid schoolId);

        Task<bool> SendPlayerPrizeEmail(Guid playerId, CreatePlayerPrize2Dto playerPrizeDto);

    }
}

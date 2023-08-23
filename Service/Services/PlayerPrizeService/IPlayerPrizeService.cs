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
        Task<ServiceResponse<bool>> UpdatePlayerPrize(Guid id, UpdatePlayerPrizeDto PlayerPrizeDto);
    }
}

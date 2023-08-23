using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.PlayerHistoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.PlayerHistoryService
{
    public interface IPlayerHistoryService
    {
        Task<ServiceResponse<IEnumerable<GetPlayerHistoryDto>>> GetPlayerHistory();
        Task<ServiceResponse<GetPlayerHistoryDto>> GetPlayerHistoryById(Guid eventId);
        Task<ServiceResponse<PlayerHistoryDto>> GetPlayerHistoryByEventTaskId(Guid eventTaskId);
        Task<ServiceResponse<GetPlayerHistoryDto>> GetPlayerHistoryByEventTaskIdAndPlayerId(Guid taskId, Guid PlayerId);
        Task<ServiceResponse<Guid>> CreateNewPlayerHistory(CreatePlayerHistoryDto createPlayerHistoryDto);
        Task<ServiceResponse<bool>> UpdatePlayerHistory(Guid id, UpdatePlayerHistoryDto PlayerHistoryDto);

        Task<ServiceResponse<bool>> DisablePlayerHistory(Guid id);

    }
}

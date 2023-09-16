using BusinessObjects.Model;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerPrizeRepositories
{
    public interface IPlayerPrizeRepositories : IGenericRepository<PlayerPrize>
    {
        Task<GetPlayerPrizeDto> GetPlayerPrizeByEventIdAndSchoolId(Guid eventId, Guid schoolId);
        Task<GetPlayerPrize2Dto> GetPlayerPrizeByPlayerId(Guid playerId);
    }
}

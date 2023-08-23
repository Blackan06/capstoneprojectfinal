using BusinessObjects.Model;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerHistoryRepositories
{
    public interface IPlayerHistoryRepository : IGenericRepository<PlayerHistory>
    {
        Task<GetPlayerHistoryDto> GetPlayerHistoryByEventTaskIdAndPlayerId(Guid taskId, Guid PlayerId);
    }
}

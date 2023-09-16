using BusinessObjects.Model;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerRepositories
{
    public interface IPlayerRepository : IGenericRepository<Player>
    {
      
        Task<IEnumerable<RankPlayer>> GetRankedPlayer(Guid eventid, Guid schoolId);

        Task<Guid> GetSchoolByPlayerId(Guid playerId);
        Task<Player> GetPlayerByStudentId(Guid studentId);
        Task<GetPlayerDto> GetPlayerByEventId(Guid eventId);
        Task<GetPlayerDto> GetPlayerBySchoolId(Guid schoolId);
        Task<IEnumerable<GetPlayerWithSchoolAndEvent>> filterData(Guid? schoolId, Guid? eventId);

    }
}

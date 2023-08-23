using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerRepositories
{
    public class PlayerRepository : GenericRepository<Player> , IPlayerRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public PlayerRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetPlayerDto> GetPlayerByEventId(Guid eventId)
        {
            var player = await _dbContext.Players
                .Include(p => p.Student)
                .ThenInclude(s => s.School)
                .FirstOrDefaultAsync(p => p.EventId == eventId);
            if (player != null)
            {
                var getPlayerDto = _mapper.Map<GetPlayerDto>(player);
                return getPlayerDto;
            }

            return null;
        }

        public async Task<GetPlayerDto> GetPlayerBySchoolId(Guid schoolId)
        {
            var player = await _dbContext.Players
                    .Include(p => p.Student)
                        .ThenInclude(s => s.School)
                    .FirstOrDefaultAsync(p => p.Student.SchoolId == schoolId);

            if (player != null)
            {
                var getPlayerDto = _mapper.Map<GetPlayerDto>(player);
                return getPlayerDto;
            }

            return null;
        }

        public async Task<Player> GetPlayerByStudentId(Guid studentId)
        {
            var player = await _dbContext.Players.Include(x => x.Student).FirstOrDefaultAsync(x => x.StudentId == studentId);
            if(player == null)
            {
                return null;
            }
            return player;
        }

        public async Task<IEnumerable<PlayerDto>> GetRankedPlayer(Guid eventid, Guid schoolId)
        {
            var ranked = await _dbContext.Players.Include(s => s.Student.School).Where(p => p.EventId.Equals(eventid) && p.Student.SchoolId.Equals(schoolId) &&
                (p.Nickname != null || p.TotalPoint != 0 || p.TotalTime != 0)).
                OrderByDescending(x => x.TotalPoint).ThenBy(x => x.TotalTime).Select(x => new PlayerDto
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    EventName = x.Event.Name,
                    StudentName = x.Student.Fullname,
                    Passcode = x.Passcode,
                    StudentId = x.StudentId,
                    Nickname = x.Nickname,
                    CreatedAt = x.CreatedAt,
                    TotalPoint = x.TotalPoint,
                    TotalTime = x.TotalTime,
                    Isplayer = x.Isplayer
                }).ToListAsync();

            return ranked;
        }

        public async Task<Guid> GetSchoolByPlayerId(Guid playerId)
        {
            var school = await _dbContext.Schools
                .Include(s => s.Students)
                    .ThenInclude(st => st.Player)
                .FirstOrDefaultAsync(s => s.Students.Any(st => st.Player.Id == playerId));
           
            return school.Id;
        }

        /*public async Task<GetPlayerWithStudentNameDto> ExportPlayersData()
        {
            var schoolList = await _dbContext.Schools.ToListAsync();
            if(schoolList != null)
            {
                
            }
            
        }*/
    }
}
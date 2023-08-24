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
           var rankedPlayers = await _dbContext.PlayerHistories
                                .Include(ph => ph.Player) // Đảm bảo tải thông tin của người chơi (Player)
                                .Where(ph => ph.Eventtask.EventId == eventid && ph.Player.Student.SchoolId == schoolId)
                                .GroupBy(ph => ph.PlayerId)
                                .Select(group => new PlayerDto
                                {
                                    Id = group.Key,
                                    EventId = eventid,
                                    EventName = group.First().Eventtask.Event.Name,
                                    StudentName = group.First().Player.Student.Fullname,
                                    Passcode = group.First().Player.Passcode,
                                    StudentId = group.First().Player.StudentId,
                                    Nickname = group.First().Player.Nickname,
                                    CreatedAt = group.First().Player.CreatedAt,
                                    TotalPoint = group.Sum(ph => ph.TaskPoint ?? 0) , 
                                    TotalTime = group.Sum(ph => ph.CompletedTime ?? 0),   
                                    Isplayer = group.First().Player.Isplayer
                                })
                                .OrderByDescending(p => p.TotalPoint)
                                .ThenBy(p => p.TotalTime)
                                .ToListAsync();

            return rankedPlayers;

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
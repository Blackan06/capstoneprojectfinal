using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.StudentDto;
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
            var players = await _dbContext.Players
                .Include(p => p.Event)
                .Include(p => p.Student).ThenInclude(p => p.School)
                .Where(p => p.Student.SchoolId == schoolId && p.EventId == eventid)
                .ToListAsync();

          

            var playerDtos = players
                .Select(p => new PlayerDto
                {
                    Id = p.Id,
                    EventId = eventid,
                    EventName = p.Event.Name,  // Thay thế bằng logic lấy tên sự kiện nếu cần
                    StudentName = p.Student.Fullname,
                    SchoolName = p.Student.School.Name,
                    Passcode = p.Passcode,
                    StudentId = p.StudentId,
                    Nickname = !string.IsNullOrEmpty(p.Nickname) ? p.Nickname : "",  
                    CreatedAt = p.CreatedAt,
                    TotalPoint = p.TotalPoint,
                    TotalTime = p.TotalTime,
                    Isplayer = p.Isplayer
                })
                .Where(dto => dto.TotalPoint != 0 || dto.TotalTime != 0 || !string.IsNullOrEmpty(dto.Nickname))  // Loại bỏ những người chơi có point, time và nickname đều không tồn tại
                .OrderByDescending(p => p.TotalPoint)
                .ThenBy(p => p.TotalTime)
                .ToList();

            return playerDtos;
        }

        public async Task<IEnumerable<GetPlayerWithSchoolAndEvent>> filterData(Guid? schoolId, Guid? eventId)
        {
            IQueryable<Player> query = _dbContext.Players.Include(x => x.Student).ThenInclude(s => s.School)
                                                            .ThenInclude(school => school.SchoolEvents)
                                                            .ThenInclude(schoolEvent => schoolEvent.Event);
            if (schoolId.HasValue)
            {
                query = query.Where(s => s.Student.SchoolId == schoolId.Value);
            }
            if (eventId.HasValue)
            {
                query = query.Where(s => s.Student.School.SchoolEvents.Any(se => se.EventId == eventId.Value));
            }

            var result = await query.Select(s => new GetPlayerWithSchoolAndEvent
            {
                Id = s.Id,
                Email = s.Student.Email,
                Passcode = s.Passcode,
                Nickname = s.Nickname,
                SchoolName = s.Student.School.Name,
                TotalPoint = s.TotalPoint,
                TotalTime = s.TotalTime
            }).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
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
using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.StudentDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<IEnumerable<RankPlayer>> GetRankedPlayer(Guid eventid, Guid schoolId)
        {
            var players = await _dbContext.Players
                .Include(p => p.Event)
                .Include(p => p.Student).ThenInclude(p => p.School)
                .Include(x => x.PlayerPrizes)
                .Where(p => p.Student.SchoolId == schoolId && p.EventId == eventid)
                .ToListAsync();
            var schoolEvent = await _dbContext.SchoolEvents.Include(x => x.Event).Include(x => x.School).Where(x => x.SchoolId == schoolId && x.EventId == eventid).FirstOrDefaultAsync();
            if(schoolEvent == null)
            {
                return null;
            }
            else
            {
                var prizeLists = await _dbContext.Prizes
                                 .Include(x => x.PlayerPrizes)
                                 .Include(x => x.Schoolevent)
                                 .OrderBy(x => x.PrizeRank)
                                 .Where(x => x.SchooleventId == schoolEvent.Id)
                                 .ToListAsync();

                var playerDtos = players
                .Select((p) => new RankPlayer
                {
                        Id = p.Id,
                        EventId = eventid,
                        EventName = p.Event.Name,
                        StudentName = p.Student.Fullname,
                        SchoolName = p.Student.School.Name,
                        Passcode = p.Passcode,
                        StudentId = p.StudentId,
                        Nickname = !string.IsNullOrEmpty(p.Nickname) ? p.Nickname : "",
                        CreatedAt = p.CreatedAt,
                        TotalPoint = p.TotalPoint,
                        TotalTime = p.TotalTime,
                        Isplayer = p.Isplayer,
                })
                    .Where(dto => dto.TotalPoint != 0 || dto.TotalTime != 0 || !string.IsNullOrEmpty(dto.Nickname))  // Loại bỏ những người chơi có point, time và nickname đều không tồn tại
                    .OrderByDescending(p => p.TotalPoint)
                    .ThenBy(p => p.TotalTime)
                    .ToList();

                for (int i = 0; i < playerDtos.Count; i++)
                {
                    if (i >= prizeLists.Count)
                    {
                        playerDtos[i].PrizedId = Guid.Empty;
                        playerDtos[i].PrizedName = null;

                    }
                    else
                    {
                      var checkPrize = prizeLists.Any(x => x.PrizeRank == i);
                        if (checkPrize)
                        {
                            playerDtos[i].PrizedId = prizeLists[i].Id;
                            playerDtos[i].PrizedName = prizeLists[i].Name;
                        }
                    }
                   
                    
                }
                return playerDtos;
            }
         
        }
        private string GetNextPrizeName(List<Prize> prizeList)
        {
            for (int i = 0; i < prizeList.Count; i++)
            {
                return prizeList[i].Name;
            }
            return null;
        }
        public async Task<IEnumerable<GetPlayerWithSchoolAndEvent>> filterData(Guid? schoolId, Guid? eventId)
        {
            IQueryable<Player> query = _dbContext.Players.Include(x => x.Student)
                                                        .ThenInclude(s => s.School)
                                                        .ThenInclude(school => school.SchoolEvents)
                                                        .ThenInclude(schoolEvent => schoolEvent.Event);
            if (schoolId == null && eventId == null)
            {
                // Trả về toàn bộ danh sách người chơi
                var result = await query.Select(s => new
                {
                    PlayerId = s.Id,
                    Email = s.Student.Email,
                    Passcode = s.Passcode,
                    Nickname = s.Nickname,
                    SchoolName = s.Student.School.Name,
                    TotalPoint = s.TotalPoint,
                    TotalTime = s.TotalTime,
                    StudentId = s.StudentId,
                    StudentName = s.Student.Fullname,
                    EventName = s.Event.Name,
                    CreatedAt = s.CreatedAt  // Include the CreatedAt property
                })
                .OrderByDescending(x => x.CreatedAt)  // Order by CreatedAt
                .ToListAsync();

                // Project the result into GetPlayerWithSchoolAndEvent objects
                var finalResult = result.Select(s => new GetPlayerWithSchoolAndEvent
                {
                    Id = s.PlayerId,
                    StudentEmail = s.Email,
                    Passcode = s.Passcode,
                    Nickname = s.Nickname,
                    SchoolName = s.SchoolName,
                    TotalPoint = s.TotalPoint,
                    TotalTime = s.TotalTime,
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    EventName = s.EventName,
                    CreatedAt = s.CreatedAt  // Assign the CreatedAt property
                }).ToList();

                return finalResult;
            }

            if (schoolId.HasValue)
            {
                query = query.Where(s => s.Student.SchoolId == schoolId.Value);
            }

            if (eventId.HasValue)
            {
                query = query.Where(s => s.Student.School.SchoolEvents.Any(se => se.EventId == eventId.Value));
            }
            if (schoolId.HasValue && eventId.HasValue)
            {
                query = query.Where(s => s.Student.School.SchoolEvents.Any(se => se.EventId == eventId.Value && se.SchoolId == schoolId.Value));

            }

            var filteredResult = await query.Select(s => new
            {
                PlayerId = s.Id,
                Email = s.Student.Email,
                Passcode = s.Passcode,
                Nickname = s.Nickname,
                SchoolName = s.Student.School.Name,
                TotalPoint = s.TotalPoint,
                TotalTime = s.TotalTime,
                StudentId = s.StudentId,
                StudentName = s.Student.Fullname,
                EventName = s.Event.Name,
                CreatedAt = s.CreatedAt  // Include the CreatedAt property
            })
       .OrderByDescending(x => x.CreatedAt)  // Order by CreatedAt
       .ToListAsync();

            // Project the filtered result into GetPlayerWithSchoolAndEvent objects
            var finalFilteredResult = filteredResult.Select(s => new GetPlayerWithSchoolAndEvent
            {
                Id = s.PlayerId,
                StudentEmail = s.Email,
                Passcode = s.Passcode,
                Nickname = s.Nickname,
                SchoolName = s.SchoolName,
                TotalPoint = s.TotalPoint,
                TotalTime = s.TotalTime,
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                EventName = s.EventName,
                CreatedAt = s.CreatedAt  // Assign the CreatedAt property
            }).ToList();

            return finalFilteredResult;

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
using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.PlayerDto;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerHistoryRepositories
{
    public class PlayerHistoryRepository : GenericRepository<PlayerHistory> , IPlayerHistoryRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public PlayerHistoryRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetPlayerHistoryDto> GetPlayerHistoryByEventTaskIdAndPlayerId(Guid taskId, Guid PlayerId)
        {
            var playerHistory = await _dbContext.PlayerHistories.Include(ph => ph.Eventtask).ThenInclude(x => x.Task)
                                    .FirstOrDefaultAsync(ph => ph.Eventtask.TaskId == taskId && ph.PlayerId == PlayerId);

            if (playerHistory == null)
            {
                return null; // Hoặc xử lý tùy ý nếu không tìm thấy thông tin lịch sử
            }

            // Sử dụng AutoMapper để ánh xạ sang PlayerHistoryDto (nếu cần thiết)
            var playerHistoryDto = _mapper.Map<GetPlayerHistoryDto>(playerHistory);

            return playerHistoryDto;
        }

        public async Task<IEnumerable<PlayerHistoryDto>> GetPlayerHistoryByPlayerId(Guid PlayerId)
        {
            var playerHistory = await _dbContext.PlayerHistories.Include(x => x.Player).ThenInclude(x => x.Student).ThenInclude(x => x.School)
                                                                .Include(x => x.Eventtask).ThenInclude(x => x.Event)
                                                                .Include(x => x.Eventtask)
                                                                    .ThenInclude(x => x.Task)
                                                                    .ThenInclude(x => x.Major)
                                                                    .Where(x => x.PlayerId == PlayerId).ToListAsync();
            var playerHistoryDtos = playerHistory
              .Select(p => new PlayerHistoryDto
              {
                 Id = p.Id,
                 PlayerId = p.PlayerId,
                 TaskId = p.Eventtask.TaskId,
                 TaskName = p.Eventtask.Task.Name,
                 EventtaskId = p.EventtaskId,
                 EventName = p.Eventtask.Event.Name,
                 Passcode = p.Player.Passcode,
                 MajorId = p.Eventtask.Task.MajorId,
                 MajorName = p.Eventtask.Task.Major.Name,
                 PlayerNickName = p.Player.Nickname,
                 Status = p.Status,
                 TaskPoint = p.TaskPoint,
                 CompletedTime = p.CompletedTime,
                 EventId = p.Eventtask.EventId,
                 StudentEmail = p.Player.Student.Email,
                 StudentName = p.Player.Student.Fullname,
                 SchoolName = p.Player.Student.School.Name,
                 TotalPoint = p.Player.TotalPoint,
                 TotalTime = p.Player.TotalTime,
                 
                 
              })
              .OrderByDescending(p => p.CompletedTime)
              .ToList();

                if (playerHistoryDtos == null)
                {
                    return null;
                }
                var playerHistoryDto = _mapper.Map<IEnumerable<PlayerHistoryDto>>(playerHistoryDtos);
                return playerHistoryDto;
        }
          
        
    }
}

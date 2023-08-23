using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.PlayerHistoryDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
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
    }
}

using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.PlayerPrizeDto;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.PlayerHistoryRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PlayerPrizeRepositories
{
    public class PlayerPrizeRepositories : GenericRepository<PlayerPrize>, IPlayerPrizeRepositories
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public PlayerPrizeRepositories(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

      

        public async Task<GetPlayerPrizeDto> GetPlayerPrizeByEventIdAndSchoolId(Guid eventId, Guid schoolId)
        {
            var playerPrize = await _dbContext.PlayerPrizes.Include(x => x.Player).ThenInclude(x => x.Student).ThenInclude(x => x.School)
                                                               .Include(x => x.Player).ThenInclude(x => x.Event).Where(x => x.Player.EventId == eventId && x.Player.Student.SchoolId == schoolId).FirstOrDefaultAsync();
        
            if(playerPrize == null)
            {
                return null;
            }

            var playerPrizeDto = _mapper.Map<GetPlayerPrizeDto>(playerPrize);
            return playerPrizeDto;
        }

        public async Task<GetPlayerPrize2Dto> GetPlayerPrizeByPlayerId(Guid playerId)
        {
            var playerPrize = await _dbContext.PlayerPrizes.Include(x => x.Player).ThenInclude(x => x.Student).ThenInclude(x => x.School)
                                                     .Include(x => x.Prize).ThenInclude(x => x.Schoolevent).Where(x => x.PlayerId == playerId).FirstOrDefaultAsync();

            var playerPrizeDto = new GetPlayerPrize2Dto
            {
                EventName = playerPrize.Prize.Schoolevent.Event.Name,
                DateReceived = playerPrize.DateReceived,
                Description = playerPrize.Prize.Description,
                PrizeName = playerPrize.Prize.Name,
                PrizeRank = playerPrize.Prize.PrizeRank,
                Quantity = playerPrize.Prize.Quantity,
                Status = playerPrize.Prize.Status,
                StudentName = playerPrize.Player.Student.Fullname,
                SchoolName = playerPrize.Prize.Schoolevent.School.Name,
                StudentEmail = playerPrize.Player.Student.Email
            };

            return playerPrizeDto;
        }
    }
}
using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.SchoolEventRepositories
{
    public class SchoolEventRepository : GenericRepository<SchoolEvent>,ISchoolEventRepository 
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public SchoolEventRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<GetSchoolByEventIdDto>> GetSchoolByEventId(Guid eventid)
        {
            var schoolList = await _dbContext.SchoolEvents.Include(x => x.Event).Include(se => se.School).OrderByDescending(x => x.CreatedAt).Where(se => se.EventId == eventid).Select(s => new GetSchoolByEventIdDto
            {
                Id = s.Id,
                Status = s.Status,
                ApprovalStatus = s.ApprovalStatus,
                EndTime = s.EndTime,
                SchoolId = s.SchoolId,
                SchoolName = s.School.Name,
                StartTime = s.StartTime,
                EventName = s.Event.Name,
                PhoneNumber = s.School.PhoneNumber.ToString(),
                Email = s.School.Email,
                CreatedAt = s.CreatedAt
            }).ToListAsync();

            return schoolList;
        }

        public async Task<SchoolEventDto> GetSchoolByEventIdCheck(Guid schoolEventId)
        {
            var schoolEvent = await _dbContext.SchoolEvents
                                .Include(x => x.Event)
                                .Include(se => se.School)
                                .OrderByDescending(x => x.CreatedAt)
                                .Where(se => se.Id == schoolEventId && se.Status == "ACTIVE").FirstOrDefaultAsync();
            
            if(schoolEvent == null)
            {
                return null;
            }
            var schoolEventDto = _mapper.Map<SchoolEventDto>(schoolEvent);
            return schoolEventDto;
        }
    }
}

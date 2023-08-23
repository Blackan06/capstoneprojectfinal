using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.SchoolDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<List<GetSchoolDto>> GetSchoolByEventId(Guid eventid)
        {
            var schoolList = await _dbContext.SchoolEvents.Include(se => se.School).Where(se => se.EventId.Equals(eventid)).Select(s => new GetSchoolDto
            {
                Id = s.School.Id,
                Name = s.School.Name,
                PhoneNumber = s.School.PhoneNumber.ToString(),
                Email = s.School.Email,
                Address = s.School.Address
            }).ToListAsync();
            return schoolList;
        }

    }
}

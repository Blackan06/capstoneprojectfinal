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

namespace DataAccess.Repositories.SchoolRepositories
{
    public class SchoolRepository : GenericRepository<School>, ISchoolRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public SchoolRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

      
        public async Task<IEnumerable<School>> GetSchoolByName(string schoolname)
        {
            var schoolList = await _dbContext.Schools.Where(x => x.Name.ToLower().Contains(schoolname.ToLower())).ToListAsync();
            return schoolList;
        }
    }
}

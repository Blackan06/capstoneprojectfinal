using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.LocationDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.LocationRepositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public LocationRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Location> GetLocationByName(string locationName)
        {
            var location = await _dbContext.Locations.FirstOrDefaultAsync(x => x.LocationName.Trim() == locationName.Trim());
            return location;
        }

        public async Task<IEnumerable<GetLocationDto>> GetLocationListNPC()
        {
            var listLocation = await _dbContext.Locations.Where(x => x.Status.Equals("ACTIVE") && x.LocationName.Contains("NPC")).ToListAsync();
            if (listLocation == null)
            {
                return null;

            }
            var listLocationNpc = _mapper.Map<IEnumerable<GetLocationDto>>(listLocation);
            return listLocationNpc;

        }
    }
}

using BusinessObjects.Model;
using DataAccess.Dtos.LocationDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.LocationRepositories
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<Location> GetLocationByName(string locationName);
        Task<IEnumerable<GetLocationDto>> GetLocationListNPC();

    }
}

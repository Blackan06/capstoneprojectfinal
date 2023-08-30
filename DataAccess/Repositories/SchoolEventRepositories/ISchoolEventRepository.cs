using BusinessObjects.Model;
using DataAccess.Dtos.SchoolDto;
using DataAccess.Dtos.SchoolEventDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.SchoolEventRepositories
{
    public interface ISchoolEventRepository : IGenericRepository<SchoolEvent>
    {
        Task<List<GetSchoolByEventIdDto>> GetSchoolByEventId(Guid eventid);
        Task<SchoolEventDto> GetSchoolByEventIdCheck(Guid eventid);

    }
}

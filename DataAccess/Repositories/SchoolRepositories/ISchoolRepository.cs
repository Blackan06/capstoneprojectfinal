using BusinessObjects.Model;
using DataAccess.Dtos.SchoolDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.SchoolRepositories
{
    public interface ISchoolRepository : IGenericRepository<School>
    {
        Task<IEnumerable<School>> GetSchoolByName(string schoolname);

    }
}

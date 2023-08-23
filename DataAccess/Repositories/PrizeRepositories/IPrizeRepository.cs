using BusinessObjects.Model;
using DataAccess.Dtos.PrizeDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.PrizeRepositories
{
    public interface IPrizeRepository : IGenericRepository<Prize>
    {
    }
}

using BusinessObjects.Model;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ExchangeHistoryRepositories
{
    public interface IExchangeHistoryRepository : IGenericRepository<ExchangeHistory>
    {
        Task<ExchangeHistoryDto> getExchangeByItemName(string itemName);
    }
}

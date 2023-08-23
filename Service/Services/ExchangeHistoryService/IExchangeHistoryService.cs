using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.LocationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ExchangeHistoryService
{
    public interface IExchangeHistoryService
    {
        Task<ServiceResponse<IEnumerable<ExchangeHistoryDto>>> GetExchangeHistory();
        Task<ServiceResponse<GetExchangeHistoryDto>> GetExchangeHistoryById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewExchangeHistory(CreateExchangeHistoryDto createEventTaskDto);
        Task<ServiceResponse<bool>> UpdateExchangeHistory(Guid id, UpdateExchangeHistoryDto eventTaskDto);
        Task<ServiceResponse<ExchangeHistoryDto>> GetExchangeByItemName(string itemName);

    }
}

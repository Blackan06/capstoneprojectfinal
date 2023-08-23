using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.PrizeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.PrizeService

{
    public interface IPrizeService
    {
        Task<ServiceResponse<IEnumerable<PrizeDto>>> GetPrize();
        Task<ServiceResponse<PrizeDto>> GetPrizeById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewPrize(CreatePrizeDto createGiftDto);
        Task<ServiceResponse<bool>> UpdatePrize(Guid id, UpdatePrizeDto giftDto);
        Task<ServiceResponse<string>> GetTotalPrize();
        Task<ServiceResponse<bool>> DisablePrize(Guid id);


    }
}

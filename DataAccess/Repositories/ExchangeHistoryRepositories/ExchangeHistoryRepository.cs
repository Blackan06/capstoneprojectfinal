using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.ExchangeHistoryDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ExchangeHistoryRepositories
{
    public class ExchangeHistoryRepository : GenericRepository<ExchangeHistory>, IExchangeHistoryRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public ExchangeHistoryRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ExchangeHistoryDto> getExchangeByItemName(string itemName)
        {
            var getItemByName = await _dbContext.ExchangeHistories
                       .Include(x => x.Item) // Chắc chắn rằng bạn đã Include bảng Item để có thể truy cập vào thông tin của nó
                       .Where(x => x.Item.Name == itemName)
                       .FirstOrDefaultAsync();

            // Ánh xạ từ ItemIventories sang ItemInventoryDto bằng AutoMapper
            ExchangeHistoryDto itemDto = _mapper.Map<ExchangeHistoryDto>(getItemByName);
            return itemDto;
        }


    }
}
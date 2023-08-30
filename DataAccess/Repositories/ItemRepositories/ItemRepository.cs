using AutoMapper;
using BusinessObjects.Model;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ItemRepositories
{
    public class ItemRepository : GenericRepository<Item> , IItemRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public ItemRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Item> GetItemByName(string itemName)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Name.Trim() == itemName.Trim());
            return item;
        }
    }
}

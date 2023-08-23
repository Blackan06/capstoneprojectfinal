using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ItemInventoryRepositories
{
    public class ItemInventoryRepositories : GenericRepository<ItemInventory> , IItemInventoryRepositories
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public ItemInventoryRepositories(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ItemInventoryDto> getItemByItemName(string itemName)
        {
            var getItemByName = await _dbContext.ItemInventories
                       .Include(x => x.Item) // Chắc chắn rằng bạn đã Include bảng Item để có thể truy cập vào thông tin của nó
                       .Where(x => x.Item.Name == itemName)
                       .FirstOrDefaultAsync();

            // Ánh xạ từ ItemIventories sang ItemInventoryDto bằng AutoMapper
            ItemInventoryDto itemDto = _mapper.Map<ItemInventoryDto>(getItemByName);
            return itemDto;
        }

        public async Task<GetListItemInventoryByPlayer> GetListItemInventoryByPlayer(string PlayerNickName)
        {
            var player = await _dbContext.Players
                .Where(e => e.Nickname == PlayerNickName)
                .Include(e => e.Inventories).ThenInclude(inventory => inventory.ItemInventories)
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return null; // Xử lý khi không tìm thấy người chơi
            }

            // Thay đổi ở đây, lấy danh sách Item thay vì ItemInventory theo InventoryId
            var inventoryId = player.Inventories.FirstOrDefault()?.Id ?? Guid.Empty;
            var items = await _dbContext.ItemInventories
                .Where(itemInventory => itemInventory.InventoryId == inventoryId)
                .Select(itemInventory => itemInventory.Item) // Lấy danh sách các Item từ ItemInventory
                .ToListAsync();
            var listItemInventoryByPlayer = new GetListItemInventoryByPlayer
            {
                PlayerId = player.Id,
                InventoryId = inventoryId,
                // Ánh xạ từ danh sách Item sang GetItemDto (nếu cần)
                ListItem = _mapper.Map<List<GetListItemDto>>(items),
                ListItemInventory = _mapper.Map<List<GetItemInventoryDto>>(player.Inventories
                                    .FirstOrDefault()?.ItemInventories)
            };

            return listItemInventoryByPlayer;
        }

    }
}

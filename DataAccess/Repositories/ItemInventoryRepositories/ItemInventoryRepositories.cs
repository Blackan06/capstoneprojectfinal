﻿using AutoMapper;
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

        public async Task<ItemInventory> GetByItemId(Guid itemId, Guid inventoryId)
        {
            var getItemInventoryById = await _dbContext.ItemInventories.FirstOrDefaultAsync(x => x.ItemId == itemId && x.InventoryId == inventoryId);
            if(getItemInventoryById == null)
            {
                return null;
            }
            return getItemInventoryById;
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

        public async Task<ItemInventory> GetItemInventoryByPlayer(string playerNickName, Guid itemId)
        {
            var player = await _dbContext.Players
                           .Where(e => e.Nickname == playerNickName)
                           .Include(e => e.Inventory).ThenInclude(inventory => inventory.ItemInventories).ThenInclude(x => x.Item)
                           .FirstOrDefaultAsync();

            if (player == null)
            {
                return null; // Xử lý khi không tìm thấy người chơi
            }
            else
            {
                var checkItemInventoryByItemId = await _dbContext.ItemInventories.Include(x => x.Inventory).Where(x => x.ItemId == itemId && x.InventoryId == player.Inventory.Id).FirstOrDefaultAsync();
                if (checkItemInventoryByItemId == null)
                {
                    return null;
                }
                else
                {
                    return checkItemInventoryByItemId;
                }
            }

          
        }

        public async Task<Inventory> GetItemInventoryByPlayerNotItem(string playerNickName)
        {
            var player = await _dbContext.Players
                           .Where(e => e.Nickname == playerNickName)
                           .Include(e => e.Inventory).ThenInclude(inventory => inventory.ItemInventories).ThenInclude(x => x.Item)
                           .FirstOrDefaultAsync();

            if (player == null)
            {
                return null; // Xử lý khi không tìm thấy người chơi
            }
            else
            {
                var inventory = await _dbContext.Inventories.Where(x => x.PlayerId == player.Id).FirstOrDefaultAsync();
                if(inventory != null)
                {
                    return inventory;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<GetListItemInventoryByPlayer> GetListItemInventoryByPlayer(string PlayerNickName)
        {
            var player = await _dbContext.Players
                .Where(e => e.Nickname == PlayerNickName)
                .Include(e => e.Inventory).ThenInclude(inventory => inventory.ItemInventories)
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return null; // Xử lý khi không tìm thấy người chơi
            }

            // Thay đổi ở đây, lấy danh sách Item thay vì ItemInventory theo InventoryId
            var inventoryId = player.Inventory.Id;
            if(inventoryId != Guid.Empty)
            {
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
                    ListItemInventory = _mapper.Map<List<GetItemInventoryDto>>(player.Inventory
                                        .ItemInventories)
                };

                return listItemInventoryByPlayer;
            }
            else
            {
                return null;
            }
           
        }

    
    }
}

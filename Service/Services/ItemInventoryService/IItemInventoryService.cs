using DataAccess.Dtos.InventoryDto;
using DataAccess.Dtos.ItemInventoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ItemInventoryService
{
    public interface IItemInventoryService
    {
        Task<ServiceResponse<IEnumerable<GetItemInventoryDto>>> GetItemInventory();
        Task<ServiceResponse<ItemInventoryDto>> GetItemInventoryById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewItemInventory(CreateItemInventoryDto createItemInventoryDto);
        Task<ServiceResponse<bool>> UpdateItemInventory(Guid id, UpdateItemInventoryDto ItemInventoryDto);
        Task<ServiceResponse<ItemInventoryDto>> GetItemByItemName(string itemName);

        Task<ServiceResponse<GetListItemInventoryByPlayer>> getListItemInventoryByPlayer(string PlayerNickName);
    }
}

using DataAccess.Dtos.ItemDto;
using DataAccess.Dtos.ItemInventoryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.ItemService
{
    public interface IItemService
    {
        Task<ServiceResponse<IEnumerable<ItemDto>>> GetItem();
        Task<ServiceResponse<ItemDto>> GetItemById(Guid eventId);
        Task<ServiceResponse<Guid>> CreateNewItem(CreateItemDto createItemDto);
        Task<ServiceResponse<bool>> UpdateItem(Guid id, UpdateItemDto ItemDto);

        Task<ServiceResponse<string>> DisableStausItem(Guid id);
    }
}

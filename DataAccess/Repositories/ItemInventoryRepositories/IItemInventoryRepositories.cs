using BusinessObjects.Model;
using DataAccess.Dtos.ItemInventoryDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ItemInventoryRepositories
{
    public interface IItemInventoryRepositories : IGenericRepository<ItemInventory>
    {
        Task<GetListItemInventoryByPlayer> GetListItemInventoryByPlayer(string PlayerNickName);
        Task<ItemInventoryDto> getItemByItemName(string itemName);
    }
}

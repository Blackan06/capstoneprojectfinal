using BusinessObjects.Model;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ItemRepositories
{
    public interface IItemRepository : IGenericRepository<Item>
    {
    }
}

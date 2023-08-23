using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = BusinessObjects.Model.Task;

namespace DataAccess.Repositories.TaskRepositories
{
    public interface ITaskRepositories : IGenericRepository<Task>
    {

    }
}

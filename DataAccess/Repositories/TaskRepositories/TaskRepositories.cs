using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.EventRepositories;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = BusinessObjects.Model.Task;

namespace DataAccess.Repositories.TaskRepositories
{
    public class TaskRepositories : GenericRepository<Task>, ITaskRepositories
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public TaskRepositories(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GetTaskDto> GetTaskByTaskId(Guid id)
        {
            var task = await _dbContext.Tasks.Include(x => x.Major).Where(x => x.Id == id).FirstOrDefaultAsync();

            if(task == null)
            {
                return null;
            }
            var taskDto = _mapper.Map<GetTaskDto>(task);
            return taskDto;
        }

        public async Task<bool> IsTaskUnique(string name)
        {
            return await _dbContext.Tasks.AnyAsync(q => q.Name == name) == false;
        }
    }
}

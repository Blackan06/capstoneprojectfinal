using AutoMapper;
using BusinessObjects.Model;
using DataAccess.Dtos.TaskDto;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.EventRepositories;
using Microsoft.EntityFrameworkCore;
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

      
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObjects.Model;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Enum;
using DataAccess.Exceptions;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.EventRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.EventTaskRepositories
{
    public class EventTaskRepository : GenericRepository<EventTask>, IEventTaskRepository
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public EventTaskRepository(db_a9c31b_capstoneContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventTaskDto>> GetEventTaskByEventId(Guid eventId)
        {
            var eventTasks = await _dbContext.EventTasks
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            var eventTaskDtos = _mapper.Map<IEnumerable<EventTaskDto>>(eventTasks);
            return eventTaskDtos;
        }
        public async Task<IEnumerable<GetTaskByEventIdDto>> GetTaskByEventTaskWithEventId(Guid eventId)
        {
            var getTaskAndEventDtos = await _dbContext.SchoolEvents
                .Include(se => se.Event)
                    .ThenInclude(e => e.EventTasks)
                        .ThenInclude(et => et.Task)
                            .ThenInclude(t => t.Location)
                .Include(se => se.Event)
                    .ThenInclude(e => e.EventTasks)
                        .ThenInclude(et => et.Task)
                            .ThenInclude(t => t.Major)
                .Include(se => se.Event)
                    .ThenInclude(e => e.EventTasks)
                        .ThenInclude(et => et.Task)
                            .ThenInclude(t => t.Npc)
                .Include(se => se.Event)
                    .ThenInclude(e => e.EventTasks)
                        .ThenInclude(et => et.Task)
                            .ThenInclude(t => t.Item)
                .Where(se => se.EventId == eventId)
                .SelectMany(e => e.Event.EventTasks.Select(et => new GetTaskByEventIdDto
                {
                    EventName = e.Event.Name,
                    TaskId = et.Task.Id,
                    EventtaskId = et.Id,
                    Name = et.Task.Name,
                    ItemName = et.Task.Item.Name,
                    LocationName = et.Task.Location.LocationName,
                    MajorName = et.Task.Major.Name,
                    MajorId = et.Task.MajorId,
                    NpcName = et.Task.Npc.Name,
                    Point = et.Point,
                    Status = et.Task.Status,
                    Type = et.Task.Type,
                    Priority = et.Priority,
                    Starttime = et.StartTime,
                    Endtime = et.EndTime,
                    EventSchoolStartTime = e.StartTime,
                    EventSchoolEndTime = e.EndTime
                }))
                .OrderByDescending(dto => dto.Starttime) // Sắp xếp theo thời gian tạo mới
                .ToListAsync();

            return getTaskAndEventDtos;
        }
    }
}

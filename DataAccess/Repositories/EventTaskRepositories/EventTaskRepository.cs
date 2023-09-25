using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObjects.Model;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.EventTaskDto;
using DataAccess.Dtos.SchoolEventDto;
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

        public async Task<IEnumerable<EventTaskDto>> GetEventTaskWithTypeByEventId(Guid eventId)
        {
            var eventTasks = await _dbContext.EventTasks
                           .Where(x => x.EventId == eventId)
                           .ToListAsync();

            var eventTaskDtos = _mapper.Map<IEnumerable<EventTaskDto>>(eventTasks);
            return eventTaskDtos;
        }

        public async Task<int> GetPriorityByEventTask(Guid eventId, Guid? majorId)
        {
            int maxPriorityByType = 0;

            var eventTasks = await _dbContext.EventTasks
                .Include(x => x.Task).ThenInclude(x => x.Major)
                .Where(x => x.EventId == eventId)
                .ToListAsync();
            bool checkMajor = eventTasks.Any(i => i.Task.MajorId == majorId);   
            if(checkMajor == false)
            {
                maxPriorityByType = 0;
            }

            foreach (var eventTask in eventTasks)
            {

                if (eventTask.Task != null && eventTask.Task.Major != null && eventTask.Task.Major.Id == majorId)
                {
                    maxPriorityByType = eventTask.Priority;
                }
            }

            return maxPriorityByType;
        }



        public async Task<SchoolEventDto> GetSchoolEventDto(Guid eventId)
        {
            var schoolEventdto = await _dbContext.SchoolEvents.FirstOrDefaultAsync(x => x.EventId == eventId);
            if (schoolEventdto == null)
            {
                return null;
            }

            DateTime currentTime = TimeZoneVietName(DateTime.Now); 
            if (currentTime >= schoolEventdto.StartTime && currentTime <= schoolEventdto.EndTime)
            {
                return null;
            }

            var schoolEvent = _mapper.Map<SchoolEventDto>(schoolEventdto);
            return schoolEvent;
        }

        public async Task<IEnumerable<GetTaskByEventIdDto>> GetTaskByEventTaskWithEventId(Guid eventId)
        {
            var getTaskAndEventDtos = await _dbContext.EventTasks
             .Include(et => et.Task)
                 .ThenInclude(t => t.Location)
             .Include(et => et.Task)
                 .ThenInclude(t => t.Major)
             .Include(et => et.Task)
                 .ThenInclude(t => t.Npc)
             .Include(et => et.Task)
                 .ThenInclude(t => t.Item)
             .Where(et => et.EventId == eventId)
             .Select(et => new GetTaskByEventIdDto
             {
                 EventName = et.Event.Name,
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
                 CreatedAt = et.CreatedAt
             })
             .OrderByDescending(dto => dto.CreatedAt)
             .ToListAsync();

                    return getTaskAndEventDtos;

        }

        private DateTime TimeZoneVietName(DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Lấy thời gian hiện tại theo múi giờ UTC
            DateTime utcNow = DateTime.UtcNow;

            // Chuyển múi giờ từ UTC sang múi giờ Việt Nam
            dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            return dateTime;
        }
    }
}

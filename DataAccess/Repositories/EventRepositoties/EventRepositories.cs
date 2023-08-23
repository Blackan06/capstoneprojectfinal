using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObjects.Model;
using DataAccess.Dtos.EventDto;
using DataAccess.Dtos.TaskDto;
using DataAccess.Exceptions;
using DataAccess.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.EventRepositories
{
    public class EventRepositories : GenericRepository<Event>, IEventRepositories
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public EventRepositories(db_a9c31b_capstoneContext dbContext,IMapper mapper) : base(dbContext,mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetTaskAndEventDto>> GetTaskAndEventListByTimeNow(Guid schoolId)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime vietnamTime = ConvertToVietnamTime(utcNow);
            var status = "ACTIVE";

            var query = _dbContext.SchoolEvents
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
                .Where(se => se.SchoolId == schoolId && se.StartTime <= vietnamTime && se.EndTime > vietnamTime && se.Status == status);

            var getTaskAndEventDtos = await query
                .Select(se => new GetTaskAndEventDto
                {
                    EventName = se.Event.Name,
                    TaskDtos = se.Event.EventTasks
                        .Where(et => et.Task.Status == status)
                        .Select(et => new GetTaskRequestDto
                        {
                            Id = et.Task.Id,
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
                            Starttime = et.StartTime,
                            Endtime = et.EndTime,
                        })
                        .ToList(),
                    StartTime = se.StartTime,
                    EndTime = se.EndTime
                })
                .ToListAsync();

            if (getTaskAndEventDtos.Count == 0 || getTaskAndEventDtos.Any(dto => dto.EventName == null) || getTaskAndEventDtos.Any(dto => dto.TaskDtos.Count() == 0))
            {
                return Enumerable.Empty<GetTaskAndEventDto>();
            }

            return getTaskAndEventDtos;
        }


        public DateTime ConvertToVietnamTime(DateTime dateTimeUtc)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, vietnamTimeZone);
            return vietnamTime;
        }
    }
} 



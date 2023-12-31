﻿using AutoMapper;
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

        public async Task<GetTaskAndEventDto> GetTaskAndEventListByTimeNow(Guid schoolId)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime vietnamTime = ConvertToVietnamTime(utcNow);
            var status = "ACTIVE";
            var schoolEvent = await _dbContext.SchoolEvents
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
                    .Where(se => se.SchoolId == schoolId && se.StartTime <= vietnamTime && se.EndTime > vietnamTime && se.Status == status)
                    .FirstOrDefaultAsync();

            if (schoolEvent == null)
            {
                return null; 
            }
            var getTaskAndEventDto = new GetTaskAndEventDto
            {
                EventName = schoolEvent.Event.Name,
                TaskDtos = schoolEvent.Event.EventTasks
                        .Where(et => et.Task.Status == status)
                        .Select(et => new GetTaskRequestDto
                        {
                            Id = et.Task.Id,
                            EventtaskId = et.Id,
                            Name = et.Task.Name,
                            LocationName = et.Task.Location != null ? et.Task.Location.LocationName : null,
                            MajorName = et.Task.Major != null ? et.Task.Major.Name : null,
                            ItemId = et.Task.ItemId,
                            ItemName = et.Task.Item != null ? et.Task.Item.Name : null,
                            MajorId = et.Task.MajorId,
                            NpcName = et.Task.Npc != null ? et.Task.Npc.Name : null,
                            Point = et.Point,
                            Status = et.Task.Status,
                            Type = et.Task.Type,
                            Starttime = et.StartTime,
                            Endtime = et.EndTime,
                            Priority = et.Priority
                        })
                        .ToList().OrderBy(x => x.Priority),
                StartTime = schoolEvent.StartTime,
                EndTime = schoolEvent.EndTime
            };

            if (getTaskAndEventDto.EventName == null || getTaskAndEventDto.TaskDtos.Count() == 0)
            {
                return null;
            }
            return getTaskAndEventDto;
        }

        public async Task<List<string>> GetExistingNamesAsync(List<string> namesToCheck)
        {
            // Thực hiện truy vấn để lấy danh sách các tên từ cơ sở dữ liệu
            var existingNames = await _dbContext.Events
                .Where(e => namesToCheck.Contains(e.Name))
                .Select(e => e.Name)
                .ToListAsync();

            return existingNames;
        }
        public DateTime ConvertToVietnamTime(DateTime dateTimeUtc)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, vietnamTimeZone);
            return vietnamTime;
        }
    }
} 



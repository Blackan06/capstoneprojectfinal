using BusinessObjects.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Service.UpdateStatusHandler
{
    public class StatusUpdateScheduler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _updateInterval;

        public StatusUpdateScheduler(IServiceScopeFactory serviceScopeFactory, TimeSpan updateInterval)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _updateInterval = updateInterval;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<db_a9c31b_capstoneContext>();

                    var currentTime = DateTime.UtcNow;
                   
                    var activeEventSchools = await dbContext.SchoolEvents.Include(x => x.School).ThenInclude(x => x.Students).ThenInclude(x => x.Player)
                                                    .Where(a => a.EndTime <= TimeZoneVietName(currentTime) && a.Status != "INACTIVE")
                                                    .ToListAsync();


                    foreach (var schoolEvent in activeEventSchools)
                    {
                        schoolEvent.Status = "INACTIVE";
                        await dbContext.SaveChangesAsync();
                    }

                }

                await Task.Delay(_updateInterval, stoppingToken); // Khoảng thời gian cập nhật
            }
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

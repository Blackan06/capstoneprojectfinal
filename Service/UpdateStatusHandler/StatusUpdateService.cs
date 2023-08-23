using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.UpdateStatusHandler
{
    public class StatusUpdateService : BackgroundService
    {
        private readonly StatusUpdateScheduler _statusUpdateScheduler;

        public StatusUpdateService(StatusUpdateScheduler statusUpdateScheduler)
        {
            _statusUpdateScheduler = statusUpdateScheduler;
        }

         protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _statusUpdateScheduler.StartAsync(stoppingToken);
        }

    }
}

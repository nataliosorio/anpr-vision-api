using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.BackgroundTasks
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("📌 Worker de cola iniciado...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                if (workItem == null)
                {
                    continue;
                }

                // 🔹 Disparar en paralelo
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await workItem(stoppingToken);
                        _logger.LogDebug("✅ Tarea en segundo plano ejecutada correctamente.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error ejecutando tarea en segundo plano.");
                    }
                }, stoppingToken);
            }

            _logger.LogInformation("🛑 Worker de cola detenido.");
        }
    }
}

namespace iVectorOne.Importer
{
    using Intuitive;
    using Intuitive.Scheduling;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class OwnStockPropertyImportService : IHostedService
    {
        private readonly IScheduleManager _manager;
        private readonly ISchedulingOptionsService _options;
        private readonly ILogger<OwnStockPropertyImportService> _logger;

        public OwnStockPropertyImportService(
            IScheduleManager manager,
            ISchedulingOptionsService options,
            ILogger<OwnStockPropertyImportService> logger)
        {
            _manager = Ensure.IsNotNull(manager, nameof(manager));
            _options = Ensure.IsNotNull(options, nameof(options));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await NewMethod(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("Error scheduling Import, " + e.Message, e);
            }
        }

        private async Task NewMethod(CancellationToken cancellationToken)
        {
            var options = _options.GetJobOptions(typeof(OwnStockPropertyImportJob));
            foreach (var triggerOptions in options.Triggers)
            {
                if (triggerOptions.Trigger != TriggerType.None)
                {
                    await _manager.ScheduleAsync<OwnStockPropertyImportJob>(
                            options.Name,
                            triggerOptions.Trigger,
                            triggerOptions.Cron,
                            triggerOptions.Interval,
                            options.Description,
                            options.AllowConcurrentExecution,
                            new Dictionary<string, object>
                            {
                                ["options"] = triggerOptions
                            },
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
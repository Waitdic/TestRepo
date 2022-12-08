namespace iVectorOne.Content.Jobs
{
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Scheduling;
    using Microsoft.Extensions.Logging;

    [ScheduledJob("Reset Search Logging", trigger: TriggerType.Cron, cron: "0 0 * ? * * *")]
    public class ResetSeachLogging : IScheduledJob
    {
        private readonly ISql _sql;
        private readonly ILogger<ResetSeachLogging> _logger;

        public ResetSeachLogging(ISql sql, ILogger<ResetSeachLogging> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task ExecuteAsync(IScheduledJobContext context)
        {
            try
            {
                await _sql.ExecuteAsync(
                    "Search_SwitchLoggingOff",
                    new CommandSettings()
                        .IsStoredProcedure());
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}
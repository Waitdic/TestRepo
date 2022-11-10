namespace iVectorOne.Content.Jobs
{
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Scheduling;
    using Microsoft.Extensions.Logging;

    [ScheduledJob("iVectorOne Import", trigger: TriggerType.Once)]
    public class iVectorOneImportJob : IScheduledJob
    {
        private readonly ISql _sql;
        private readonly ILogger<iVectorOneImportJob> _logger;

        public iVectorOneImportJob(ISql sql, ILogger<iVectorOneImportJob> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task ExecuteAsync(IScheduledJobContext context)
        {
            try
            {
                await _sql.ExecuteAsync(
                    "IVectorOne_ImportProperties",
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
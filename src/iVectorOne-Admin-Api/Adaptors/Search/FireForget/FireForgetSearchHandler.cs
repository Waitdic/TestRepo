using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search;

namespace iVectorOne_Admin_Api.Adaptors.Search.FireForget
{
    public class FireForgetSearchHandler : IFireForgetSearchHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<FireForgetSearchHandler> _logger;

        public FireForgetSearchHandler(IServiceScopeFactory serviceScopeFactory, ILogger<FireForgetSearchHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public void Execute(Func<IAdaptor<Request, Response>, Task> propertySearch)
        {
            // Fire off the task, but don't await the result
            Task.Run(async () =>
            {
                // Exceptions must be caught
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var adaptor = scope.ServiceProvider.GetRequiredService<IAdaptor<Request, Response>>();
                    await propertySearch(adaptor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error executing fire and forget search request.");
                }
            });
        }
    }
}

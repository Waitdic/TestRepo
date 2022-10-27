using iVectorOne_Admin_Api.Adaptors;
using iVectorOne_Admin_Api.Adaptors.Search;

namespace iVectorOne_Admin_Api.Adaptors.Search.FireForget
{
    public class FireForgetSearchHandler : IFireForgetSearchHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FireForgetSearchHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}

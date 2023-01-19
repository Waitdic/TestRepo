namespace iVectorOne.Services
{
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.ExtraContent;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A service for interacting with the repository
    /// </summary>
    public class ExtraContentService : IExtraContentService
    {
        private IExtraRepository _extraRepository;
        public ExtraContentService(IExtraRepository extraRepository)
        {
            _extraRepository = extraRepository;
        }

        /// <inheritdoc />
        public async Task<Response> GetAllExtras(Request request)
        {
            Response response = new Response();

            try
            {
                if (AccountHasSupplier(request))
                {
                    var extras = await _extraRepository.GetAllExtras(request.Supplier);
                    if (extras != null && extras.Count > 0)
                    {
                        response.Extras = extras;
                    }
                }
                else
                {
                    throw new Exception($"The supplier {request.Supplier} is not a supplier associated with your account.");
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.Message);
            }

            return response;
        }
        private bool AccountHasSupplier(Request searchRequest)
          => searchRequest.Account.Configurations.Where(c => c.Supplier.ToLower() == searchRequest.Supplier.ToLower()).Any();

    }
}

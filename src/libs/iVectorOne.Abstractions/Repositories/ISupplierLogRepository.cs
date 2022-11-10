namespace iVectorOne.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;

    public interface ISupplierLogRepository
    {
        Task LogSearchRequestsAsync(SearchDetails searchDetails, int supplierId, List<Request> requests);
        Task LogPrebookRequestsAsync(PropertyDetails propertyDetails);
        Task LogBookRequestsAsync(PropertyDetails propertyDetails);
        Task LogPrecancelRequestsAsync(PropertyDetails propertyDetails);
        Task LogCancelRequestsAsync(PropertyDetails propertyDetails);
    }
}
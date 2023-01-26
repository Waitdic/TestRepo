namespace iVectorOne.Services
{
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.LocationContent;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LocationContentService : ILocationContentService
    {
        private readonly ILocationRepository _locationRepository;
        public LocationContentService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        public async Task<Response> GetAllLocations(Request locationRequest)
        {
            Response response = new Response();

            try
            {
                if (AccountHasSupplier(locationRequest))
                {
                    var locations = await _locationRepository.GetAllLocations(locationRequest.Supplier);
                    if (locations != null && locations.Count>0)
                    {
                        response.Locations = locations;
                    }
                }
                else
                {
                    response.Warnings.Add($"The supplier {locationRequest.Supplier} is not a supplier associated with your account.");
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.ToString());
            }

            return response;
        }

        private bool AccountHasSupplier(Request searchRequest)
            => searchRequest.Account.Configurations.Where(c => c.Supplier.ToLower() == searchRequest.Supplier.ToLower()).Any();
    }
}

using Intuitive;
using iVectorOne.Lookups;
using iVectorOne.Repositories;
using iVectorOne.Search.Models;
using System.Collections.Generic;
using System.Linq;

namespace iVectorOne.Services.Transfer
{
    public class LocationManagerService : ILocationManagerService
    {
        private readonly ITransferSearchRepository _transferSearchRepository;
        private readonly ITPSupport _tpSupport;
        public LocationManagerService(ITransferSearchRepository transferSearchRepository, ITPSupport tpSupport)
        {
            _transferSearchRepository = Ensure.IsNotNull(transferSearchRepository, nameof(transferSearchRepository));
            _tpSupport = Ensure.IsNotNull(tpSupport, nameof(tpSupport));
        }

        public async void CheckLocations(List<string> uniqueLocationList, TransferSearchDetails searchDetails, string locationCode)
        {
            uniqueLocationList = uniqueLocationList.Select(x => $"{locationCode}: {x}").ToList();
            List<string> currentLocations = await _tpSupport.TPAllLocationLookup(searchDetails.Source);
            List<string> newLocations = uniqueLocationList.Except(currentLocations).ToList();
            if (newLocations.Any())
            {
                await _transferSearchRepository.AddLocations(searchDetails.Source, newLocations);
                _tpSupport.RemoveLocationCache(searchDetails.Source);

            }

        }
    }
}

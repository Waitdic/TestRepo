namespace iVectorOne.Suppliers.Polaris
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using System.Threading.Tasks;
    using iVectorOne.Suppliers.Polaris.Models;
    using System.Text;
    using Newtonsoft.Json;

    public class Polaris : IThirdParty, ISingleSource
    {
        #region "Properties"
        public string Source => ThirdParties.POLARIS;

        private readonly IPolarisSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ISecretKeeper _secretKeeper;
        private readonly ILogger<Polaris> _logger;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        #endregion

        #region "Constructors"
        public Polaris(
            IPolarisSettings settings,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ILogger<Polaris> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "PreBook"
        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var preBookSuccess = false;
            var requests = new List<Request>();

            try
            {
                bool isSplit = _settings.SplitMultiRoom(propertyDetails);

                var polarisTpRefs = propertyDetails.Rooms.Where((room, idx) => isSplit || idx == 0)
                                .Select(room => PolarisTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference));

                var prebookResponses = new List<PrebookResponse>();

                foreach (var polarisTpRef in polarisTpRefs) 
                {
                    var prebookContent = new PrebookRequest
                    {
                        BookToken = polarisTpRef.BookToken
                    };

                    var preBookUrl = _settings.PrebookURL(propertyDetails);
                    var preBookRequest = await CreateRequestAsync(propertyDetails, preBookUrl, prebookContent);
                    requests.Add(preBookRequest);
                    var prebookReponse = JsonConvert.DeserializeObject<PrebookResponse>(preBookRequest.ResponseString);
                    prebookResponses.Add(prebookReponse);
                }
                //check status
                //verify price
                //add cancellations with solidify
                //add erratum
                preBookSuccess = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                preBookSuccess = false;
            }
            finally 
            {
                foreach (var request in requests) 
                {
                    propertyDetails.AddLog("Prebook", request);
                }
            }

            return preBookSuccess;
        }

        public async Task<Request> CreateRequestAsync<T>(PropertyDetails propertyDetail, string endpoint, T requestContent)
        {
            var requestString = JsonConvert.SerializeObject(requestContent);
            var request = new Request()
            {
                EndPoint = endpoint,
                Method = RequestMethod.POST,
                Source = Source,
                ContentType = ContentTypes.Application_json,
                UseGZip = _settings.UseGZip(propertyDetail)
            };
            request.SetRequest(requestString);
            request.Headers.AddNew("Authorization", "Basic " + BasicToken(propertyDetail, _settings));

            await request.Send(_httpClient, _logger);

            return request;
        }

        #endregion

        #region "Book"
        public Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region "Cancellations"
        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region "Other Funcitons"
        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }
        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }
        #endregion

        #region "Helpers"

        public static List<Cancellation> TransformCancellations(List<CancellationPolicy> cancellations)
        {
            var canxs = cancellations.Select(cancellation => new Cancellation
            {
                StartDate = cancellation.From.ToSafeDate(),
                EndDate = cancellation.To.ToSafeDate(),
                Amount = cancellation.Pricing.Net.Price
            }).ToList();
            return canxs;
        }

        internal static string BasicToken(IThirdPartyAttributeSearch tpAttributeSearch, IPolarisSettings settings)
        {
            return Convert.ToBase64String(new UTF8Encoding().GetBytes(
                $"{settings.User(tpAttributeSearch)}:{settings.Password(tpAttributeSearch)}"));
        }
        #endregion
    }
}

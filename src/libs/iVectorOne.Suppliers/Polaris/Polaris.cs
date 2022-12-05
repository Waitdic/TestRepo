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
            bool isSplit = _settings.SplitMultiRoom(propertyDetails);

            if (isSplit) return await PreBookSplitAsync(propertyDetails);

            var preBookSuccess = false;
            var request = new Request();

            try
            {
                var prebookResponses = new List<PrebookResponse>();

                var tpRef = PolarisTpRef.Decrypt(_secretKeeper, propertyDetails.Rooms.First().ThirdPartyReference);

                var preBookRequest = await CreateRequestAsync(propertyDetails, _settings.PrebookURL(propertyDetails), 
                    new PrebookRequest 
                    { 
                        BookToken = tpRef.BookToken 
                    });

                var prebookResponse = JsonConvert.DeserializeObject<PrebookResponse>(preBookRequest.ResponseString);
                var roomRate = prebookResponse.Hotel.RoomRates.First();

                //check status
                if (!string.Equals(prebookResponse.Status, Constant.Status.Confirmed)) 
                {
                    throw new Exception($"PreBook status is {prebookResponse.Status}");
                }

                //verify price
                var isPriceChanged = false;
                var roomIdx = 0;
                foreach (var room in roomRate.Rooms.OrderBy(x => x.Index)) 
                {
                    if (!decimal.Equals(room.Pricing.Net.Price, propertyDetails.Rooms[roomIdx].LocalCost))
                    {
                        propertyDetails.Rooms[roomIdx].LocalCost = room.Pricing.Net.Price;
                        propertyDetails.Rooms[roomIdx].GrossCost = room.Pricing.Net.Price;
                    }
                    roomIdx++;
                }
                if (isPriceChanged) 
                {
                    propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                    propertyDetails.GrossCost = propertyDetails.Rooms.Sum(r => r.GrossCost);
                }

                //add cancellations with solidify
                var canx = new Cancellations();
                canx.AddRange(TransformCancellations(roomRate.CancellationPolicies));

                propertyDetails.Cancellations = canx;

                //add erratum
                foreach (var observation in roomRate.Observations) 
                {
                    propertyDetails.Errata.Add(new Erratum 
                    {
                        Title = "Important information",
                        Text = $"{observation.Type} {observation.Txt}"
                    });
                }

                preBookSuccess = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                preBookSuccess = false;
            }
            finally 
            {
                propertyDetails.AddLog("Prebook", request);
            }

            return preBookSuccess;
        }



        public async Task<bool> PreBookSplitAsync(PropertyDetails propertyDetails) 
        {
            var preBookSuccess = false;
            var requests = new List<Request>();
            var prebookResponses = new List<PrebookResponse>();

            try
            {
                var isPriceChanged = false;
                var canx = new Cancellations();
                var observations = new List<Observation>();

                foreach (var room in propertyDetails.Rooms) 
                {
                    var polarisTpRef = PolarisTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference);
                    var prebookContent = new PrebookRequest
                    {
                        BookToken = polarisTpRef.BookToken
                    };

                    var preBookRequest = await CreateRequestAsync(propertyDetails, _settings.PrebookURL(propertyDetails), prebookContent);
                    var prebookResponse = JsonConvert.DeserializeObject<PrebookResponse>(preBookRequest.ResponseString);
                    var roomRate = prebookResponse.Hotel.RoomRates.First();

                    if (!string.Equals(prebookResponse.Status, Constant.Status.Confirmed)) 
                    {
                        throw new Exception($"PreBook status is {prebookResponse.Status}");
                    }

                    var roomPrice = roomRate.Pricing.Net.Price;
                    if (!Equals(room.LocalCost, roomPrice)) 
                    {
                        isPriceChanged = true;
                        room.LocalCost = roomPrice;
                        room.GrossCost = roomPrice;
                    }

                    var cancellations = TransformCancellations(roomRate.CancellationPolicies);
                    canx.AddRange(cancellations);

                    observations.AddRange(roomRate.Observations);
                }
                if (isPriceChanged)
                {
                    propertyDetails.LocalCost = propertyDetails.Rooms.Sum(x => x.LocalCost);
                    propertyDetails.GrossCost = propertyDetails.Rooms.Sum(x => x.GrossCost);
                }

                propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(canx);

                //add erratum
                var information = observations.Select(x => $"{x.Type} {x.Txt}").Distinct();
                foreach (var observation in information)
                {
                    propertyDetails.Errata.Add(new Erratum
                    {
                        Title = "Important Information",
                        Text = observation
                    });
                }

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

        #endregion

        #region "Book"
        public Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> BookSplitAsync(PropertyDetails propertyDetails) 
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

namespace iVectorOne.Suppliers.GoGlobal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.GoGlobal.Models;
    using Cancellation = iVectorOne.Models.Cancellation;
    using Newtonsoft.Json;
    using iVectorOne.Models.Property;

    public partial class GoGlobalSearch : IThirdPartySearch, ISingleSource
    {
        #region "Properties"

        private IGoGlobalSettings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.GOGLOBAL;

        #endregion

        #region "Constructors"

        public GoGlobalSearch(IGoGlobalSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region "SearchRestrictions"

        public bool SearchRestrictions(SearchDetails oSearchDetails, string source)
        {
            if (oSearchDetails.Rooms > 1) return true;

            // Supplier Restrictions
            // ALL rooms in a request must have Adults
            // The maximum amount of persons (adults + children), per room, is 8
            // The limit of children, per room, is 4
            // Children are between ages 1 to 18 when using version 2.2 and above(Legacy support ages 2 to 10
            bool bRestricted = oSearchDetails.RoomDetails.Any(oRoom =>
            {
                if (oRoom.Adults == 0) return true;
                if (oRoom.Adults + oRoom.Children > 8) return true;
                if (oRoom.ChildAges.Any(age => age < 1 && age > 18)) return true;
                return false;
            });

            return bRestricted;
        }

        #endregion

        #region "SearchFunctions"

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var oRequests = new List<Request>();

            string sNationalityCode = string.IsNullOrEmpty(oSearchDetails.ISONationalityCode)
                ? _settings.LeadGuestNationality(oSearchDetails)
                : oSearchDetails.ISONationalityCode;

            var TPKeyBatches = oResortSplits.SelectMany(split => split.Hotels.Select(h => h.TPKey))
                                .Distinct().Batch(_settings.HotelBatchLimit(oSearchDetails)).ToList();

            foreach (var TPKeyBatch in TPKeyBatches)
            {

                int idxRoom = 0;
                foreach (var oRoom in oSearchDetails.RoomDetails)
                {
                    idxRoom++;

                    var infantChild = Enumerable.Range(0, oRoom.Infants).Select(_ => "1");
                    var childAges = oRoom.ChildAges.Select(age => $"{age}").Concat(infantChild).ToList();

                    //'Build the request
                    var oRequestData = new SearchRq
                    {

                        Version = Constant.Version_2_3,
                        ResponseFormat = Constant.FormatJSON,
                        Hotels = TPKeyBatch.ToList(),
                        Nationality = sNationalityCode,
                        ArrivalDate = oSearchDetails.ArrivalDate.ToString(Constant.DataFormat),
                        Nights = oSearchDetails.Duration.ToString(),
                        Rooms =
                        {
                            new Room
                            {
                                Adults = $"{oRoom.Adults + oRoom.ChildAges.Count(age => age > Constant.MaxChildAge)}",
                                RoomCount = $"1",
                                ChildCount = $"{oRoom.Infants + oRoom.Children}",
                                ChildAges = oRoom.ChildAges.Where(age => age <= Constant.MaxChildAge).Select(age => $"{age}")
                                            .Concat(Enumerable.Range(0, oRoom.Infants).Select(i => "1")).ToList()
                            }
                        },
                    };

                    var oRoot = new Root<SearchRq>
                    {
                        Header =
                        {
                             Agency = _settings.Agency(oSearchDetails),
                             User = _settings.User(oSearchDetails),
                             Password = _settings.Password(oSearchDetails),
                             Operation = Constant.RequestHotelSearch,
                             OperationType = Constant.OperationTypeRequest,
                        },
                        Main = oRequestData
                    };

                    string sRoot = $"{_serializer.Serialize(oRoot).OuterXml}";
                    var envelope = Envelope.CreateRequest(Constant.RequestCodeHotelSearch, sRoot, _serializer);

                    //'Make the web request
                    var oWebRequest = new Request
                    {
                        EndPoint = _settings.GenericURL(oSearchDetails),
                        Method = RequestMethod.POST,
                        Source = Source,
                        LogFileName = "Search",
                        CreateLog = true,
                        ExtraInfo = $"{idxRoom}",
                        UseGZip = true,
                    };
                    oWebRequest.SetRequest(envelope);

                    oRequests.Add(oWebRequest);
                }
            }

            return Task.FromResult(oRequests);
        }

        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            foreach (var oRequest in oRequests)
            {
                string response = Envelope.GetResponse(oRequest.ResponseXML, _serializer);
                var searchResult = JsonConvert.DeserializeObject<SearchRs>(response);
                int roomIdx = oRequest.ExtraInfo.ToSafeInt();

                var transfomedRooms = searchResult.Hotels.SelectMany(oHotel =>
                {
                    return oHotel.Offers.SelectMany(offer =>
                    {
                        decimal nRate = offer.TotalPrice.ToSafeDecimal();
                        var cancellationList = new Cancellations();

                        var cancellations = GoGlobal.GetCancellationPeriods(offer.Remark, oSearchDetails.ArrivalDate)
                                .Select(c => new Cancellation
                                {
                                    StartDate = c.StartDate,
                                    EndDate = c.EndDate.AddDays(-1),
                                    Amount = nRate * c.FeePercent / 100
                                });

                        cancellationList.AddRange(cancellations);

                        return offer.Rooms.Select((sRoomType, idx) =>
                        {
                            return new TransformedResult
                            {
                                MasterID = oHotel.HotelCode.ToSafeInt(),
                                PropertyRoomBookingID = roomIdx,
                                MealBasisCode = offer.RoomBasis,
                                TPKey = oHotel.HotelCode,
                                CurrencyCode = offer.Currency,
                                RoomType = sRoomType,
                                Amount = nRate,
                                TPReference = $"{offer.HotelSearchCode}",
                                NonRefundableRates = offer.NonRef,
                                Cancellations = cancellationList
                            };
                        });
                    });
                }).ToList();
                transformedResults.TransformedResults.AddRange(transfomedRooms);
            }
            
            return transformedResults;
        }

        #endregion

        #region "ResponseHasExceptions"

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion
    }
}

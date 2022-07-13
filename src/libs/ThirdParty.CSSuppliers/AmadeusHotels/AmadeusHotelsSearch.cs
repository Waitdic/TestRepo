namespace ThirdParty.CSSuppliers.AmadeusHotels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Models;
    using Models.Common;
    using Models.Header;
    using Models.Soap;
    using Support;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class AmadeusHotelsSearch : IThirdPartySearch, ISingleSource, IPagedResultSearch
    {
        private readonly ISerializer _serializer;
        private readonly IAmadeusHotelsSettings _settings;

        public AmadeusHotelsSearch(IAmadeusHotelsSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.AMADEUSHOTELS;

        public int MaxPages(SearchDetails searchDetails) => _settings.MaxPages(searchDetails);

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var sentResortCodes = new HashSet<string>();

            foreach (var resort in resortSplits)
            {
                string resortCode = resort.ResortCode.Split('|')[0];

                if (!sentResortCodes.Contains(resortCode))
                {
                    if (_settings.SplitMultiRoom(searchDetails))
                    {
                        for (int i = 0; i < searchDetails.RoomDetails.Count; i++)
                        {
                            BuildPagedRequest(searchDetails, requests, resort, searchDetails.RoomDetails[i], i + 1);
                        }
                    }
                    else
                    {
                        BuildPagedRequest(searchDetails, requests, resort, searchDetails.RoomDetails[0], 1);
                    }

                }
                if (_settings.SearchMode(searchDetails).ToLower() == "radius" || resort.Hotels.Count > 1)
                {
                    // if searching by radius and not multiple hotels, we should prevent duplicate requests with the same code being sent.
                    sentResortCodes.Add(resortCode);
                }
            }

            return Task.FromResult(requests);
        }

        private void BuildPagedRequest(SearchDetails searchDetails, List<Request> requests, ResortSplit resort, RoomDetail roomDetails, int roomId)
        {
            var pageTokenKey = new AmadeusPagingTokenKey(resort, roomId);

            searchDetails.PagingTokenCollector.NextPageTokens.TryGetValue(pageTokenKey, out string foundPagingToken);

            if (!searchDetails.PagingTokenCollector.NextPageTokens.Any() || !string.IsNullOrWhiteSpace(foundPagingToken))
            {
                var webRequest = BuildWebRequest(searchDetails, resort, roomDetails, 1, roomId, foundPagingToken, pageTokenKey, true);
                requests.Add(webRequest);
            }
        }

        private Request BuildWebRequest(
            SearchDetails searchDetails,
            IResortSplit resort,
            RoomDetail roomDetail,
            int roomCount,
            int propertyRoomBookingId,
            string foundPagingToken,
            AmadeusPagingTokenKey pageTokenKey,
            bool isStateful)
        {
            const string soapAction = AmadeusHotelsSoapActions.HotelMultiSingleHotelSoapAction;

            var request = BuildHotelMultiSingleAvailabilityRequest(searchDetails, resort, roomDetail, roomCount, foundPagingToken, isStateful);

            var amadeusExtraInfo = new AmadeusSearchHelper(
                pageTokenKey,
                propertyRoomBookingId,
                roomDetail.Adults,
                roomDetail.Children,
                searchDetails.Duration);

            var webRequest = new Request
                {
                    EndPoint = _settings.URL(searchDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Text_xml,
                    ExtraInfo = amadeusExtraInfo,
                    SOAP = true,
                    TimeoutInSeconds = 300,
                    SoapAction = soapAction
                };

            webRequest.SetRequest(request);

            return webRequest;
        }

        private XmlDocument BuildHotelMultiSingleAvailabilityRequest(
            SearchDetails searchDetails,
            IResortSplit resort,
            RoomDetail roomDetail,
            int roomCount,
            string foundPagingToken,
            bool isStateful)
        {
            var request = new Envelope<OTAHotelAvailRQ>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    searchDetails,
                    AmadeusHotelsSoapActions.HotelMultiSingleHotelSoapAction,
                    isStateful),
                Body = { Content =
                {
                    RateRangeOnly = true,
                    RateDetailsInd = true,
                    Version = "4",
                    EchoToken = "MultiSingle",
                    AvailRatesOnly = true,
                    SummaryOnly = true,
                    SortOrder = "PA",
                    SearchCacheLevel = _settings.SearchCacheLevel(searchDetails),
                    AvailRequestSegments =
                    {
                        AvailRequestSegment =
                        {
                            MoreDataEchoToken = foundPagingToken,
                            InfoSource = "Distribution",
                            HotelSearchCriteria =
                            {
                                AvailableOnlyIndicator = true,
                                Criterion =
                                {
                                    ExactMatch = true,
                                    StayDateRange =
                                    {
                                        End = $"{searchDetails.DepartureDate:yyyy-MM-dd}",
                                        Start = $"{searchDetails.ArrivalDate:yyyy-MM-dd}"
                                    },
                                    RoomStayCandidates = new RoomStayCandidates
                                    {
                                        RoomStayCandidate = new[]
                                        {
                                            new RoomStayCandidate
                                            {
                                                RoomID = 1,
                                                Quantity = roomCount,
                                                GuestCounts =
                                                {
                                                    GuestCount =
                                                    {
                                                        AgeQualifyingCode = "10",
                                                        Count = roomDetail.Adults + roomDetail.Children
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }}
            };

            var criterion = request.Body.Content.AvailRequestSegments.AvailRequestSegment.HotelSearchCriteria.Criterion;

            resort.ResortCode = resort.ResortCode.Split('|')[0];
            string searchMode = _settings.SearchMode(searchDetails);
            bool searchWithHotelList = searchMode == "negotiated" && resort.Hotels.Count > 1;

            if (resort.Hotels.Count == 1)
            {
                string[] hotelItems = resort.Hotels[0].TPKey.Split('_');

                criterion.HotelRef.ChainCode = hotelItems[0];
                criterion.HotelRef.HotelCode = hotelItems[1];
                criterion.HotelRef.HotelCityCode = resort.ResortCode;
            }
            else if (searchMode == "radius")
            {
                criterion.CodeRef.CodeContext = "IATA";
                criterion.CodeRef.LocationCode = resort.ResortCode;
            }
            else if (!searchWithHotelList)
            {
                criterion.HotelRef.HotelCityCode = resort.ResortCode;
            }

            return _serializer.Serialize(request);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<EnvelopeResponse<OTAHotelAvailRS>>(_serializer.CleanXmlNamespaces(request.ResponseXML));

            var searchHelper = (AmadeusSearchHelper)requests[0].ExtraInfo;

            transformedResults.TransformedResults.AddRange(responses.SelectMany(r => GetResultFromResponse(r.Body.Content, searchHelper, searchDetails)));

            return transformedResults;
        }

        private IEnumerable<TransformedResult> GetResultFromResponse(OTAHotelAvailRS response, AmadeusSearchHelper searchHelper, SearchDetails searchDetails)
        {
            var transformedResults = new List<TransformedResult>();

            searchDetails.PagingTokenCollector.NextPageTokens.Add(searchHelper.PagingTokenKey, response.RoomStays.MoreIndicator);

            foreach (var hotelStay in response.HotelStays)
            {
                string tpKey = $"{hotelStay.BasicPropertyInfo.ChainCode}_{hotelStay.BasicPropertyInfo.HotelCode}";
                int duration = searchHelper.Duration;
                int adults = searchHelper.Adults;
                int children = searchHelper.Children;
                int propertyRoomBookingId = searchHelper.PropertyRoomBookingID;

                foreach (string roomStayRph in hotelStay.RoomStayRph.Split(' '))
                {
                    var roomStay = response.RoomStays.RoomStayList.First(r => r.Rph == roomStayRph);
                    var roomRate = roomStay.RoomRates.First();
                    var rate = roomRate.Rates.First();
                    string ratePlanCode = roomRate.RatePlanCode;
                    var ratePlan = roomStay.RatePlans.First();

                    if (!_settings.ExcludePackageRates(searchDetails)
                        || _settings.ExcludePackageRates(searchDetails) && ratePlanCode != "PKG")
                    {
                        //bool commissionable = ratePlan.Commission.StatusType == "Commissionable";
                        //decimal percent = ratePlan.Commission.Percent;

                        string totalCurrencyCode = roomRate.Total.CurrencyCode;
                        string baseCurrencyCode = rate.Base.CurrencyCode;

                        decimal totalAmountBeforeTax = roomRate.Total.AmountBeforeTax;
                        decimal totalAmountAfterTax = roomRate.Total.AmountAfterTax;
                        decimal baseAmountBeforeTax = rate.Base.AmountBeforeTax;
                        decimal baseAmountAfterTax = rate.Base.AmountAfterTax;
                        string rateTimeUnit = rate.Base.RateTimeUnit;

                        // 2019/08/29 Amadeus send the rates in the wrong order to the cost so the total is wrong.
                        // It is correct if only one rate is returned
                        // If any changes are made here please check the PreBook Function GetTotalCost
                        int rateCount = roomStay.RoomRates.Sum(r => r.Rates.Length);

                        decimal totalAmount = 0;

                        if (totalAmountAfterTax != 0)
                        {
                            totalAmount = totalAmountAfterTax;
                        }
                        else if (totalAmountBeforeTax != 0)
                        {
                            totalAmount = totalAmountBeforeTax;
                        }
                        else if (rateCount == 1 && (rateTimeUnit == "Day") || string.IsNullOrEmpty(rateTimeUnit))
                        {
                            if (baseAmountBeforeTax != 0)
                            {
                                totalAmount = baseAmountBeforeTax * duration;
                            }
                            else if (baseAmountAfterTax != 0)
                            {
                                totalAmount = baseAmountAfterTax * duration;
                            }
                        }

                        string currencyCode = string.IsNullOrEmpty(totalCurrencyCode) ? baseCurrencyCode : totalCurrencyCode;
                        //decimal commission = commissionable && percent != 0 ? percent : 0;

                        string mealBasisCode = ratePlan.MealsIncluded.MealPlanCodes;
                        if (string.IsNullOrEmpty(mealBasisCode)) mealBasisCode = "14";

                        var roomStayType = roomStay.RoomTypes?.FirstOrDefault();
                        string roomTypeCode = roomStayType is { IsConverted: true } 
                            ? roomStayType.RoomTypeAttribute 
                            : roomStay.RoomRates.First().RoomTypeCode;

                        string roomType = string.Join("  ", roomStay.RoomRates.SelectMany(r => r.RoomRateDescription.Text));
                        int availableRooms = roomRate.NumberOfUnits;
                        string thirdPartyReference = $"{roomRate.BookingCode}|{ratePlanCode}|{roomStayType?.IsConverted}|{mealBasisCode}";
                        decimal cancellationAmount = ratePlan.CancelPenalties?.FirstOrDefault()?.AmountPercent?.Amount ?? 0;
                        bool nonRefundable = totalAmountAfterTax == cancellationAmount;

                        transformedResults.Add(new TransformedResult()
                        {
                            TPKey = tpKey,
                            CurrencyCode = currencyCode,
                            Amount = totalAmount,
                            RoomType = roomType,
                            MealBasisCode = mealBasisCode,
                            RoomTypeCode = roomTypeCode,
                            TPReference = thirdPartyReference,
                            TPRateCode = ratePlanCode,
                            Adults = adults,
                            Children = children,
                            PropertyRoomBookingID = propertyRoomBookingId,
                            AvailableRooms = availableRooms,
                            NonRefundableRates = nonRefundable
                        });
                    }
                }
            }

            return transformedResults;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            if (_settings.SplitMultiRoom(searchDetails))
            {
                return restrictions;
            }

            int occupancy = searchDetails.RoomDetails[0].Adults + searchDetails.RoomDetails[0].Children;

            if (searchDetails.Rooms > 9)
            {
                restrictions = true;
            }

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                if (roomDetail.Adults > 9)
                {
                    restrictions = true;
                }

                // only allows multiple identical rooms with same occupancy
                if (roomDetail.Adults + roomDetail.Children != occupancy)
                {
                    restrictions = true;
                }
            }

            return restrictions;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}

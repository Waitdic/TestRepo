namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using iVectorOne.Transfer;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    public abstract class TourPlanTransfersBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ISerializer _serializer;
        public const string InvalidSupplierReference = "Invalid Supplier Reference";

        public TourPlanTransfersBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger,
            ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public async Task<bool> PreBookAsync(TransferDetails transferDetails)
        {
            var requests = new List<Request>();
            try
            {
                var supplierReferenceData = SplitSupplierReference(transferDetails);

                var request = BuildOptionInfoRequest(transferDetails, supplierReferenceData.First(), transferDetails.DepartureDate);

                requests.Add(request);

                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML))
                {
                    var deserializedResponse = DeSerialize<OptionInfoReply>(request.ResponseXML);

                    if (IsValidResponse(deserializedResponse, supplierReferenceData.First().Opt))
                    {
                        transferDetails.LocalCost = deserializedResponse.Option[0].OptStayResults.TotalPrice;
                        transferDetails.ISOCurrencyCode = deserializedResponse.Option[0].OptStayResults.Currency;
                        transferDetails.SupplierReference = CreateSupplierReference(deserializedResponse.Option[0].Opt, deserializedResponse.Option[0].OptStayResults.RateId);
                        AddCancellation(deserializedResponse, transferDetails);

                        if (!transferDetails.OneWay)
                        {
                            var returnRequest = BuildOptionInfoRequest(transferDetails, supplierReferenceData.Last(), transferDetails.ReturnDate);

                            requests.Add(returnRequest);

                            await returnRequest.Send(_httpClient, _logger);
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML))
                            {
                                var deserializedReturnResponse = DeSerialize<OptionInfoReply>(returnRequest.ResponseXML);

                                if (IsValidResponse(deserializedReturnResponse, supplierReferenceData.Last().Opt))
                                {
                                    transferDetails.LocalCost += deserializedReturnResponse.Option[0].OptStayResults.TotalPrice;
                                    transferDetails.SupplierReference = CreateSupplierReference(deserializedResponse.Option[0].Opt, deserializedResponse.Option[0].OptStayResults.RateId, deserializedReturnResponse.Option[0].Opt, deserializedReturnResponse.Option[0].OptStayResults.RateId);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (ArgumentException ex)
            {
                transferDetails.Warnings.AddNew("ArgumentException", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                transferDetails.Warnings.AddNew("PrebookException", ex.Message);
                return false;
            }
            finally
            {
                foreach (var request in requests)
                {
                    transferDetails.AddLog("Prebook", request);
                }
            }
        }

        public async Task<string> BookAsync(TransferDetails transferDetails)
        {
            var requests = new List<Request>();
            try
            {
                string refValue = string.Empty;
                var supplierReferenceData = SplitSupplierReference(transferDetails);

                var request = await BuildRequestAsync(transferDetails, supplierReferenceData.First(),
                    transferDetails.DepartureDate, transferDetails.DepartureTime);

                requests.Add(request);

                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML))
                {
                    var deserializedResponse = DeSerialize<AddServiceReply>(request.ResponseXML);

                    if (deserializedResponse != null &&
                        string.Equals(deserializedResponse.Status.ToUpper(), "OK"))
                    {
                        transferDetails.ConfirmationReference = deserializedResponse.BookingId.ToSafeString();
                        transferDetails.LocalCost = deserializedResponse.Services.Service.LinePrice;
                        refValue = deserializedResponse.Ref;

                        if (!transferDetails.OneWay)
                        {
                            var returnRequest = await BuildRequestAsync(transferDetails, supplierReferenceData.Last(),
                                                transferDetails.ReturnDate, transferDetails.ReturnTime);

                            requests.Add(returnRequest);

                            await returnRequest.Send(_httpClient, _logger);
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML))
                            {
                                var deserializedReturnResponse = DeSerialize<AddServiceReply>(returnRequest.ResponseXML);

                                if (deserializedReturnResponse != null &&
                                    string.Equals(deserializedReturnResponse.Status.ToUpper(), "OK"))
                                {
                                    transferDetails.ConfirmationReference += $"|{deserializedReturnResponse.BookingId.ToSafeString()}";
                                    transferDetails.LocalCost += deserializedReturnResponse.Services.Service.LinePrice;
                                    refValue += $"|{deserializedReturnResponse.Ref}";
                                }
                                else
                                {
                                    return "failed";
                                }
                            }
                        }
                        return refValue;
                    }
                    else
                    {
                        return "failed";
                    }
                }


                return "failed";
            }
            catch (ArgumentException ex)
            {
                transferDetails.Warnings.AddNew("ArgumentException", ex.Message);
                return "failed";
            }
            catch (Exception ex)
            {
                transferDetails.Warnings.AddNew("BookException", ex.Message);
                return "failed";
            }
            finally
            {
                foreach (var request in requests)
                {
                    transferDetails.AddLog("Book", request);
                }

            }
        }
        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }
        private class SupplierReferenceData
        {
            public string Opt { get; set; }
            public string RateId { get; set; }
        }
        private List<SupplierReferenceData> SplitSupplierReference(TransferDetails transferDetails)
        {
            List<SupplierReferenceData> result = new List<SupplierReferenceData>();

            if (transferDetails.OneWay)
            {
                string[] supplierReferenceValues = transferDetails.SupplierReference.Split("-");
                if (supplierReferenceValues.Length != 2)
                {
                    throw new ArgumentException(InvalidSupplierReference);
                }
                result.Add(new SupplierReferenceData { Opt = supplierReferenceValues[0], RateId = supplierReferenceValues[1] });
            }
            else
            {
                string[] legSupplierReferences = transferDetails.SupplierReference.Split("|");
                if (legSupplierReferences.Length != 2)
                {
                    throw new ArgumentException(InvalidSupplierReference);
                }
                foreach (string legSupplierReference in legSupplierReferences)
                {
                    string[] supplierReferenceValues = legSupplierReference.Split("-");

                    if (supplierReferenceValues.Length != 2)
                    {
                        throw new ArgumentException(InvalidSupplierReference);
                    }
                    result.Add(new SupplierReferenceData { Opt = supplierReferenceValues[0], RateId = supplierReferenceValues[1] });
                }

            }
            return result;
        }

        private async Task<Request> BuildRequestAsync(TransferDetails transferDetails,
            SupplierReferenceData supplierReferenceData, DateTime departureDate, string departureTime)
        {
            var bookingData = BuildBookingDataAsync(transferDetails, supplierReferenceData.Opt,
                supplierReferenceData.RateId, departureDate, departureTime);

            var request = new Request()
            {
                EndPoint = _settings.URL(transferDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml
            };
            var xmlDocument = Serialize(await bookingData);
            request.SetRequest(xmlDocument);
            return request;
        }

        private Task<AddServiceRequest> BuildBookingDataAsync(TransferDetails transferDetails,
            string opt, string rateId, DateTime departureDate, string departureTime)
        {
            var addServiceRequest = new AddServiceRequest()
            {
                AgentID = _settings.AgentId(transferDetails),
                Password = _settings.Password(transferDetails),
                Opt = opt,
                RateId = rateId,
                ExistingBookingInfo = "",
                DateFrom = departureDate.ToString("yyyy-MM-dd"),
                NewBookingInfo = new NewBookingInformation
                {
                    Name = $"{transferDetails.LeadGuestFirstName} {transferDetails.LeadGuestLastName}"
                },
                RoomConfigs = new RoomConfigurations()
                {
                    RoomConfig = new RoomConfiguration()
                    {
                        Adults = transferDetails.Adults,
                        Children = transferDetails.Children,
                        Infants = transferDetails.Infants,
                        PaxList = transferDetails.Passengers.Select(x => new PaxDetails
                        {
                            Title = x.Title,
                            Forename = x.FirstName,
                            Surname = x.LastName,
                            PaxType = x.PassengerType.ToString().ToCharArray()[0].ToString()
                        }).ToList()
                    }
                },
                PickUp_Date = departureDate.ToString("yyyy-MM-dd"),
                PuTime = departureTime,
                PuRemark = ""
            };

            return Task.FromResult(addServiceRequest);
        }

        public XmlDocument Serialize(object request)
        {
            var xmlRequest = _serializer.SerializeWithoutNamespaces(request);
            xmlRequest.InnerXml = $"<Request>{xmlRequest.InnerXml}</Request>";
            return xmlRequest;
        }
        public T DeSerialize<T>(XmlDocument xmlDocument) where T : class
        {
            var xmlResponse = _serializer.CleanXmlNamespaces(xmlDocument);
            xmlResponse.InnerXml = xmlResponse.InnerXml.Replace("<Reply>", "").Replace("</Reply>", "");
            return _serializer.DeSerialize<T>(xmlResponse);
        }

        private bool ResponseHasError(TransferDetails transferDetails, XmlDocument responseXML)
        {
            if (responseXML.OuterXml.Contains("<ErrorReply>"))
            {
                var errorResponseObj = DeSerialize<ErrorReply>(responseXML);
                transferDetails.Warnings.AddNew("Book Exception", string.IsNullOrEmpty(errorResponseObj.Error) ?
                    "Failed to confirm booking" : errorResponseObj.Error);
                return true;
            }
            return false;
        }

        private Request BuildOptionInfoRequest(TransferDetails transferDetails, SupplierReferenceData supplierReferenceData, DateTime dateFrom)
        {
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {

                AgentID = _settings.AgentId(transferDetails),
                Password = _settings.Password(transferDetails),
                DateFrom = dateFrom.ToString(Constant.DateTimeFormat),
                Info = Constant.Info,
                Opt = supplierReferenceData.Opt,
                RoomConfigs = new List<RoomConfiguration>()
                {
                   new RoomConfiguration() {
                   Adults = transferDetails.Adults,
                   Children = transferDetails.Children,
                   Infants = transferDetails.Infants
                   }
                }
            };

            var request = new Request()
            {
                EndPoint = _settings.URL(transferDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml

            };
            var xmlDocument = Serialize(optionInfoRequest);
            request.SetRequest(xmlDocument);

            return request;
        }

        // To do : Remove this method, when helper method will be created. 
        private string CreateSupplierReference(string outBoundOpt, string outBoundRateId, string returnOpt = "", string returnRateId = "")
        {
            var reference = outBoundOpt + "-" + outBoundRateId;
            if (!string.IsNullOrEmpty(returnOpt))
            {
                reference += "|" + returnOpt + "-" + returnRateId;
            }
            return reference;
        }

        private bool IsValidResponse(OptionInfoReply response, string opt)
        {
            return (response != null &&
                    response.Option != null &&
                    response.Option.Count == 1 &&
                    response.Option[0].Opt == opt);
        }

        private void AddCancellation(OptionInfoReply deserializedResponse, TransferDetails transferDetails)
        {
            var cancelPolicies = deserializedResponse.Option[0].OptStayResults.CancelPolicies;
            if (cancelPolicies != null)
            {
                transferDetails.Cancellations = GetCancellationFromCancelPolicies(cancelPolicies, transferDetails.DepartureDate);
            }
        }

        private Cancellations GetCancellationFromCancelPolicies(CancelPolicies cancelPolicies, DateTime departureDate)
        {
            var cancellations = new Cancellations();
            var cancelPenalties = cancelPolicies.CancelPenalty;

            foreach (var cancelPenalty in cancelPenalties)
            {
                var deadlineDate = cancelPenalty.Deadline.DeadlineDateTime;
                var timeUnit = cancelPenalty.Deadline.OffsetTimeUnit;
                var timeValue = cancelPenalty.Deadline.OffsetUnitMultiplier;
                var linePrice = cancelPenalty.LinePrice;

                if (linePrice != null)
                {
                    decimal amount = decimal.Parse(linePrice) / 100m;

                    if (deadlineDate != null)
                    {
                        var deadlineDateTime = DateTime.Parse(deadlineDate);
                        cancellations.AddNew(deadlineDateTime, departureDate, amount);
                    }
                    else if (timeUnit != null && timeValue != null)
                    {
                        TimeSpan timeOffset = GetCancellationOffset(timeUnit, timeValue);
                        cancellations.AddNew(departureDate.Add(timeOffset), departureDate, amount);
                    }
                }
            }

            cancellations.Solidify(SolidifyType.LatestStartDate);
            return cancellations;
        }

        private TimeSpan GetCancellationOffset(string timeUnitNode, string timeValueNode)
        {
            string timeUnit = timeUnitNode;
            decimal timeValue = decimal.Parse(timeValueNode);

            switch (timeUnit ?? "")
            {
                case "Hour":
                    {
                        return TimeSpan.FromHours((double)-timeValue);
                    }
                case "Day":
                    {
                        return TimeSpan.FromDays((double)-timeValue);
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(timeUnit), $"Unrecognised cancellation OffsetTimeUnit value ({timeUnit})");
                    }
            }

        }

    }
}

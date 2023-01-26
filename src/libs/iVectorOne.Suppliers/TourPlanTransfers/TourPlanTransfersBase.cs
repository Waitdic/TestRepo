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
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    public abstract class TourPlanTransfersBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

        private readonly ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ISerializer _serializer;

        public TourPlanTransfersBase(
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger,
            ISerializer serializer)
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _settings = new InjectedTourPlanTransfersSettings();
        }

        public bool ValidateSettings(TransferDetails transferDetails)
        {
            if (!_settings.SetThirdPartySettings(transferDetails.ThirdPartySettings))
            {
                transferDetails.Warnings.AddRange(_settings.GetWarnings());
                return false;
            }
            return true;
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

                if (!ResponseHasError(transferDetails, request.ResponseXML, Constant.PrebookException))
                {
                    var deserializedResponse = Helpers.DeSerialize<OptionInfoReply>(request.ResponseXML, _serializer);

                    if (IsValidResponse(deserializedResponse, supplierReferenceData.First().Opt))
                    {
                        transferDetails.LocalCost = deserializedResponse.Option[0].OptStayResults.TotalPrice / 100m;
                        transferDetails.ISOCurrencyCode = deserializedResponse.Option[0].OptStayResults.Currency;
                        transferDetails.SupplierReference = Helpers.CreateSupplierReference(deserializedResponse.Option[0].Opt, deserializedResponse.Option[0].OptStayResults.RateId);
                        AddErrata(deserializedResponse.Option[0].OptionNotes, transferDetails, true);
                        AddCancellation(deserializedResponse, transferDetails, transferDetails.DepartureDate);

                        if (!transferDetails.OneWay)
                        {
                            var returnRequest = BuildOptionInfoRequest(transferDetails, supplierReferenceData.Last(), transferDetails.ReturnDate);

                            requests.Add(returnRequest);

                            await returnRequest.Send(_httpClient, _logger);
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML, Constant.PrebookException))
                            {
                                var deserializedReturnResponse = Helpers.DeSerialize<OptionInfoReply>(returnRequest.ResponseXML, _serializer);

                                if (IsValidResponse(deserializedReturnResponse, supplierReferenceData.Last().Opt))
                                {
                                    transferDetails.LocalCost += deserializedReturnResponse.Option[0].OptStayResults.TotalPrice / 100m;
                                    transferDetails.SupplierReference = Helpers.CreateSupplierReference(deserializedResponse.Option[0].Opt, deserializedResponse.Option[0].OptStayResults.RateId, deserializedReturnResponse.Option[0].Opt, deserializedReturnResponse.Option[0].OptStayResults.RateId);
                                    AddErrata(deserializedReturnResponse.Option[0].OptionNotes, transferDetails, false);
                                    AddCancellation(deserializedReturnResponse, transferDetails, transferDetails.DepartureDate);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (transferDetails.Cancellations != null && transferDetails.Cancellations.Count > 0)
                        {
                            transferDetails.Cancellations.Solidify(SolidifyType.LatestStartDate, useMinutes: true);
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
            string refValue = string.Empty;
            try
            {
                var supplierReferenceData = SplitSupplierReference(transferDetails);

                var request = await BuildRequestAsync(transferDetails,
                              supplierReferenceData.First(),
                              transferDetails.DepartureDate,
                              transferDetails.DepartureTime,
                              transferDetails.OutboundPickUpDetails,
                              transferDetails.OutboundDropoffDetails);

                requests.Add(request);

                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML, Constant.BookException))
                {
                    var deserializedResponse = Helpers.DeSerialize<AddServiceReply>(request.ResponseXML, _serializer);

                    if (deserializedResponse != null && CheckFreesaleOnRequestStatus(deserializedResponse.Status))
                    {
                        transferDetails.ConfirmationReference = deserializedResponse.BookingId.ToSafeString();
                        transferDetails.LocalCost = deserializedResponse.Services.Service.LinePrice.DivideBy100M();
                        refValue = deserializedResponse.Ref;

                        if (!transferDetails.OneWay)
                        {
                            var returnRequest = await BuildRequestAsync(transferDetails,
                                                supplierReferenceData.Last(),
                                                transferDetails.ReturnDate,
                                                transferDetails.ReturnTime,
                                                transferDetails.ReturnPickUpDetails,
                                                transferDetails.ReturnDropOffDetails);

                            requests.Add(returnRequest);

                            await returnRequest.Send(_httpClient, _logger);
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML, Constant.BookException))
                            {
                                var deserializedReturnResponse = Helpers.DeSerialize<AddServiceReply>(returnRequest.ResponseXML, _serializer);

                                if (deserializedReturnResponse != null && CheckFreesaleOnRequestStatus(deserializedResponse.Status))
                                {
                                    transferDetails.ConfirmationReference += $"|{deserializedReturnResponse.BookingId.ToSafeString()}";
                                    transferDetails.LocalCost += deserializedReturnResponse.Services.Service.LinePrice.DivideBy100M();
                                    refValue += $"|{deserializedReturnResponse.Ref}";
                                }
                                else
                                {
                                    await CancelBookingAsync(transferDetails, refValue);
                                    return "failed";
                                }
                            }
                            else
                            {
                                await CancelBookingAsync(transferDetails, refValue);
                                return "failed";
                            }
                        }
                        return refValue;
                    }
                }
                return "failed";
            }
            catch (ArgumentException ex)
            {
                transferDetails.Warnings.AddNew("ArgumentException", ex.Message);
                await CancelBookingAsync(transferDetails, refValue);
                return "failed";
            }
            catch (Exception ex)
            {
                transferDetails.Warnings.AddNew("BookException", ex.Message);
                await CancelBookingAsync(transferDetails, refValue);
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

        private async Task CancelBookingAsync(TransferDetails transferDetails, string supplierReferenceToCancel)
        {
            if (string.IsNullOrEmpty(supplierReferenceToCancel))
                return;

            string tempSupplierReference = transferDetails.SupplierReference;

            transferDetails.SupplierReference = supplierReferenceToCancel;

            await CancelBookingAsync(transferDetails);

            transferDetails.SupplierReference = tempSupplierReference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(TransferDetails transferDetails)
        {
            var requests = new List<Request>();
            var tpCancellationResponse = new ThirdPartyCancellationResponse() { Success = false, TPCancellationReference = "failed" };
            try
            {
                string[] supplierReferenceData = transferDetails.SupplierReference.Split('|');
                if (supplierReferenceData.Length == 0 ||
                    supplierReferenceData.Length > 2)
                {
                    throw new ArgumentException(Constant.InvalidSupplierReference);
                }

                bool firstBookingCancelStatus = false, secondBookingCancelStatus = false;

                var request = BuildCancellationRequestAsync(transferDetails, supplierReferenceData[0]);
                requests.Add(request);
                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML, Constant.CancelException))
                {
                    var deserializedResponse = Helpers.DeSerialize<CancelServicesReply>(request.ResponseXML, _serializer);

                    if (CancellationSuccessful(deserializedResponse))
                    {
                        firstBookingCancelStatus = true;
                    }
                }

                if (supplierReferenceData.Length > 1)
                {
                    var returnCancellationRequest = BuildCancellationRequestAsync(transferDetails, supplierReferenceData[1]);
                    requests.Add(returnCancellationRequest);
                    await returnCancellationRequest.Send(_httpClient, _logger);

                    if (!ResponseHasError(transferDetails, returnCancellationRequest.ResponseXML, Constant.CancelException))
                    {
                        var deserializedReturnCancellationResponse = Helpers.DeSerialize<CancelServicesReply>(returnCancellationRequest.ResponseXML, _serializer);

                        if (CancellationSuccessful(deserializedReturnCancellationResponse))
                        {
                            secondBookingCancelStatus = true;
                        }
                    }
                }

                if (firstBookingCancelStatus && (supplierReferenceData.Length == 1 || secondBookingCancelStatus))
                {
                    tpCancellationResponse.Success = true;
                    tpCancellationResponse.TPCancellationReference = transferDetails.SupplierReference;
                }
                return tpCancellationResponse;
            }
            catch
            {
                return tpCancellationResponse;
            }
            finally
            {

                foreach (var request in requests)
                {
                    transferDetails.AddLog("Cancellation", request);
                }
            }
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(TransferDetails transferDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellation;
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
                    throw new ArgumentException(Constant.InvalidSupplierReference);
                }
                result.Add(new SupplierReferenceData { Opt = supplierReferenceValues[0], RateId = supplierReferenceValues[1] });
            }
            else
            {
                string[] legSupplierReferences = transferDetails.SupplierReference.Split("|");
                if (legSupplierReferences.Length != 2)
                {
                    throw new ArgumentException(Constant.InvalidSupplierReference);
                }
                foreach (string legSupplierReference in legSupplierReferences)
                {
                    string[] supplierReferenceValues = legSupplierReference.Split("-");

                    if (supplierReferenceValues.Length != 2)
                    {
                        throw new ArgumentException(Constant.InvalidSupplierReference);
                    }
                    result.Add(new SupplierReferenceData { Opt = supplierReferenceValues[0], RateId = supplierReferenceValues[1] });
                }

            }
            return result;
        }

        private Request BuildCancellationRequestAsync(TransferDetails transferDetails, string supplierReference)
        {
            var cancellationData = new CancelServicesRequest
            {
                AgentID = _settings.AgentID,
                Password = _settings.Password,
                Ref = supplierReference
            };

            var request = new Request
            {
                EndPoint = _settings.URL,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml
            };
            var xmlDocument = Helpers.Serialize(cancellationData, _serializer);
            request.SetRequest(xmlDocument);
            return request;
        }

        private async Task<Request> BuildRequestAsync(TransferDetails transferDetails,
            SupplierReferenceData supplierReferenceData, 
            DateTime departureDate, 
            string departureTime, 
            JourneyDetails pickUpDetails, 
            JourneyDetails dropOffDetails)
        {
            var bookingData = BuildBookingDataAsync(transferDetails,
                              supplierReferenceData.Opt,
                              supplierReferenceData.RateId,
                              departureDate,
                              departureTime,
                              pickUpDetails,
                              dropOffDetails);

            var request = new Request()
            {
                EndPoint = _settings.URL,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml
            };
            var xmlDocument = Helpers.Serialize(await bookingData, _serializer);
            request.SetRequest(xmlDocument);
            return request;
        }

        private Task<AddServiceRequest> BuildBookingDataAsync(TransferDetails transferDetails,
            string opt, 
            string rateId, 
            DateTime departureDate, 
            string departureTime,
            JourneyDetails pickUpDetails, 
            JourneyDetails dropOffDetails)
        {
            var addServiceRequest = new AddServiceRequest()
            {
                AgentID = _settings.AgentID,
                Password = _settings.Password,
                Opt = opt,
                RateId = rateId,
                ExistingBookingInfo = "",
                DateFrom = departureDate.ToString(Constant.DateTimeFormat),
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
                PickUp_Date = departureDate.ToString(Constant.DateTimeFormat),
                PuTime = departureTime,
                PuRemark = BuildPickUpRemarks(pickUpDetails),
                DoRemark = BuildDropOffRemarks(dropOffDetails)
            };

            return Task.FromResult(addServiceRequest);
        }
        private bool ResponseHasError(TransferDetails transferDetails, XmlDocument responseXML,
            Warning warning)
        {
            if (responseXML.OuterXml.Contains("<ErrorReply>"))
            {

                var errorResponseObj = Helpers.DeSerialize<ErrorReply>(responseXML, _serializer);
                transferDetails.Warnings.AddNew(warning.Title, string.IsNullOrEmpty(errorResponseObj.Error) ?
                    warning.Text : errorResponseObj.Error);
                return true;
            }
            else if (responseXML.OuterXml.Contains("<Error>"))
            {
                transferDetails.Warnings.AddNew(warning.Title, warning.Text);
                return true;
            }
            return false;
        }

        private bool CancellationSuccessful(CancelServicesReply deserializedResponse)
        {
            if (deserializedResponse != null &&
                        string.Equals(deserializedResponse.ServiceStatuses.ServiceStatus.Status.ToUpper(), "XX"))
            {
                return true;
            }
            return false;
        }

        private Request BuildOptionInfoRequest(TransferDetails transferDetails, SupplierReferenceData supplierReferenceData, DateTime dateFrom)
        {
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {

                AgentID = _settings.AgentID,
                Password = _settings.Password,
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
                EndPoint = _settings.URL,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml

            };
            var xmlDocument = Helpers.Serialize(optionInfoRequest, _serializer);
            request.SetRequest(xmlDocument);

            return request;
        }
        private bool IsValidResponse(OptionInfoReply response, string opt)
        {
            return (response != null &&
                    response.Option != null &&
                    response.Option.Count == 1 &&
                    response.Option[0].Opt == opt);
        }

        private void AddErrata(OptionNotes optionNotes, TransferDetails transferDetails, bool outbound)
        {
            foreach (var note in optionNotes.OptionNote)
            {
                if (!_settings.ExcludeNoteCategory.Contains(note.NoteCategory.ToLower()))
                {
                    if (outbound)
                    {
                        transferDetails.DepartureErrata.AddNew(note.NoteCategory, note.NoteText);
                    }
                    else
                    {
                        transferDetails.ReturnErrata.AddNew(note.NoteCategory, note.NoteText);
                    }
                }
            }
        }

        private void AddCancellation(OptionInfoReply deserializedResponse, TransferDetails transferDetails, DateTime date)
        {
            var cancelPolicies = deserializedResponse.Option[0].OptStayResults.CancelPolicies;
            if (cancelPolicies != null)
            {
                transferDetails.Cancellations.AddRange(GetCancellationFromCancelPolicies(cancelPolicies, date));
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
                DateTime deadlineDateTime = DateTime.MinValue;
                if (linePrice != null)
                {
                    decimal amount = decimal.Parse(linePrice).DivideBy100M();

                    if (deadlineDate != null)
                    {
                        deadlineDateTime = DateTime.Parse(deadlineDate);
                    }
                    else if (timeUnit != null && timeValue != null)
                    {
                        TimeSpan timeOffset = GetCancellationOffset(timeUnit, timeValue);
                        deadlineDateTime = departureDate.Add(timeOffset);
                    }

                    if (deadlineDateTime != DateTime.MinValue && deadlineDateTime < departureDate)
                    {
                        cancellations.AddNew(deadlineDateTime, departureDate, amount);
                    }
                }
            }

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

        private string BuildPickUpRemarks(JourneyDetails journeyDetails)
        {
            if (!string.IsNullOrEmpty(journeyDetails.FlightCode))
            {
                return $"Collect from Airport. Flight number: {journeyDetails.FlightCode}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.AccommodationName))
            {
                return $"Collection from hotel: {journeyDetails.AccommodationName}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.TrainDetails))
            {
                return $"Collect from Station: {journeyDetails.TrainDetails}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.VesselName))
            {
                return $"Collect from Port, Vessel name: {journeyDetails.VesselName}";
            }
            else
            {
                return $"Exact collection point to be determined";
            }
        }
        private string BuildDropOffRemarks(JourneyDetails journeyDetails)
        {
            if (!string.IsNullOrEmpty(journeyDetails.FlightCode))
            {
                return $"Drop off to Airport, Flight number: {journeyDetails.FlightCode}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.AccommodationName))
            {
                return $"Dropping off at hotel: {journeyDetails.AccommodationName}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.TrainDetails))
            {
                return $"Drop off to Station: {journeyDetails.TrainDetails}";
            }
            else if (!string.IsNullOrEmpty(journeyDetails.VesselName))
            {
                return $"Drop off to Port, Vessel name: {journeyDetails.VesselName}";
            }
            else
            {
                return $"Exact drop off point to be determined";
            }
        }

        private bool CheckFreesaleOnRequestStatus(string availability)
        {
            return (availability.ToUpper() == Constant.FreesaleCode || availability.ToUpper() == Constant.OnRequestCode);
        }

    }
}

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

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        private readonly ISerializer _serializer;
        public const string InvalidSupplierReference = "Invalid Supplier Reference";

        //TODO:move these constants to the Constants class once refactoring is done
        public static readonly Warning BookException = new Warning("BookException", "Failed to confirm booking");
        public static readonly Warning CancelException = new Warning("CancelException", "Failed to cancel bookng");

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

        public Task<bool> PreBookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> BookAsync(TransferDetails transferDetails)
        {
            var requests = new List<Request>();
            string refValue = string.Empty;
            try
            {
                var supplierReferenceData = SplitSupplierReference(transferDetails);

                var request = await BuildRequestAsync(transferDetails, supplierReferenceData.First(),
                    transferDetails.DepartureDate, transferDetails.DepartureTime);

                requests.Add(request);

                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML, BookException))
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
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML, BookException))
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
                    throw new ArgumentException(InvalidSupplierReference);
                }

                bool firstBookingCancelStatus = false, secondBookingCancelStatus = false;

                var request = BuildCancellationRequestAsync(transferDetails, supplierReferenceData[0]);
                requests.Add(request);
                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML, CancelException))
                {
                    var deserializedResponse = DeSerialize<CancelServicesReply>(request.ResponseXML);

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

                    if (!ResponseHasError(transferDetails, returnCancellationRequest.ResponseXML, CancelException))
                    {
                        var deserializedReturnCancellationResponse = DeSerialize<CancelServicesReply>(returnCancellationRequest.ResponseXML);

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
            return _settings.AllowCancellations(searchDetails);
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

        private Request BuildCancellationRequestAsync(TransferDetails transferDetails, string supplierReference)
        {
            var cancellationData = new CancelServicesRequest
            {
                AgentID = _settings.AgentId(transferDetails),
                Password = _settings.Password(transferDetails),
                Ref = supplierReference
            };

            var request = new Request
            {
                EndPoint = _settings.URL(transferDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml
            };
            var xmlDocument = Serialize(cancellationData);
            request.SetRequest(xmlDocument);
            return request;
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

        private bool ResponseHasError(TransferDetails transferDetails, XmlDocument responseXML,
            Warning warning)
        {
            if (responseXML.OuterXml.Contains("<ErrorReply>"))
            {

                var errorResponseObj = DeSerialize<ErrorReply>(responseXML);
                transferDetails.Warnings.AddNew(warning.Title, string.IsNullOrEmpty(errorResponseObj.Error) ?
                    warning.Text : errorResponseObj.Error);
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
    }
}

﻿namespace iVectorOne.Suppliers.TourPlanTransfers
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
        private enum exceptionTypes
        {
            BookException,
            CancelException
        };
        private readonly Dictionary<exceptionTypes, string> exceptionTypeDescriptions =
            new Dictionary<exceptionTypes, string>();

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
            exceptionTypeDescriptions.Add(exceptionTypes.BookException, "Failed to confirm booking");
            exceptionTypeDescriptions.Add(exceptionTypes.CancelException, "Failed to cancel booking");
        }

        public Task<bool> PreBookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
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

                if (!ResponseHasError(transferDetails, request.ResponseXML, exceptionTypes.BookException))
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
                            if (!ResponseHasError(transferDetails, returnRequest.ResponseXML, exceptionTypes.BookException))
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

                var request = await BuildCancellationRequestAsync(transferDetails, supplierReferenceData[0]);

                requests.Add(request);

                await request.Send(_httpClient, _logger);

                if (!ResponseHasError(transferDetails, request.ResponseXML, exceptionTypes.CancelException))
                {
                    var deserializedResponse = DeSerialize<CancelServicesReply>(request.ResponseXML);

                    if (deserializedResponse != null &&
                        string.Equals(deserializedResponse.ServiceStatuses.ServiceStatus.Status.ToUpper(), "XX")) //First Booking cancel success
                    {
                        if (supplierReferenceData.Length > 1)
                        {
                            var returnCancellationRequest = await BuildCancellationRequestAsync(transferDetails, supplierReferenceData[1]);

                            requests.Add(returnCancellationRequest);

                            await returnCancellationRequest.Send(_httpClient, _logger);

                            if (!ResponseHasError(transferDetails, returnCancellationRequest.ResponseXML,
                                exceptionTypes.CancelException))
                            {
                                var deserializedReturnCancellationResponse = DeSerialize<CancelServicesReply>(returnCancellationRequest.ResponseXML);

                                if (deserializedReturnCancellationResponse != null &&
                                    string.Equals(deserializedReturnCancellationResponse.ServiceStatuses.ServiceStatus.Status.ToUpper(), "XX")) //Second Booking cancel success
                                {
                                    tpCancellationResponse.Success = true;
                                    tpCancellationResponse.TPCancellationReference = transferDetails.SupplierReference;
                                }
                            }
                        }
                        else
                        {
                            tpCancellationResponse.Success = true;
                            tpCancellationResponse.TPCancellationReference = supplierReferenceData[0];
                        }
                    }
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

        private Task<Request> BuildCancellationRequestAsync(TransferDetails transferDetails, string supplierReference)
        {
            var calcellationData = new CancelServicesRequest
            {
                AgentID = _settings.AgentId(transferDetails),
                Password = _settings.Password(transferDetails),
                Ref = supplierReference
            };

            var request = new Request
            {
                EndPoint = _settings.URL(transferDetails), //Change this to calcellationURL
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml
            };
            var xmlDocument = Serialize(calcellationData);
            request.SetRequest(xmlDocument);
            return Task.FromResult(request);
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
            exceptionTypes requestExceptionType)
        {
            if (responseXML.OuterXml.Contains("<ErrorReply>"))
            {

                var errorResponseObj = DeSerialize<ErrorReply>(responseXML);
                transferDetails.Warnings.AddNew(requestExceptionType.ToString(), string.IsNullOrEmpty(errorResponseObj.Error) ?
                    exceptionTypeDescriptions[requestExceptionType] : errorResponseObj.Error);
                return true;
            }
            return false;
        }
    }
}

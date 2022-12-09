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
    using System.Xml.Serialization;

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
    }
}

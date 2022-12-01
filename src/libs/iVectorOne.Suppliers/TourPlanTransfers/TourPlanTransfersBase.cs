namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Transfer;
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;
    using Intuitive;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using iVectorOne.SDK.V2;
    using System;
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using iVectorOne.Suppliers.Xml.W2M;
    using Newtonsoft.Json;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive.Helpers.Serialization;
    using NodaTime.TimeZones;
    using System.Globalization;
    using iVectorOne.Suppliers.GoGlobal.Models;
    using System.Xml.Linq;
    using iVectorOne.Suppliers.Netstorming.Models.Common;
    using iVectorOne.Models.Property.Booking;

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
            var request = new Request();
            bool isFirstLeg = true;
            try
            {
                var supplierReferenceData = SplitSupplierReference(transferDetails);

                request = await BuildRequestAsync(transferDetails, supplierReferenceData, isFirstLeg);

                await request.Send(_httpClient, _logger);

                // IF Request succeeds 
                // SET isFirstLeg = false;
                // CREATE second leg request 
                // SEND second leg request

                // ELSE IF Request Fails
                // CALL Cancel workflow

                return "failed";
            }
            catch (ArgumentException ex)
            {
                transferDetails.Warnings.AddNew("ArgumentException", ex.Message);
                return "failed";
            }
            finally
            {
                //Here as well based on isFirstLeg flag, we can log the requests.
                transferDetails.AddLog("Book", request);
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
            List<SupplierReferenceData> supplierReferenceData, bool isFirstLeg)
        {
            Tuple<SupplierReferenceData, string, string> dynamicLegValues = new Tuple<SupplierReferenceData, string, string>(
                (transferDetails.OneWay && isFirstLeg) ? supplierReferenceData.First() : supplierReferenceData.Last(),
                ((transferDetails.OneWay && isFirstLeg) ? transferDetails.DepartureDate : transferDetails.ReturnDate).ToString("yyyy-MM-dd"),
                (transferDetails.OneWay && isFirstLeg) ? transferDetails.DepartureTime : transferDetails.ReturnTime
                );

            var bookingData = BuildBookingDataAsync(transferDetails, dynamicLegValues);

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
            Tuple<SupplierReferenceData, string, string> dynamicLegValues)
        {
            var addServiceRequest = new AddServiceRequest()
            {
                AgentID = _settings.AgentId(transferDetails),
                Password = _settings.Password(transferDetails),
                Opt = dynamicLegValues.Item1.Opt,
                RateId = dynamicLegValues.Item1.RateId,
                ExistingBookingInfo = "",
                DateFrom = dynamicLegValues.Item2,
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
                PickUp_Date = dynamicLegValues.Item2,
                PuTime = dynamicLegValues.Item3,
                PuRemark = ""
            };

            return Task.FromResult(addServiceRequest);
        }

        public XmlDocument Serialize(AddServiceRequest request)
        {
            var xmlRequest = _serializer.SerializeWithoutNamespaces(request);
            xmlRequest.InnerXml = $"<Request>{xmlRequest.InnerXml}</Request>";
            return xmlRequest;
        }

    }
}

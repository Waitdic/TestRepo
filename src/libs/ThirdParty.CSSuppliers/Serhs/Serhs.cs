namespace ThirdParty.CSSuppliers.Serhs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using Models;
    using Models.Common;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;

    public class Serhs : IThirdParty
    {
        private readonly ISerhsSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Serhs> _logger;

        public Serhs(ISerhsSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<Serhs> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.SERHS;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool preBookSuccess = true;
            var request = new Request();

            try
            {
                var baseHelper = new TpReference(propertyDetails.Rooms[0].ThirdPartyReference);

                string version = _settings.Version(propertyDetails);
                string clientCode = _settings.ClientCode(propertyDetails);
                string password = _settings.Password(propertyDetails);
                string branch = _settings.Branch(propertyDetails);
                string tradingGroup = _settings.TradingGroup(propertyDetails);
                string languageCode = _settings.LanguageCode(propertyDetails);

                var preBookRequest = new SerhsPreBookRequest(version, clientCode, password, branch, tradingGroup, languageCode)
                {
                    Accommodation =
                    {
                        Code = propertyDetails.TPKey,
                        Concept = baseHelper.ConceptCode,
                        Board = baseHelper.MealBasisCode,
                        Offer = baseHelper.OfferCode
                    },
                    Period =
                    {
                        Start = $"{propertyDetails.ArrivalDate:yyyyMMdd}",
                        End = $"{propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration):yyyyMMdd}"
                    }
                };

                if (preBookRequest.Version != BaseRequest.Version4)
                {
                    preBookRequest.Accommodation.Ticket = baseHelper.Ticket;
                }

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    baseHelper = new TpReference(roomDetails.ThirdPartyReference);

                    var room = new Room
                    {
                        Type = baseHelper.RoomTypeCode,
                        Adults = roomDetails.Adults,
                        Children = roomDetails.Children + roomDetails.Infants
                    };

                    foreach (var passenger in roomDetails.Passengers)
                    {
                        var guest = new GuestInfo
                        {
                            DocumentType = "0",
                            Document = string.Empty,
                            Name = passenger.FirstName,
                            Surname = passenger.LastName
                        };

                        if (passenger.PassengerType is PassengerType.Child or PassengerType.Infant)
                        {
                            guest.Age = passenger.Age;
                            room.Child.Add(guest);
                        }
                        else
                        {
                            room.Adult.Add(guest);
                        }
                    }

                    preBookRequest.Rooms.Add(room);
                }

                var xmlRequest = _serializer.Serialize(preBookRequest);
                var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
                xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);

                request.EndPoint = _settings.URL(propertyDetails);
                request.Method = eRequestMethod.POST;
                request.Source = ThirdParties.SERHS;
                request.ContentType = ContentTypes.Text_xml;
                request.LogFileName = "PreBook";
                request.CreateLog = propertyDetails.CreateLogs;
                request.SetRequest(xmlRequest);
                request.Send(_httpClient, _logger).RunSynchronously();

                //SERHS response returns error codes
                if (string.IsNullOrEmpty(request.ResponseXML.InnerText) || request.ResponseXML.InnerText.Substring(0, 3) == "000")
                {
                    return false;
                }

                var response = _serializer.DeSerialize<SerhsPreBookResponse>(request.ResponseXML);

                //store the basketId
                propertyDetails.TPRef1 = response.PreBooking.Code;

                var costs = response.AmountDetails.TotalAmount;
                var extraCostList = new List<string>();

                foreach (var extraCost in response.ExtraCharges)
                {
                    if (extraCost.Name == null || !SafeTypeExtensions.ToSafeBoolean(extraCost.Obligatory) || extraCost.Type == "included")
                    {
                        continue;
                    }

                    //decimal amount = extraCost.Prices.Sum(price => GetCleanCost(price.Amount));
                    //extraCostList.Add($"{extraCost.Name}: {amount}");

                    extraCostList.Add(extraCost.Name);
                }

                //SERHS have confirmed that extras shouldn't be added on to the cost, even if obligatory.
                //They are either included or direct payment, which means they are to be paid directly by the client.
                for (int i = 0; i < propertyDetails.Rooms.Count - 1; i++)
                {
                    decimal cost = ConvertSerhsPrice(costs[0]) / propertyDetails.Rooms.Count;
                    propertyDetails.Rooms[i].GrossCost = cost;
                    propertyDetails.Rooms[i].LocalCost = cost;
                }

                //get booking comments
                foreach (var collect in response.Remarks.SelectMany(remark => remark.Collects))
                {
                    if (collect.Address != null)
                    {
                        propertyDetails.Errata.AddNew("Important Information", collect.Address);
                    }
                }

                //Add extra costs
                if (extraCostList.Count > 0)
                {
                    propertyDetails.Errata.AddNew("Extra Costs",
                        string.Join(Environment.NewLine, extraCostList).TrimEnd());
                }

                //get cancellation policies
                try
                {
                    var sortedList = new SortedList<string, string>();
                    foreach (var cancelPolicy in response.CancelPolicies
                                 .Where(cancelPolicy => !sortedList.ContainsKey(cancelPolicy.ReleaseDate)))
                    {
                        sortedList.Add(cancelPolicy.ReleaseDate, cancelPolicy.Amount);
                    }

                    int c = 0;
                    DateTime startDate = default;
                    decimal value = 0;

                    var cancellations = new Cancellations();

                    foreach (var item in sortedList)
                    {
                        if (c > 0)
                        {
                            var endDate = ConvertSerhsDate(item.Key).AddDays(-1);
                            cancellations.AddNew(startDate, endDate, value);
                        }

                        startDate = ConvertSerhsDate(item.Key);
                        value = ConvertSerhsPrice(item.Value);

                        //set final band
                        if (c == sortedList.Count - 1)
                        {
                            cancellations.AddNew(startDate, new DateTime(2099, 12, 31), value);
                        }

                        c++;
                    }

                    propertyDetails.Cancellations = cancellations;
                }
                catch
                {
                    //don't record cancellation charges if something goes wrong but carry on with booking
                }

            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("PreBook Exception", exception.ToString());
                preBookSuccess = false;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS PreBook Request", request.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS PreBook Response", request.ResponseString);
                }
            }

            return preBookSuccess;
        }

        public DateTime ConvertSerhsDate(string date)
        {
            return new DateTime(SafeTypeExtensions.ToSafeInt(date.Substring(0, 4)),
                SafeTypeExtensions.ToSafeInt(date.Substring(4, 2)), SafeTypeExtensions.ToSafeInt(date.Substring(6, 2)));
        }

        public decimal ConvertSerhsPrice(string price)
        {
            return SafeTypeExtensions.ToSafeDecimal(price.Replace(".", "").Replace(",", "."));
        }

        private class TpReference
        {
            public readonly string RoomTypeCode;
            public readonly string MealBasisCode;
            public readonly string ConceptCode;
            public readonly string OfferCode;
            public readonly string Ticket = string.Empty;
            //public string CancelPolicyId = string.Empty;

            public TpReference(string tpReference)
            {
                string[] aParts = tpReference.Split('|');
                RoomTypeCode = aParts[0];
                MealBasisCode = aParts[1];
                ConceptCode = aParts[2];
                OfferCode = aParts[3];

                if (aParts.Length > 4)
                {
                    Ticket = aParts[4];
                    //cancelPolicyId = aParts[5];
                }
            }
        }

        public string Book(PropertyDetails propertyDetails)
        {
            string reference;
            var request = new Request();

            try
            {
                string version = _settings.Version(propertyDetails);
                string clientCode = _settings.ClientCode(propertyDetails);
                string password = _settings.Password(propertyDetails);
                string branch = _settings.Branch(propertyDetails);
                string tradingGroup = _settings.TradingGroup(propertyDetails);
                string languageCode = _settings.LanguageCode(propertyDetails);

                var bookRequest = new SerhsBookRequest(version, clientCode, password, branch, tradingGroup, languageCode)
                {
                    Client =
                    {
                        Code = _settings.ClientCode(propertyDetails),
                        Branch = _settings.Branch(propertyDetails),
                        TradingGroup = _settings.TradingGroup(propertyDetails)
                    },
                    Customer =
                    {
                        Name = propertyDetails.LeadGuestFirstName,
                        Surname = propertyDetails.LeadGuestLastName
                    },
                    Address = propertyDetails.LeadGuestAddress1,
                    City = 
                    {
                        Name = propertyDetails.LeadGuestTownCity,
                        ZipCode = propertyDetails.LeadGuestPostcode
                    },
                    Region =
                    {
                        Name = propertyDetails.LeadGuestCounty
                    },
                    Document =
                    {
                        Type = "0"
                    },
                    ContactInfo =
                    {
                        Email = string.Empty,
                        MobilePhone = string.Empty
                    },
                    PreBooking =
                    {
                        Confirmed = "1",
                        Type = "A",
                        Code = propertyDetails.TPRef1,
                        ClientReference = propertyDetails.BookingID == 0 ? "iVector" : propertyDetails.BookingID.ToString()
                    }
                };

                var xmlRequest = _serializer.Serialize(bookRequest);
                var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
                xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);

                request.Source = ThirdParties.SERHS;
                request.EndPoint = _settings.URL(propertyDetails);
                request.Method = eRequestMethod.POST;
                request.ContentType = ContentTypes.Text_xml;

                request.SetRequest(xmlRequest);
                request.Send(_httpClient, _logger).RunSynchronously();

                var response = _serializer.DeSerialize<SerhsBookResponse>(request.ResponseXML);
                var booking = response.Bookings.FirstOrDefault();

                if (booking == null || string.IsNullOrEmpty(booking.Code))
                {
                    reference = "failed";
                }
                else
                {
                    reference = booking.Code;
                }
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());
                reference = "failed";
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS Book Request", request.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS Book Response", request.ResponseString);
                }
            }
            return reference;
        }

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                request.EndPoint = _settings.URL(propertyDetails);
                request.Method = eRequestMethod.POST;
                request.Source = ThirdParties.SERHS;
                request.ContentType = ContentTypes.Text_html;
                request.LogFileName = "Cancel";
                request.CreateLog = propertyDetails.CreateLogs;

                var cancellationRequest =
                    BuildCancellationRequest(propertyDetails, propertyDetails.SourceReference, "1");

                var xmlRequest = _serializer.Serialize(cancellationRequest);
                var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
                xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);

                request.SetRequest(xmlRequest);

                request.Send(_httpClient, _logger).RunSynchronously();

                var response = _serializer.DeSerialize<SerhsCancellationResponse>(request.ResponseXML);

                cancellationResponse.Success = response.Status == "0" && response.Booking.Cancelled == "yes";
                cancellationResponse.TPCancellationReference = response.Cancel.Code;
            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;

                cancellationResponse.TPCancellationReference = "failed";

                propertyDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.EndPoint))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS Cancellation Request", request.EndPoint);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS Cancellation Response", request.ResponseString);
                }
            }

            return cancellationResponse;
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationCostResponse = new ThirdPartyCancellationFeeResult();

            try
            {
                request.EndPoint = _settings.URL(propertyDetails);
                request.Method = eRequestMethod.POST;
                request.Source = ThirdParties.SERHS;
                request.ContentType = ContentTypes.Text_html;
                request.LogFileName = "Cancellation Costs";
                request.CreateLog = propertyDetails.CreateLogs;

                var getCancellationCostRequest =
                    BuildCancellationRequest(propertyDetails, propertyDetails.SourceReference, "0");

                var xmlRequest = _serializer.Serialize(getCancellationCostRequest);
                var xmlDeclaration = xmlRequest.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
                xmlRequest.InsertBefore(xmlDeclaration, xmlRequest.DocumentElement);

                request.SetRequest(xmlRequest);

                request.Send(_httpClient, _logger).RunSynchronously();

                var response = _serializer.DeSerialize<SerhsCancellationResponse>(request.ResponseXML);

                cancellationCostResponse.Success = response.Status == "0";
                cancellationCostResponse.Amount = SafeTypeExtensions.ToSafeDecimal(response.AmountDetails.TotalAmount);
                cancellationCostResponse.CurrencyCode = response.AmountDetails.CurrencyCode;
            }
            catch (Exception exception)
            {
                cancellationCostResponse.Success = false;

                propertyDetails.Warnings.AddNew("GetCancellationCost Exception", exception.ToString());
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS GetCancellationCost Request", request.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SERHS, "SERHS GetCancellationCost Response", request.ResponseString);
                }
            }

            return cancellationCostResponse;
        }

        private SerhsCancellationRequest BuildCancellationRequest(IThirdPartyAttributeSearch searchDetails, string bookingReference, string requestTypeCode)
        {
            string version = _settings.Version(searchDetails);
            string clientCode = _settings.ClientCode(searchDetails);
            string password = _settings.Password(searchDetails);
            string branch = _settings.Branch(searchDetails);
            string tradingGroup = _settings.TradingGroup(searchDetails);
            string languageCode = _settings.LanguageCode(searchDetails);

            return new SerhsCancellationRequest(version, clientCode, password, branch, tradingGroup, languageCode)
            {
                Client =
                {
                    Code = _settings.ClientCode(searchDetails),
                    Branch = _settings.Branch(searchDetails)
                },
                Booking =
                {
                    Code = bookingReference,
                    Confirmed = requestTypeCode,
                    ClientReference = "iVector"
                },
                AgentInfo =
                {
                    Name = _settings.CancellationName(searchDetails),
                    Email = _settings.CancellationEmail(searchDetails),
                }
            };
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {

        }
    }
}

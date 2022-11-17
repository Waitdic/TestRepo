namespace iVectorOne.Suppliers.TPIntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Text.RegularExpressions;
    using iVector.Search.Property;
    using Intuitive.Helpers.Net;
    using Newtonsoft.Json;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.Helpers;
    using iVectorOne.Models.Property;

    //using MoreLinq;

    /// <summary>
    /// The Utility class for third party testing
    /// </summary>
    public class Helper
    {
        public static readonly RoomDetails RoomWithOneAdult = CreateRoomDetails();
        public static readonly RoomDetails RoomWithTwoAdults = CreateRoomDetails(2, 0, 0);
        public static readonly RoomDetails RoomWithOneChildOneAdult = CreateRoomDetails(1, 1, 0);
        public static readonly RoomDetails RoomWithOneInfantOneAdult = CreateRoomDetails(1, 0, 1);
        public static readonly RoomDetails FullRoom = CreateRoomDetails(1, 1, 1);
        public static readonly List<RoomDetails> Rooms = new List<RoomDetails> { RoomWithOneAdult, RoomWithTwoAdults, RoomWithOneChildOneAdult, RoomWithOneInfantOneAdult, FullRoom };
        public static readonly List<int> EmptyResortSplits = new List<int>();

        /// <summary>
        ///  Creates and returns a hard-coded search details object with specified fields in params
        /// </summary>
        /// <param name="supplier">The supplier source</param>
        /// <param name="roomDetails">The room to search for - one adult room by default</param>
        /// <returns>A search details object</returns>
        public static SearchDetails CreateSearchDetails(string supplier, RoomDetails? roomDetails = null)
        {
            if (roomDetails == null)
            {
                roomDetails = RoomWithOneAdult;
            }

            var searchDetails = new SearchDetails
            {
                ArrivalDate = new DateTime(2021, 9, 1),
                DepartureDate = new DateTime(2021, 9, 6),
                BookingDate = DateTime.Now,
                Duration = 5,
                RoomDetails = roomDetails,
                ThirdPartyConfigurations = new List<ThirdPartyConfiguration> { CreateThirdPartyConfiguration(supplier) },
                DedupeResults = DedupeMethod.cheapestleadin,
                LoggingType = "All",
                ISOCurrencyCode = "GBP"
            };

            return searchDetails;
        }

        /// <summary>
        /// Creates and returns a room details objects with number of guest specified in params.
        /// All children speified in the param are populated with asceding age (starting from 8)
        /// All infants have defualt age of 0 
        /// </summary>
        /// <param name="adults">Number of adults in the room</param>
        /// <param name="children">Number of children in the room</param>
        /// <param name="infants">Number of infants in the room</param>
        /// <returns>A room details object</returns>
        public static RoomDetails CreateRoomDetails(int adults = 1, int children = 0, int infants = 0)
        {
            var roomDetails = new RoomDetails();
            if (children == 0 && infants == 0)
            {
                roomDetails.Add(new RoomDetail(adults));
            }
            else
            {
                var childAges = new List<int>();
                int age = 7;
                for (int i = 0; i < children; ++i)
                {
                    age++;
                    childAges.Add(age);
                }

                roomDetails.Add(new RoomDetail(adults, children, infants, string.Join(",", childAges)));
            }
            return roomDetails;
        }

        /// <summary>
        /// Retrieves configurations from Users file of specified supplier and creates thirdparty configuration objects from retrieved data
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>A thirdparty configurations object</returns>
        public static ThirdPartyConfiguration CreateThirdPartyConfiguration(string supplier)
        {
            var users = JsonConvert.DeserializeObject<List<Account>>(helpers.Users);
            return users?.FirstOrDefault()?.Configurations?.FirstOrDefault(o => o.Supplier == supplier) ?? null!;
        }

        public static List<ResortSplit> CreateResortSplits(string supplier, List<Hotel> hotels, string resortCode)
        {
            return new List<ResortSplit>()
            {
                new ResortSplit()
                {
                    Hotels = hotels,
                    ThirdPartySupplier = supplier,
                    ResortCode = resortCode
                }
            };
        }

        /// <summary>
        ///  Creates and returns a list of with a hotel of supplied params
        /// </summary>
        /// <param name="propertyId">The property id of the hotel</param>
        /// <param name="propertyReferenceID">The property reference id of the hotel</param>
        /// <param name="propertyName">The property name of the hotel</param>
        /// <param name="tpKey">The TPKey of the hotel</param>
        /// <returns>A list of hotels objects</returns>
        public static List<Hotel> CreateHotels(int propertyId, int propertyReferenceID, string propertyName, string tpKey)
        {
            return new List<Hotel>
            {
                new Hotel(0, tpKey, propertyReferenceID, propertyId)
                {
                    PropertyName = propertyName,
                }
            };
        }

        /// <summary>
        /// Creates and returns a web request with supplied params
        /// </summary>
        /// <param name="URL">The endpoint of the request</param>
        /// <param name="thirdparty">The thirdparty supplier of the request</param>
        /// <returns>A web request object</returns>
        public static Request CreateWebRequest(string URL, string thirdparty)
        {
            var request = new Request
            {
                EndPoint = URL,
                Method = RequestMethod.POST,
                Source = thirdparty,
                ContentType = ContentTypes.Application_json,
                Accept = "application/json",
                TimeoutInSeconds = 100,
            };

            return request;
        }

        /// <summary>
        /// Compares two web requests by request string, endpoint and source. If same return true
        /// </summary>
        /// <param name="mockRequests">The hard-coded request to be compared from the resource file</param>
        /// <param name="builtRequests">The built request from thirdparty BuildSearchRequest method call</param>
        /// <returns>A boolean result of comparison</returns>
        public static bool AreSameWebRequests(List<Request> mockRequests, List<Request> builtRequests)
        {
            if (!builtRequests.Any()) return false;

            for (int i = 0; i < builtRequests.Count(); ++i)
            {
                var request = Regex.Replace(builtRequests[i].RequestString, @"\s+", string.Empty);  // removes all tabs new lines and whites spaces from request
                var mockrequest = Regex.Replace(mockRequests[i].RequestString, @"\s+", string.Empty);

                Assert.Equal(builtRequests[i].Source, mockRequests[i].Source);
                Assert.Equal(mockRequests[i].EndPoint, builtRequests[i].EndPoint);
                Assert.Equal(mockrequest, request);
            }

            return true;
        }

        /// <summary>
        ///  Verify the xml format.If valid then return true
        /// </summary>
        /// <param name="xml">The xml to be verified</param>
        /// <returns>A boolean result of verification</returns>
        public static bool IsValidXml(string xml)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verify transformed response
        /// </summary>
        /// <param name="mockResponse">The hard-coded response from the resource file</param>
        /// <param name="transformedResponse">The return response from thirdparty TransformResponce call</param>
        /// <returns>A boolan result of response verfication</returns>
        public static bool IsValidResponse(string mockResponse, string transformedResponse)
        {
            return IsValidXml(transformedResponse) && transformedResponse.Contains("</Results>") && mockResponse.Equals(transformedResponse);
        }

        /// <summary>
        /// Load the request log and returns a tuple with list of urls and requestString
        /// </summary>
        /// <param name="requestLog">The request log to be transformed</param>
        /// <returns>A tuple with list of urls and request strings</returns>
        public static Tuple<List<string>, List<string>> LoadRequestLog(string requestLog)
        {
            List<string> urls = new List<string>();
            List<string> requestStrings = new List<string>();

            string pattern = "\nURL: ";
            List<string> requests = Regex.Split(requestLog, pattern).ToList();
            var requestString = string.Empty;
            var url = string.Empty;

            foreach (var request in requests)
            {
                url = request.Substring(request.IndexOf("http"), request.IndexOf("\r")).Replace("\r", "").Replace("\n", "").Replace("*", "");
                urls.Add(url);

                if (!(url.Contains("<") || url.Contains("{")))
                {
                var startOfRequestString = request.IndexOf("<") != -1 ? request.IndexOf("<") : request.IndexOf("{"); // Find the beginning of a json responseString - {  or XML responseString - <
                if (startOfRequestString != -1) // requestString might be empty
                {
                    requestString = request.Substring(startOfRequestString);
                    requestStrings.Add(requestString);
                }
            }
            }

            if (!requestStrings.Any())
            {
                requestStrings = Enumerable.Repeat(string.Empty, 5).ToList();
            }

            return new Tuple<List<string>, List<string>>(urls, requestStrings);
        }

        /// <summary>
        /// Creates and returns a dictionary with hard-coded meal basis code 
        /// </summary>
        /// <returns>A dictionary with mealbasis codes</returns>
        public static Dictionary<string, int> GetMealBasisCodes()
        {
            return new Dictionary<string, int> { { "0", 8 }, { "1", 9 }, { "1073742551", 10 } };
        }

    }
}
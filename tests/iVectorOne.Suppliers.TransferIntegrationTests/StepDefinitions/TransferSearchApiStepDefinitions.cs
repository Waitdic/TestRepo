namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.Book;
    using iVectorOne.SDK.V2.TransferSearch;
    using Newtonsoft.Json;
    using System;
    using System.Security.Policy;

    [Binding]
    public class TransferSearchApiStepDefinitions : TransferBaseStepDefinitions
    {
        public TransferSearchApiStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"Create request object for search")]
        public void GivenCreateRequestObjectForSearch()
        {
            CreateClient("search");
            int departureId = 0, arrivalId = 0;
            var locations = GetLocation("gowaysydneytransfers");
            if (locations != null && locations.Count == 2)
            {
                arrivalId = 187;
                departureId = 184;
            }
            var requestObj = new Request
            {
                DepartureLocationID = departureId,
                ArrivalLocationID = arrivalId,
                DepartureDate = DateTime.Now.AddMonths(1),
                OneWay = true,
                Adults = 2,
                Supplier = "gowaysydneytransfers",
                DepartureTime = "10:00",
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["RequestObj"] = requestObj;
        }


        [When(@"make a post request to ""([^""]*)""")]
        public async Task WhenMakeAPostRequestTo(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["RequestObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(url, requestContent);

            _scenarioContext["ResponseCode"] = (int)response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<Response>(result);
                List<TransferResult> transferResults = obj.TransferResults;
                if (transferResults != null && transferResults.Count > 0)
                {
                    _scenarioContext["SearchResult"] = transferResults[0];
                    keyValuePairs["BookingToken"] = transferResults[0].BookingToken;
                    keyValuePairs["SupplierReference"] = transferResults[0].SupplierReference;
                }
                else
                {
                    GivenCreateRequestObjectForSearch();
                    await WhenMakeAPostRequestTo("v2/transfers/search");
                }
            }

        }

        [Then(@"the status code should be (.*)")]
        public void ThenTheStatusCodeShouldBe(int status)
        {
            Assert.Equal(_scenarioContext["ResponseCode"], status);
        }

        [Given(@"Create request object for prebook")]
        public async Task GivenCreateRequestObjectForPrebook()
        {
            GivenCreateRequestObjectForSearch();
            await WhenMakeAPostRequestTo("v2/transfers/search");
            CreateClient();
            string supplierReference = GetValue("SupplierReference");
            string bookingToken = GetValue("BookingToken");
            var requestObj = new SDK.V2.TransferPrebook.Request
            {
                SupplierReference = supplierReference,//"SYDTRINSSY1INTOP0-Default",
                BookingToken = bookingToken,//"QWed#   $c['!",
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["PrebookObj"] = requestObj;
        }

        [When(@"make a post request to prebook ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToPrebook(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["PrebookObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(url, requestContent);

            _scenarioContext["ResponseCode"] = (int)response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<SDK.V2.TransferPrebook.Response>(result);
                if (obj != null)
                {
                    _scenarioContext["PrebookResult"] = obj;
                    keyValuePairs["BookingToken"] = obj.BookingToken;
                    keyValuePairs["SupplierReference"] = obj.SupplierReference;

                }
            }
        }

        [Given(@"Create request object for book")]
        public async Task GivenCreateRequestObjectForBook()
        {
            GivenCreateRequestObjectForSearch();
            await WhenMakeAPostRequestTo("v2/transfers/search");
            CreateClient();
            string supplierReference = GetValue("SupplierReference");
            string bookingToken = GetValue("BookingToken");
            var guestDetails = new List<GuestDetail>() {
                new GuestDetail()
                {
                    Type = GuestType.Adult,
                    Title ="Mr",
                    FirstName ="Test",
                    LastName = "Test",
                    DateOfBirth = new DateTime(1990,5,5)
             } ,
             new GuestDetail()
             {
                Type = GuestType.Adult,
                Title ="Mrs",
                FirstName ="Test",
                LastName = "Test",
                DateOfBirth = new DateTime(1990,5,6)
             }
            };
            var requestObj = new SDK.V2.TransferBook.Request
            {
                BookingReference = "BookingRef2",
                SupplierReference = supplierReference,//"SYDTRINSSY1INTOP0-Default",
                BookingToken = bookingToken,// "QWed#   $c['!",
                LeadCustomer = new LeadCustomer()
                {
                    CustomerTitle = "Mr",
                    CustomerFirstName = "test",
                    CustomerLastName = "test",
                    DateOfBirth = new DateTime(1990, 05, 05),
                    CustomerAddress1 = "123 road",
                    CustomerAddress2 = "test area",
                    CustomerTownCity = "test town",
                    CustomerCounty = "test",
                    CustomerPostcode = "cr35ig",
                    CustomerBookingCountryCode = "GB",
                    CustomerPhone = "123456789",
                    CustomerMobile = "123456789",
                    CustomerEmail = "test@Test.com"
                },
                GuestDetails = guestDetails,
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["BookObj"] = requestObj;
        }

        [When(@"make a post request to book ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToBook(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["BookObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(url, requestContent);

            _scenarioContext["ResponseCode"] = (int)response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<SDK.V2.TransferBook.Response>(result);
                if (obj != null)
                {
                    _scenarioContext["BookResult"] = obj;
                    keyValuePairs["SupplierBookingReference"] = obj.SupplierBookingReference;
                    keyValuePairs["SupplierReference"] = obj.SupplierReference;

                }
            }

        }


        [Given(@"Create request object for cancel")]
        public async Task GivenCreateRequestObjectForCancel()
        {
            await GivenCreateRequestObjectForBook();
            await WhenMakeAPostRequestToBook("v2/transfers/book");

            string supplierReference = GetValue("SupplierReference");
            string supBookingRef = GetValue("SupplierBookingReference");
            var requestObj = new SDK.V2.TransferCancel.Request
            {
                SupplierReference = supplierReference,//"GOCA311356",
                SupplierBookingReference = supBookingRef,//"169180",
                ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
            };

            _scenarioContext["CancelObj"] = requestObj;
        }

        [When(@"make a post request to cancel ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToCancel(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)_scenarioContext["CancelObj"];
            var requestContent = CreateRequest(requestObj);

            var response = await _httpClient.PostAsync(url, requestContent);

            _scenarioContext["ResponseCode"] = (int)response.StatusCode;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<SDK.V2.TransferCancel.Response>(result);
                if (obj != null)
                {
                    _scenarioContext["CancelResult"] = obj;
                }
            }

        }
    }
}

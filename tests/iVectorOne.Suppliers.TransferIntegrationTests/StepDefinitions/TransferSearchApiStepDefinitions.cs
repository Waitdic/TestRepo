namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.Book;
    using iVectorOne.SDK.V2.TransferSearch;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Security.Policy;

    [Binding]
    public class TransferSearchApiStepDefinitions : TransferBaseStepDefinitions
    {
        private const int arrivalID = 187;
        private const int departureID = 184;
        private const string supplier = "gowaysydneytransfers";

        public TransferSearchApiStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"Create request object for search")]
        public void GivenCreateRequestObjectForSearch()
        {
            CreateClient("search");
            var requestObj = new Request
            {
                DepartureLocationID = departureID,
                ArrivalLocationID = arrivalID,
                DepartureDate = DateTime.Now.AddMonths(1),
                OneWay = true,
                Adults = 2,
                Supplier = supplier,
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
            TransferRequestBase requestObj = (TransferRequestBase)GetValueFromScenarioConext("RequestObj");
            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(url, requestContent);

                _scenarioContext["ResponseCode"] = (int)response.StatusCode;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<Response>(result);

                    List<TransferResult> transferResults = obj.TransferResults;
                    _scenarioContext["SearchResult"] = transferResults;
                    if (transferResults != null && transferResults.Count > 0)
                    {
                        keyValuePairs["BookingToken"] = transferResults[0].BookingToken;
                        keyValuePairs["SupplierReference"] = transferResults[0].SupplierReference;
                    }
                }

                else
                {
                    _scenarioContext["ErrorResponse"] = result;
                }
            }

        }

        [Then(@"the status code should be (.*)")]
        public void ThenTheStatusCodeShouldBe(int status)
        {
            Assert.Equal(status, GetValueFromScenarioConext("ResponseCode"));
            Assert.Null(GetValueFromScenarioConext("ErrorResponse"));
        }

        [Then(@"transfer results should have data")]
        public void ThenTransferResultsShouldHaveData()
        {
            Assert.NotEmpty((IList<TransferResult>)GetValueFromScenarioConext("SearchResult"));
        }

        [Given(@"Create request object for prebook")]
        public async Task GivenCreateRequestObjectForPrebook()
        {
            GivenCreateRequestObjectForSearch();
            await WhenMakeAPostRequestTo(SearchApi);
            CreateClient();
            string supplierReference = GetValue("SupplierReference");
            string bookingToken = GetValue("BookingToken");
            if (!string.IsNullOrEmpty(supplierReference) && !string.IsNullOrEmpty(bookingToken))
            {
                var requestObj = new SDK.V2.TransferPrebook.Request
                {
                    SupplierReference = supplierReference,
                    BookingToken = bookingToken,
                    ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
                };

                _scenarioContext["PrebookObj"] = requestObj;
            }
        }

        [When(@"make a post request to prebook ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToPrebook(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)GetValueFromScenarioConext("PrebookObj");
            if (requestObj != null)
            {
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
        }

        [Then(@"booking token and supplier reference are not empty")]
        public void ThenBookingTokenAndSupplierReferenceAreNotEmpty()
        {
            Assert.NotEqual(string.Empty, GetValue("BookingToken"));
            Assert.NotEqual(string.Empty, GetValue("SupplierReference"));
        }

        [Given(@"Create request object for book")]
        public async Task GivenCreateRequestObjectForBook()
        {
            await GivenCreateRequestObjectForPrebook();
            await WhenMakeAPostRequestToPrebook(PrebookApi);
            string supplierReference = GetValue("SupplierReference");
            string bookingToken = GetValue("BookingToken");

            if (!string.IsNullOrEmpty(supplierReference) && !string.IsNullOrEmpty(bookingToken))
            {
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
                    SupplierReference = supplierReference,
                    BookingToken = bookingToken,
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
        }

        [When(@"make a post request to book ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToBook(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)GetValueFromScenarioConext("BookObj");
            if (requestObj != null)
            {
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

        }


        [Given(@"Create request object for cancel")]
        public async Task GivenCreateRequestObjectForCancel()
        {
            await GivenCreateRequestObjectForBook();
            await WhenMakeAPostRequestToBook(BookApi);

            string supplierReference = GetValue("SupplierReference");
            string supBookingRef = GetValue("SupplierBookingReference");

            if (!string.IsNullOrEmpty(supplierReference) && !string.IsNullOrEmpty(supBookingRef))
            {

                var requestObj = new SDK.V2.TransferCancel.Request
                {
                    SupplierReference = supplierReference,
                    SupplierBookingReference = supBookingRef,
                    ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", URL },
                    { "AgentId", AgentID},
                    { "Password", Password }
                }
                };

                _scenarioContext["CancelObj"] = requestObj;
            }
        }

        [When(@"make a post request to cancel ""([^""]*)""")]
        public async Task WhenMakeAPostRequestToCancel(string url)
        {
            TransferRequestBase requestObj = (TransferRequestBase)GetValueFromScenarioConext("CancelObj");

            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(url, requestContent);

                _scenarioContext["ResponseCode"] = (int)response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var obj = JsonConvert.DeserializeObject<SDK.V2.TransferCancel.Response>(result);
                    if (obj != null)
                    {
                        keyValuePairs["SupplierCancellationReference"] = obj.SupplierCancellationReference;
                        _scenarioContext["CancelResult"] = obj;
                    }
                }
            }
        }

        [Then(@"Supplier Cancellation Reference should have data")]
        public void ThenSupplierCancellationReferenceShouldHaveData()
        {
            Assert.NotEqual(string.Empty, GetValue("SupplierCancellationReference"));
        }

        [Given(@"Create request object")]
        public async Task GivenCreateRequestObject()
        {
            await GivenCreateRequestObjectForCancel();
        }

        [When(@"make a post request to each endpoint")]
        public async Task WhenMakeAPostRequestToEachEndpoint()
        {
            await WhenMakeAPostRequestToCancel(CancelApi);
        }
    }
}

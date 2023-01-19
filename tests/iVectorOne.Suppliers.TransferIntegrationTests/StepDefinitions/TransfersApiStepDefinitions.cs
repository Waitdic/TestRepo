namespace iVectorOne.Suppliers.TransferIntegrationTests.StepDefinitions
{
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.Book;
    using iVectorOne.SDK.V2.TransferSearch;
    using Newtonsoft.Json;
    using System;

    [Binding]
    public class TransfersApiStepDefinitions : TransferBaseStepDefinitions
    {
        public TransfersApiStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }


        [Given(@"Create request object for search for ""([^""]*)""")]
        public void GivenCreateRequestObjectForSearchFor(string source = "", Table scenarioData = null)
        {
            string supplier;
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
                supplier = source;
            }
            else
                supplier = (string)GetValueFromScenarioConext("Source");

            int.TryParse((string)GetValueFromScenarioConext("DepartureID"), out int departureLocationID);
            int.TryParse((string)GetValueFromScenarioConext("ArrivalID"), out int arrivalLocationID);

            CreateClient("search");
            var requestObj = new Request
            {
                DepartureLocationID = departureLocationID,
                ArrivalLocationID = arrivalLocationID,
                DepartureDate = DateTime.Now.AddMonths(1),
                OneWay = true,
                Adults = 2,
                Supplier = supplier,
                DepartureTime = "10:00",
                IncludeOnRequest = true,
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
            Assert.Null(GetValueFromScenarioConext("ErrorResponse"));
            Assert.Equal(status, GetValueFromScenarioConext("ResponseCode"));
        }

        [Then(@"transfer results should have data")]
        public void ThenTransferResultsShouldHaveData()
        {
            Assert.NotEmpty((IList<TransferResult>)GetValueFromScenarioConext("SearchResult"));
        }

        [Given(@"Create request object for prebook for ""([^""]*)""")]
        public async Task GivenCreateRequestObjectForPrebookFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }
            GivenCreateRequestObjectForSearchFor();
            await WhenMakeAPostRequestTo(SearchApi);
            CreateClient();
            string supplierReference = GetValue("SupplierReference");
            string bookingToken = GetValue("BookingToken");
            if (!string.IsNullOrEmpty(supplierReference) && !string.IsNullOrEmpty(bookingToken))
            {
                var requestObj = new SDK.V2.TransferPrebook.Request
                {
                    SupplierReference = supplierReference,
                    BookingToken = bookingToken
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
                var result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<SDK.V2.TransferPrebook.Response>(result);
                    if (obj != null)
                    {
                        _scenarioContext["PrebookResult"] = obj;
                        keyValuePairs["BookingToken"] = obj.BookingToken;
                        keyValuePairs["SupplierReference"] = obj.SupplierReference;

                    }
                }
                else
                {
                    _scenarioContext["ErrorResponse"] = result;
                }
            }
        }

        [Then(@"booking token and supplier reference are not empty")]
        public void ThenBookingTokenAndSupplierReferenceAreNotEmpty()
        {
            Assert.NotEqual(string.Empty, GetValue("BookingToken"));
            Assert.NotEqual(string.Empty, GetValue("SupplierReference"));
        }

        [Given(@"Create request object for book for ""([^""]*)""")]
        public async Task GivenCreateRequestObjectForBookFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }

            await GivenCreateRequestObjectForPrebookFor();
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

                string bookingRef = (string)GetValueFromScenarioConext("BookingReference");

                var requestObj = new SDK.V2.TransferBook.Request
                {
                    BookingReference = bookingRef,
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
                    GuestDetails = guestDetails
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
                var result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<SDK.V2.TransferBook.Response>(result);
                    if (obj != null)
                    {
                        _scenarioContext["BookResult"] = obj;
                        keyValuePairs["SupplierBookingReference"] = obj.SupplierBookingReference;
                        keyValuePairs["SupplierReference"] = obj.SupplierReference;

                    }
                }
                else
                {
                    _scenarioContext["ErrorResponse"] = result;
                }
            }
        }

        [Given(@"Create request object for cancel for ""([^""]*)""")]
        public async Task GivenCreateRequestObjectForCancelFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }

            await GivenCreateRequestObjectForBookFor();
            await WhenMakeAPostRequestToBook(BookApi);

            string supplierReference = GetValue("SupplierReference");
            string supBookingRef = GetValue("SupplierBookingReference");

            if (!string.IsNullOrEmpty(supplierReference) && !string.IsNullOrEmpty(supBookingRef))
            {

                var requestObj = new SDK.V2.TransferCancel.Request
                {
                    SupplierReference = supplierReference,
                    SupplierBookingReference = supBookingRef

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
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<SDK.V2.TransferCancel.Response>(result);
                    if (obj != null)
                    {
                        keyValuePairs["SupplierCancellationReference"] = obj.SupplierCancellationReference;
                        _scenarioContext["CancelResult"] = obj;
                    }
                }
                else
                {
                    _scenarioContext["ErrorResponse"] = result;
                }
            }
        }

        [Then(@"Supplier Cancellation Reference should have data")]
        public void ThenSupplierCancellationReferenceShouldHaveData()
        {
            Assert.NotEqual(string.Empty, GetValue("SupplierCancellationReference"));
        }

        [When(@"make a post request to each endpoint")]
        public async Task WhenMakeAPostRequestToEachEndpoint()
        {
            await WhenMakeAPostRequestToCancel(CancelApi);
        }

        [Given(@"Create request object for ""([^""]*)""")]
        public async Task GivenCreateRequestObjectFor(string source, Table scenarioData)
        {
            SetDataFromStep(source, scenarioData);
            await GivenCreateRequestObjectForCancelFor();
        }

    }
}

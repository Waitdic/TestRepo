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
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }

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
                Supplier = (string)GetValueFromScenarioConext("Source"),
                DepartureTime = "10:00",
                IncludeOnRequest = true,
            };

            _scenarioContext["RequestObj"] = requestObj;
        }


        [When(@"a transfer search request is sent")]
        public async Task WhenATransferSearchRequestIsSent()
        {
            ComponentRequestBase requestObj = (ComponentRequestBase)GetValueFromScenarioConext("RequestObj");
            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(SearchApi, requestContent);

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

        [Then(@"transfer results should be returned")]
        public void ThenTransferResultsShouldBeReturned()
        {
            Assert.NotEmpty((IList<TransferResult>)GetValueFromScenarioConext("SearchResult"));
        }

        [Given(@"Create request object for prebook for ""([^""]*)""")]
        public void GivenCreateRequestObjectForPrebookFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }

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

        [When(@"a transfer prebook request is sent")]
        public async Task WhenATransferPrebookRequestIsSent()
        {
            GivenCreateRequestObjectForPrebookFor();
            ComponentRequestBase requestObj = (ComponentRequestBase)GetValueFromScenarioConext("PrebookObj");
            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(PrebookApi, requestContent);

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

        [Then(@"a booking token and supplier reference are returned")]
        public void ThenABookingTokenAndSupplierReferenceAreReturned()
        {
            Assert.NotEqual(string.Empty, GetValue("BookingToken"));
            Assert.NotEqual(string.Empty, GetValue("SupplierReference"));
        }

        [Given(@"Create request object for book for ""([^""]*)""")]
        public void GivenCreateRequestObjectForBookFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }
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

        [When(@"a transfer book request is sent")]
        public async Task WhenATransferBookRequestIsSent()
        {
            GivenCreateRequestObjectForBookFor();
            ComponentRequestBase requestObj = (ComponentRequestBase)GetValueFromScenarioConext("BookObj");
            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(BookApi, requestContent);

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
        public void GivenCreateRequestObjectForCancelFor(string source = "", Table scenarioData = null)
        {
            if (!string.IsNullOrEmpty(source) && scenarioData != null)
            {
                SetDataFromStep(source, scenarioData);
            }
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

        [When(@"a transfer precancel request is sent")]
        public async Task WhenATransferPrecancelRequestIsSent()
        {
            GivenCreateRequestObjectForCancelFor();
            ComponentRequestBase requestObj = (ComponentRequestBase)GetValueFromScenarioConext("CancelObj");

            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(PrecancelApi, requestContent);

                _scenarioContext["ResponseCode"] = (int)response.StatusCode;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<SDK.V2.TransferPrecancel.Response>(result);
                    if (obj != null)
                    {
                        keyValuePairs["Amount"] = Convert.ToString(obj.Amount);
                        _scenarioContext["PrecancelResult"] = obj;
                    }
                }
                else
                {
                    _scenarioContext["ErrorResponse"] = result;
                }
            }
        }

        [When(@"a transfer cancel request is sent")]
        public async Task WhenATransferCancelRequestIsSent()
        {
            GivenCreateRequestObjectForCancelFor();
            ComponentRequestBase requestObj = (ComponentRequestBase)GetValueFromScenarioConext("CancelObj");

            if (requestObj != null)
            {
                var requestContent = CreateRequest(requestObj);

                var response = await _httpClient.PostAsync(CancelApi, requestContent);

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

        [Given(@"Create request object for ""([^""]*)""")]
        public void GivenCreateRequestObjectFor(string source, Table scenarioData)
        {
            SetDataFromStep(source, scenarioData);
            GivenCreateRequestObjectForSearchFor();
        }

    }
}

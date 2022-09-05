namespace iVectorOne.Search
{
    using iVector.Search.Property;
    using iVectorOne.Models;
    using iVectorOne.Repositories;
    using iVectorOne.Search.Models;
    using Moq;

    public class ResultsDeduperTests
    {
        [Fact]
        public async Task DedupeResultsAsync_Should_ReplaceExistingResult_When_CheapestLeadIn_And_DedupeByTTICode_And_PropertyReferenceIDsMatch()
        {
            // arrange 
            var propertyReferenceID = 1;
            var mealBasisID = 0;
            var nonRefundable = false;

            SearchDetails searchDetails = BuildSearchDetails(DedupeMethod.cheapestleadin);

            var firstSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(1, propertyReferenceID, mealBasisID, nonRefundable, 100.00m)
            };

            var secondSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(2, propertyReferenceID, mealBasisID, nonRefundable, 80.00m)
            };

            var currencyLookup = GetCurrencyLookup();
            var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

            // act 1 
            var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

            // assert 1
            Assert.Equal(1, firstAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                });

            // act 2
            var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

            // assert 2
            Assert.Equal(1, secondAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(80.00m, rr.PriceData.Total);
                        });
                });
        }

        [Fact]
        public async Task DedupeResultsAsync_Should_AddNewResult_When_CheapestLeadIn_And_DedupeByTTICode_And_PropertyReferenceIDsDontMatch()
        {
            // arrange 
            var mealBasisID = 0;
            var nonRefundable = false;

            SearchDetails searchDetails = BuildSearchDetails(DedupeMethod.cheapestleadin);

            var firstSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(1, 1, mealBasisID, nonRefundable, 100.00m)
            };

            var secondSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(2, 2, mealBasisID, nonRefundable, 80.00m)
            };
            var currencyLookup = GetCurrencyLookup();
            var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

            // act 1 
            var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

            // assert 1
            Assert.Equal(1, firstAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(1, mealBasisID, nonRefundable), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                });

            // act 2
            var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

            // assert 2
            Assert.Equal(1, secondAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(1, mealBasisID, nonRefundable), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                },
                r =>
                {
                    Assert.Equal(GetCheckHashCode(2, mealBasisID, nonRefundable), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(80.00m, rr.PriceData.Total);
                        });
                });
        }

        //[Fact]
        //public async Task DedupeResultsAsync_Should_ReplaceExistingResult_And_AddNewResult_When_CheapestLeadIn_And_DedupeByTTICode_And_TwoPropertyReferenceIDsMatch_AndOneDoesnt()
        //{
        //    // arrange 
        //    var propertyReferenceID = 1;
        //    var mealBasisID = 0;
        //    var nonRefundable = false;

        //    SearchDetails searchDetails = BuildSearchDetails(Dedupe.Both);

        //    var firstSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(1, propertyReferenceID, mealBasisID, nonRefundable, 100.00m)
        //    };

        //    var secondSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(2, 2, mealBasisID, nonRefundable, 80.00m)
        //    };

        //    var thirdSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(3, propertyReferenceID, mealBasisID, nonRefundable, 60.00m)
        //    };

        //    var currencyLookup = GetCurrencyLookup();
        //    var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

        //    // act 1 
        //    var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

        //    // assert 1
        //    Assert.Equal(1, firstAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(100.00m, rr.PriceData.Total);
        //                });
        //        });

        //    // act 2
        //    var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

        //    // assert 2
        //    Assert.Equal(1, secondAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(100.00m, rr.PriceData.Total);
        //                });
        //        },
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(2, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(80.00m, rr.PriceData.Total);
        //                });
        //        });

        //    // act 3
        //    var thirdAddedCount = await resultsDeduper.DedupeResultsAsync(thirdSearchResults, searchDetails);

        //    // assert 3
        //    Assert.Equal(1, thirdAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(60.00m, rr.PriceData.Total);
        //                });
        //        },
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(2, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(80.00m, rr.PriceData.Total);
        //                });
        //        });
        //}

        [Fact]
        public async Task DedupeResultsAsync_Should_AddNewResult_When_CheapestLeadIn_And_DedupeByNone_And_PropertyReferenceIDsMatch()
        {
            // arrange 
            var propertyReferenceID = 1;
            var mealBasisID = 0;
            var nonRefundable = false;

            SearchDetails searchDetails = BuildSearchDetails(DedupeMethod.none);

            var firstSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(1, propertyReferenceID, mealBasisID, nonRefundable, 100.00m)
            };

            var secondSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(2, propertyReferenceID, mealBasisID, nonRefundable, 80.00m)
            };
            var currencyLookup = GetCurrencyLookup();
            var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

            // act 1 
            var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

            // assert 1
            Assert.Equal(1, firstAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable, 0), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                });

            // act 2
            var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

            // assert 2
            Assert.Equal(1, secondAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable, 0), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                },
                r =>
                {
                    Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable, 1), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(80.00m, rr.PriceData.Total);
                        });
                });
        }

        [Fact]
        public async Task DedupeResultsAsync_Should_AddNewResult_When_CheapestLeadIn_And_DedupeByNone_And_PropertyReferenceIDsDontMatch()
        {
            // arrange 
            var mealBasisID = 0;
            var nonRefundable = false;

            SearchDetails searchDetails = BuildSearchDetails(DedupeMethod.none);

            var firstSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(1, 1, mealBasisID, nonRefundable, 100.00m)
            };

            var secondSearchResults = new List<PropertySearchResult>
            {
                BuildPropertySearchResult(2, 2, mealBasisID, nonRefundable, 80.00m)
            };
            var currencyLookup = GetCurrencyLookup();
            var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

            // act 1 
            var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

            // assert 1
            Assert.Equal(1, firstAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(1, mealBasisID, nonRefundable, 0), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                });

            // act 2
            var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

            // assert 2
            Assert.Equal(1, secondAddedCount);
            Assert.Collection(OrderSearchResults(searchDetails),
                r =>
                {
                    Assert.Equal(GetCheckHashCode(1, mealBasisID, nonRefundable, 0), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(100.00m, rr.PriceData.Total);
                        });
                },
                r =>
                {
                    Assert.Equal(GetCheckHashCode(2, mealBasisID, nonRefundable, 1), r.Key);
                    Assert.Collection(r.Value.RoomResults,
                        rr =>
                        {
                            Assert.Equal(80.00m, rr.PriceData.Total);
                        });
                });
        }

        //[Fact]
        //public async Task DedupeResultsAsync_Should_AddNewResult_When_CheapestLeadIn_And_DedupeByBoth_And_TwoPropertyReferenceIDsMatch_AndOneDoesnt()
        //{
        //    // arrange 
        //    var propertyReferenceID = 1;
        //    var mealBasisID = 0;
        //    var nonRefundable = false;

        //    SearchDetails searchDetails = BuildSearchDetails(Dedupe.Both);

        //    var firstSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(1, propertyReferenceID, mealBasisID, nonRefundable, 100.00m)
        //    };

        //    var secondSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(2, propertyReferenceID, mealBasisID, nonRefundable, 80.00m)
        //    };

        //    var thirdSearchResults = new List<PropertySearchResult>
        //    {
        //        BuildPropertySearchResult(3, 0, mealBasisID, nonRefundable, 60.00m)
        //    };

        //    var currencyLookup = GetCurrencyLookup();
        //    var resultsDeduper = new ResultsDeduper(currencyLookup.Object);

        //    // act 1 
        //    var firstAddedCount = await resultsDeduper.DedupeResultsAsync(firstSearchResults, searchDetails);

        //    // assert 1
        //    Assert.Equal(1, firstAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(100.00m, rr.PriceData.Total);
        //                });
        //        });

        //    // act 2
        //    var secondAddedCount = await resultsDeduper.DedupeResultsAsync(secondSearchResults, searchDetails);

        //    // assert 2
        //    Assert.Equal(1, secondAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(80.00m, rr.PriceData.Total);
        //                });
        //        });

        //    // act 3
        //    var thirdAddedCount = await resultsDeduper.DedupeResultsAsync(thirdSearchResults, searchDetails);

        //    // assert 3
        //    Assert.Equal(1, thirdAddedCount);
        //    Assert.Collection(OrderSearchResults(searchDetails),
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(0, mealBasisID, nonRefundable, 1), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(60.00m, rr.PriceData.Total);
        //                });
        //        },
        //        r =>
        //        {
        //            Assert.Equal(GetCheckHashCode(propertyReferenceID, mealBasisID, nonRefundable), r.Key);
        //            Assert.Collection(r.Value.RoomResults,
        //                rr =>
        //                {
        //                    Assert.Equal(80.00m, rr.PriceData.Total);
        //                });
        //        });
        //}

        private static IOrderedEnumerable<KeyValuePair<string, DedupeSearchResult>> OrderSearchResults(SearchDetails searchDetails)
        {
            return searchDetails.ConcurrentResults.OrderBy(x => x.Key);
        }

        private static string GetCheckHashCode(int propertyReferenceID, int mealBasisID, bool nonRefundable)
        {
            return $"{propertyReferenceID}_{mealBasisID}_{(nonRefundable ? 1 : 0)}";
        }

        private static string GetCheckHashCode(int propertyReferenceID, int mealBasisID, bool nonRefundable, int keyCount)
        {
            return $"{propertyReferenceID}_{mealBasisID}_{(nonRefundable ? 1 : 0)}_{keyCount}";
        }

        private static PropertySearchResult BuildPropertySearchResult(
            int propertyID,
            int propertyReferenceID,
            int mealBasisID,
            bool nonRefundable,
            decimal amount)
        {
            return new PropertySearchResult
            {
                PropertyData = BuildPropertyData(propertyID, propertyReferenceID),
                RoomResults = new List<RoomSearchResult>
                {
                    new RoomSearchResult
                    {
                        PriceData = BuildPriceData(amount),
                        RoomData = BuildRoomData(mealBasisID, nonRefundable)
                    }
                }
            };
        }

        private static SearchDetails BuildSearchDetails(DedupeMethod dedupe)
        {
            return new SearchDetails
            {
                Settings = new Settings
                {
                    SingleRoomDedupingAlgorithm = DedupeMethod.cheapestleadin
                },
                DedupeResults = dedupe
            };
        }

        private static PropertyData BuildPropertyData(int propertyID, int propertyReferenceID)
        {
            return new PropertyData
            {
                PropertyID = propertyID,
                PropertyReferenceID = propertyReferenceID
            };
        }

        private static RoomData BuildRoomData(int mealBasisID, bool nonRefundable)
        {
            return new RoomData
            {
                MealBasisID = mealBasisID,
                NonRefundable = nonRefundable
            };
        }

        private static PriceData BuildPriceData(decimal amount)
        {
            return new PriceData
            {
                CurrencyID = 1,
                Total = amount
            };
        }

        private static Mock<ICurrencyLookupRepository> GetCurrencyLookup()
        {
            var mock = new Mock<ICurrencyLookupRepository>();

            mock.Setup(m => m.GetExchangeRateFromISOCurrencyIDAsync(It.IsAny<int>()))
                .ReturnsAsync(1);

            return mock;
        }
    }
}

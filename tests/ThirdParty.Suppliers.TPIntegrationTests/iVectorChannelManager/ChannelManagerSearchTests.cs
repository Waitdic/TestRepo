﻿namespace ThirdParty.Suppliers.TPIntegrationTests.iVectorChannelManager
{
    using Intuitive.Helpers.Serialization;
    using System.Collections.Generic;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.iVectorChannelManager;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.iVectorChannelManager;

    public class ChannelManagerSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = ThirdParties.CHANNELMANAGER;

        private static readonly SearchDetails _searchDetails = Helpers.Helper.CreateSearchDetails(_provider);

        private static readonly IChannelManagerSettings _settings = new InjectedChannelManagerSettings();

        public ChannelManagerSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new ChannelManagerSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequestAsync(ChannelManagerRes.RequestLog).Result);
            Assert.False(base.InvalidBuildSearchRequestAsync(ChannelManagerRes.RequestLog).Result);
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(ChannelManagerRes.ResponseString, ChannelManagerRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}
namespace ThirdParty.CSSuppliers.BedsWithEase
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Common;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models.Property.Booking;

    public class BedsWithEaseHelper
    {
        public static async Task<Result> SendRequestAsync(
            IThirdPartyAttributeSearch searchDetails,
            XmlDocument xmlRequest,
            string soapAction,
            IBedsWithEaseSettings settings,
            HttpClient httpClient,
            ILogger logger)
        {
            var webRequest = new Request
            {
                EndPoint = settings.URL(searchDetails),
                Method = RequestMethod.POST,
                SOAP = true,
                SoapAction = soapAction,
                Source = ThirdParties.BEDSWITHEASE,
                UseGZip = settings.UseGZIP(searchDetails)
            };

            webRequest.SetRequest(xmlRequest);
            await webRequest.Send(httpClient, logger);

            return new Result(webRequest, webRequest.ResponseXML);
        }

        public static async Task EndSessionAsync(
            PropertyDetails propertyDetails,
            IBedsWithEaseSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger logger)
        {
            var envelope = new Envelope<EndSessionRequest>
            {
                Body =
                {
                    Content =
                    {
                        SessionId = propertyDetails.TPRef1
                    }
                }
            };

            await SendRequestAsync(
                propertyDetails,
                serializer.Serialize(envelope),
                settings.SOAPEndSession(propertyDetails),
                settings,
                httpClient,
                logger);
        }

        public static async Task<string> GetSessionIdAsync(
            IThirdPartyAttributeSearch searchDetails,
            IBedsWithEaseSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger logger)
        {
            var envelope = new Envelope<StartSessionRequest>
            {
                Body =
                {
                    Content =
                    {
                        UserId = settings.Username(searchDetails),
                        Password = settings.Password(searchDetails),
                        LanguageCode = settings.LanguageCode(searchDetails)
                    }
                }
            };

            var result = await SendRequestAsync(
                searchDetails,
                serializer.Serialize(envelope),
                settings.SOAPStart(searchDetails),
                settings,
                httpClient,
                logger);

            string sessionId = string.Empty;

            if (result.Response != null)
            {
                var response = serializer.DeSerialize<Envelope<StartSessionResponse>>(result.Response).Body.Content;
                sessionId = response.SessionId;
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new NullReferenceException("SessionID not retrieved.");
            }

            return sessionId;
        }

        public static string ConvertTitleFormat(string title)
        {
            return title.ToLower().Trim() switch
            {
                "mr" => "MR",
                "mrs" => "MRS",
                "master" => "MSTR",
                "miss" => "MISS",
                "ms" => "MS",
                _ => ""
            };
        }
    }
}
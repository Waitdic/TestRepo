namespace iVectorOne.CSSuppliers.Miki.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Common;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Constants;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;

    public static class MikiSupport
    {
        /// <summary>
        /// Creates a random request ID with a maximum length of 9 digits
        /// </summary>
        /// <returns>The randomly generated request ID</returns>
        public static string CreateRequestID()
        {
            // Since datetime was previously used for requestid, use its max value as the lower bound for the random number generation
            // There is only one digit available for representing seconds so subsitute in 9s
            string maxDateTime = DateTime.MaxValue.ToString("MMddhhmm") + 9;
            var randomGenerator = new Random();

            #pragma warning disable SCS0005 // Weak random generator
            string randomNumber = randomGenerator.Next(maxDateTime.ToSafeInt() + 1, 1000000000).ToString("D9");
            #pragma warning restore SCS0005 // Weak random generator
           
            return randomNumber;
        }

        public async static Task<string> GetCurrencyCodeAsync(string isoCurrencyCode, ITPSupport support)
        {
            string currencyCode = await support.TPCurrencyCodeLookupAsync(ThirdParties.MIKI, isoCurrencyCode);

            return !string.IsNullOrEmpty(currencyCode) ? currencyCode : "GBP";
        }

        public async static Task<string> GetAgentCodeAsync(
            SearchDetails searchDetails,
            IMikiSettings settings,
            ITPSupport support)
        {
            return GetAgentCode(await GetCurrencyCodeAsync(searchDetails.ISOCurrencyCode, support), searchDetails, settings);
        }

        public static string GetAgentCode(
            string currencyCode,
            IThirdPartyAttributeSearch searchDetails,
            IMikiSettings settings)
        {
            string agentCode;

            switch (currencyCode)
            {
                case "USD": agentCode = settings.AgentCodeUSD(searchDetails);
                    break;
                case "EUR": agentCode = settings.AgentCodeEUR(searchDetails);
                    break;
                default: agentCode = settings.AgentCodeGBP(searchDetails);
                    break;
            }

            return agentCode;
        }

        public async static Task<string> GetPasswordAsync(
            IThirdPartyAttributeSearch searchDetails,
            IMikiSettings settings,
            ISerializer serializer,
            IMemoryCache cache)
        {
            try
            {
                var todaysDate = DateTime.Now.Date;
                string accessCodesFilename = settings.AccessCodesFilename(searchDetails);

                Task<Dictionary<string, string>> cacheBuilder()
                {
                    var passwordCollection = new Dictionary<string, string>();
                    var passwordXml = new XmlDocument();

                    switch (accessCodesFilename)
                    {
                        case "AccessCodes_BookABed":
                            passwordXml.LoadXml(MikiRes.AccessCodes_BookABed);
                            break;
                    }

                    var passwordObject = serializer.DeSerialize<AccessCodes>(passwordXml);

                    var dates = Enumerable.Range(0, 7).Select(i => todaysDate.AddDays(i));

                    foreach (var date in dates)
                    {
                        var password = passwordObject.DailyPasswords.First(x => x.SendDate == date.ToString("yyyy-MM-dd")).Password;
                        if (!string.IsNullOrEmpty(password))
                        {
                            passwordCollection.Add(date.ToString("yyyy-MM-dd"), password);
                        }
                    }

                    return Task.FromResult(passwordCollection);
                }

                var passwordCollection = await cache.GetOrCreateAsync($"Miki_{accessCodesFilename}", cacheBuilder, 60);
                passwordCollection.TryGetValue(todaysDate.ToString("yyyy-MM-dd"), out string password);

                return password;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async static Task<string> GetRoomTypeInfoAsync(string RoomTypeCode, IMemoryCache cache)
        {
            Task<Dictionary<string, string>> cacheBuilder()
            {
                var roomTypeInfo = new Dictionary<string, string>();

                // read from res file
                var roomTypes = new StringReader(MikiRes.RoomTypesWithTranslations);
                string roomTypesLine = roomTypes.ReadLine();

                // loop through Room Type lines, 
                while (!string.IsNullOrEmpty(roomTypesLine))
                {
                    string[] roomTypesCells = roomTypesLine.Split('|');
                    if (!roomTypeInfo.ContainsKey(roomTypesCells[0]))
                    {
                        roomTypeInfo.Add(roomTypesCells[0], roomTypesLine);
                    }

                    roomTypesLine = roomTypes.ReadLine();
                }

                return Task.FromResult(roomTypeInfo);
            }

            var roomTypeInfo = await cache.GetOrCreateAsync("MikiRoomTypeInfo", cacheBuilder, 9999);

            return roomTypeInfo.ContainsKey(RoomTypeCode) ? roomTypeInfo[RoomTypeCode] : "";
        }

        public async static Task<string> GetRoomDescriptionAsync(string language, string roomTypeCode, IMemoryCache cache)
        {
            string roomTypeInfo = await MikiSupport.GetRoomTypeInfoAsync(roomTypeCode, cache);
            string[] roomTypeInfoSplit = GetSplit(roomTypeInfo);
            string description = string.Empty;

            switch (language)
            {
                case "en": 
                    description = roomTypeInfoSplit[1];
                    break;
                case "fr": 
                    description = roomTypeInfoSplit.Length >= 11 && !string.IsNullOrEmpty(roomTypeInfoSplit[10]) 
                        ? roomTypeInfoSplit[10]
                        : roomTypeInfoSplit[1];
                    break;
                case "es":
                    description = roomTypeInfoSplit.Length >= 12 && !string.IsNullOrEmpty(roomTypeInfoSplit[11])
                        ? roomTypeInfoSplit[11]
                        : roomTypeInfoSplit[1];
                    break;
                case "zh":
                    description = roomTypeInfoSplit.Length >= 13 && !string.IsNullOrEmpty(roomTypeInfoSplit[12])
                        ? roomTypeInfoSplit[12]
                        : roomTypeInfoSplit[1];
                    break;
            }

            return description;
        }

        public class City
        {
            public int CityNumber { get; set; }
            public List<int> LocationIds { get; set; } = new();

            public City(int CityNumber, int LocationId)
            {
                this.CityNumber = CityNumber;
                this.LocationIds.Add(LocationId);
            }
        }

        public static RequestAuditInfo BuildRequestAuditInfo(string agentCode, string password)
        {
            return new RequestAuditInfo
            {
                AgentCode = agentCode,
                RequestPassword = password,
                RequestID = CreateRequestID(),
                RequestDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")
            };
        }

        public static string[] GetSplit(string value)
        {
            return value.Split('|');
        }
    }
}

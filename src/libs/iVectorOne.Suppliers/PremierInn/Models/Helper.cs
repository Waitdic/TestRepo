namespace iVectorOne.Suppliers.PremierInn.Models
{
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Search.Models;
    using Common;
    using Soap;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class Helper
    {
        public static Login BuildLogin(IPremierInnSettings settings, SearchDetails searchDetails)
        {
            return new Login
            {
                UserName = settings.User(searchDetails),
                Password = settings.Password(searchDetails)
            };
        }

        public static string CreateEnvelope(ISerializer serializer, string content)
        {
            var envelope = new EnvelopeRequest<ProcessMessage>
            {
                Body =
                {
                    Content =
                    {
                        Content = content
                    }
                }
            };

            return serializer.Serialize(envelope).InnerXml
                .Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "")
                .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
        }

        public static List<Batch<string>> GetHotelCodes(List<ResortSplit> resortSplits, int limit)
        {
            return resortSplits.SelectMany(split => split.Hotels.Select(h => h.TPKey))
                .Distinct().Batch(limit).ToList();
        }

        public static string GetMealCode(string cellCode)
        {
            return cellCode switch
            {
                "XMLBB" or "HUBFB" or "ZIPFB" or "DISBB" or "HUBSB" or "ZIPNF" => "BB",
                "XMLMD" or "DISMD" => "DBB",
                _ => "RO"
            };
        }
    }
}

using Intuitive.Helpers.Serialization;
using iVectorOne.Suppliers.GoGlobal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace iVectorOne.Suppliers.TourPlanTransfers
{
    public static class Helpers
    {
        public static XmlDocument Serialize(object request, ISerializer _serializer)
        {
            var xmlRequest = _serializer.SerializeWithoutNamespaces(request);
            xmlRequest.InnerXml = $"<Request>{xmlRequest.InnerXml}</Request>";
            return xmlRequest;
        }
        public static T DeSerialize<T>(XmlDocument xmlDocument, ISerializer _serializer) where T : class
        {
            var xmlResponse = _serializer.CleanXmlNamespaces(xmlDocument);
            xmlResponse.InnerXml = xmlResponse.InnerXml.Replace("<Reply>", "").Replace("</Reply>", "");
            return _serializer.DeSerialize<T>(xmlResponse);
        }

        public static string CreateSupplierReference(string outBoundOpt, string outBoundRateId, string returnOpt = "", string returnRateId = "")
        {
            var reference = outBoundOpt + "-" + outBoundRateId;
            if (!string.IsNullOrEmpty(returnOpt))
            {
                reference += "|" + returnOpt + "-" + returnRateId;
            }
            return reference;
        }

        public static decimal DivideBy100M(this decimal value)
        {
            return value / 100m;
        }

    }
}

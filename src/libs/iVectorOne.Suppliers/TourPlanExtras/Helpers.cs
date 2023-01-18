using Intuitive.Helpers.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace iVectorOne.Suppliers.TourPlanExtras
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
            if (xmlResponse.ChildNodes[0].ChildNodes.Count > 1)
            {
                XmlNode node = xmlResponse.SelectSingleNode("/Reply/" + typeof(T).Name);
                xmlResponse.RemoveAll();
                xmlResponse.AppendChild(node);
            }
            else
            {
                xmlResponse.InnerXml = xmlResponse.InnerXml.Replace("<Reply>", "").Replace("</Reply>", "");
            }
            return _serializer.DeSerialize<T>(xmlResponse);
        }
    }
}

namespace iVectorOne.Suppliers.Restel
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using Models;

    public static class RestelCommon
    {
        public static XmlDocument CreateAvailabilityRequestXml(
            string resortCode,
            int duration,
            DateTime arrivalDate,
            DateTime departureDate,
            string codusu,
            IEnumerable<string> paxes,
            ISerializer serializer)
        {
            var availabilityRequest = new RestelAvailabilityRequest
            {
                Parametros =
                {
                    Pais = resortCode.Substring(0, 2).ToUpper(),
                    Provincia = resortCode.ToUpper().PadRight(5, ' '),
                    Radio = duration < 5 ? 9 : 0,
                    Fechaentrada = arrivalDate.ToString("MM/dd/yyyy"),
                    Fechasalida = departureDate.ToString("MM/dd/yyyy"),
                    Usuario = codusu,
                    Idioma = 2
                }
            };

            foreach (string p in paxes)
            {
                availabilityRequest.Parametros.Numhab.Add(1);
                availabilityRequest.Parametros.Paxes.Add(p);
            }

            return serializer.Serialize(availabilityRequest);
        }

        public static Request CreateRequest(IRestelSettings settings, IThirdPartyAttributeSearch propertyDetails, string logFileName)
        {
            string userAgent = settings.UserAgent(propertyDetails);
            string user = settings.User(propertyDetails);
            string password = settings.Password(propertyDetails);
            string token = settings.AccessToken(propertyDetails);
            string URL = settings.GenericURL(propertyDetails);

            return new Request
            {
                EndPoint = $"{URL}?codusu={userAgent}&codigousu={user}&clausu={password}&afiliacio=RS&secacc={token}",
                Method = RequestMethod.POST,
                Source = ThirdParties.RESTEL,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
                Param = "xml",
                LogFileName = logFileName,
                CreateLog = true
            };
        }
    }
}
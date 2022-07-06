namespace ThirdParty.CSSuppliers.Restel
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
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
            string codusu = settings.Codusu(propertyDetails);
            string codigousu = settings.Codigousu(propertyDetails);
            string clausu = settings.Clausu(propertyDetails);
            string secacc = settings.Secacc(propertyDetails);
            string URL = settings.GenericURL(propertyDetails);

            return new Request
            {
                EndPoint = $"{URL}?codusu={codusu}&codigousu={codigousu}&clausu={clausu}&afiliacio=RS&secacc={secacc}",
                Method = eRequestMethod.POST,
                Source = ThirdParties.RESTEL,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
                Param = "xml",
                LogFileName = logFileName,
                CreateLog = true
            };
        }
    }
}
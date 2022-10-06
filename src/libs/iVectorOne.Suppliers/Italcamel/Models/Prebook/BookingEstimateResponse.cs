﻿namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class BookingEstimateResponse : SoapContent
    {
        [XmlElement("BOOKINGESTIMATEResult")]
        public PreebookResponse BookingEstimateResult { get; set; } = new();
    }
}

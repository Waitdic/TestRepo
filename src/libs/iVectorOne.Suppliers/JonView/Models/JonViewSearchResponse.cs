namespace iVectorOne.Suppliers.JonView
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable()]
    [XmlRoot("message")]
    public class JonViewSearchResponse
    {
        [XmlElement("alternatelistseg")]
        public ResponseDetails Response { get; set; }

        [Serializable()]
        [XmlType("alternatelistseg")]
        public class ResponseDetails
        {
            [XmlElement("listrecord")]
            public List<Room> Rooms { get; set; } = new List<Room>();
        }

        [Serializable()]
        public class Room
        {
            public string prodcode { get; set; }
            public string status { get; set; }
            public string productname { get; set; }
            public string currencycode { get; set; }
            public string dayprice { get; set; }
            public string suppliercode { get; set; }
            [XmlElement("productnamedetails")]
            public Details roomDetails { get; set; } = new Details();
            [XmlElement("cancellationpolicy")]
            public CancellationPolicy cancellationPolicy { get; set; }
        }

        [Serializable()]
        public class Details
        {
            public string board { get; set; }
            public string roomtype { get; set; }
        }

        public class CancellationPolicy
        {
            [XmlElement("canpolicyitem")]
            public CancellationDetails item { get; set; } = new CancellationDetails();
        }

        public class CancellationDetails
        {
            public string fromdays { get; set; }
        }

    }
}
﻿using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Elements")]
    public class Elements
    {
        public Elements(HotelElement hotelElement)
        {
            HotelElement = hotelElement;
        }

        public Elements()
        {
        }

        [XmlElement(ElementName = "HotelElement")]
        public HotelElement HotelElement { get; set; }
    }
}

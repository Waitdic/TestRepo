namespace iVectorOne.Suppliers.JonView.Models.Prebook
{
    using System;
    using System.Xml.Serialization;

    public class PrebookSearchSegment
    {
        [XmlElement("changesince")]
        public string ChangeSince { get; set; } = string.Empty;

        [XmlElement("fromdate")]
        public string FromDate { get; set; } = string.Empty;

        [XmlElement("todate")]
        public string ToDate { get; set; } = string.Empty;

        [XmlElement("prodtypecode")]
        public string ProdTypeCode { get; set; } = string.Empty;

        [XmlElement("searchtype")]
        public string SearchType { get; set; } = string.Empty;

        [XmlElement("productlistseg")]
        public ProductListSeg ProductListSeg { get; set; } = new();

        [XmlElement("displayrestriction")]
        public string DisplayRestriction { get; set; } = string.Empty;

        [XmlElement("displaypolicy")]
        public string DisplayPolicy { get; set; } = string.Empty;

        [XmlElement("displayschdate")]
        public string DisplaySchDate { get; set; } = string.Empty;

        [XmlElement("displaynamedetails")]
        public string DisplayNameDetails { get; set; } = string.Empty;

        [XmlElement("displayusage")]
        public string DisplayUsage { get; set; } = string.Empty;

        [XmlElement("displaygeocode")]
        public string DisplayGeoCode { get; set; } = string.Empty;

        [XmlElement("displaydynamicrates")]
        public string DisplayDynamicRates { get; set; } = string.Empty;
    }

    public class ProductListSeg
    {
        [XmlElement("codeitem")]
        public CodeItem CodeItem { get; set; } = new();

        [XmlElement("listrecord")]
        public ListRecord[] ListRecord { get; set; } = Array.Empty<ListRecord>();
        public bool ShouldSerializeListRecord() => ListRecord.Length != 0;
    }

    public class CodeItem
    {
        [XmlElement("productcode")]
        public string ProductCode { get; set; } = string.Empty;
    }

    public class ListRecord
    {
        [XmlElement("cancellation")]
        public Cancellation Cancellation { get; set; } = new();
    }

    public class Cancellation
    {
        [XmlElement("canitem")]
        public CanItem CanItem { get; set; }
    }

    public class CanItem
    {
        [XmlElement("fromdays")]
        public int FromDays { get; set; }

        [XmlElement("todays")]
        public int ToDays { get; set; }

        [XmlElement("chargetype")]
        public string ChargeType { get; set; } = string.Empty;

        [XmlElement("ratetype")]
        public string RateType { get; set; } = string.Empty;

        [XmlElement("canrate")]
        public decimal CanRate { get; set; }

        [XmlElement("cannote")]
        public string CanNote { get; set; } = string.Empty;
    }
}

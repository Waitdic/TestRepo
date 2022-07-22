namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Agreement
    {
        [XmlElement("deadline")]
        public Deadline Deadline { get; set; } = new();

        [XmlAttribute("deadline")]
        public string _Deadline { get; set; } = string.Empty;

        [XmlElement("policies")]
        public Policies Policies { get; set; } = new();

        [XmlElement("deadline_remarks")]
        public string DeadlineRemarks { get; set; } = string.Empty;

        [XmlArray("remarks")]
        [XmlArrayItem("remark")]
        public Remark[] Remarks { get; set; } = Array.Empty<Remark>();

        [XmlElement("room")]
        public Room[] Room { get; set; } = Array.Empty<Room>();

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("available")]
        public string Available { get; set; } = string.Empty;

        [XmlAttribute("room_basis")]
        public string RoomBasis { get; set; } = string.Empty;

        [XmlAttribute("meal_basis")]
        public string MealBasis { get; set; } = string.Empty;

        [XmlAttribute("ctype")]
        public string Ctype { get; set; } = string.Empty;

        [XmlAttribute("c_type")]
        public string C_type { get; set; } = string.Empty;

        [XmlAttribute("room_type")]
        public string RoomType { get; set; } = string.Empty;

        [XmlAttribute("is_dynamic")]
        public string IsDynamic { get; set; } = string.Empty;

        [XmlAttribute("currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlAttribute("total")]
        public string Total { get; set; } = string.Empty;

        [XmlAttribute("total_gross")]
        public string TotalGross { get; set; } = string.Empty;

        [XmlAttribute("original_total")]
        public string OriginalTotal { get; set; } = string.Empty;

        [XmlAttribute("special")]
        public string Special { get; set; } = string.Empty;
    }
}
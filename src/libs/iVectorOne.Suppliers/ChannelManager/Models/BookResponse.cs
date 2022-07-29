namespace iVectorOne.Suppliers.ChannelManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.ChannelManager.Models.Common;

    public class BookResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        public string BookingReference { get; set; }
        public string ChannelManagerReference { get; set; }
        public int PropertyReferenceID { get; set; }
        public int CurrencyID { get; set; }
        public string ChannelManager { get; set; }
        public decimal NetCost { get; set; }
        public decimal Commission { get; set; }
        public List<Room> Rooms { get; set; } = new();
        public Audit BookingAuditXML { get; set; }
        [XmlArrayItem("BookingXML")]
        public List<XmlDocument> BookingXMLs { get; set; } = new();
        [XmlArrayItem("TaskListItemDescription")]
        public List<string> TaskListItemDescriptions { get; set; } = new();

        public class Room
        {
            public int Seq { get; set; }
            public int RateCodeID { get; set; }
            public int PropertyRoomTypeID { get; set; }
            public int MealBasisID { get; set; }
            public decimal NetCost { get; set; }
            public decimal Commission { get; set; }
        }

        public class Audit
        {
            [XmlArrayItem("Room")]
            public List<AuditRoom> Rooms { get; set; }

            public partial class AuditRoom
            {
                public int Seq { get; set; }
                public int RateCodeID { get; set; }
                public string RateCode { get; set; }
                public List<Date> Dates { get; set; }
            }

            public class Date
            {
                public DateTime StayDate { get; set; }
                public int Day { get; set; }
                public string RateDefinition { get; set; }
                public decimal CommissionPercentage { get; set; }
                public decimal MarginPercentage { get; set; }
                public bool PackageRates { get; set; }
                public decimal TaxAmount { get; set; }
                public decimal DailyRate { get; set; }
                public RateBreakdown RateBreakdown { get; set; }
            }

            public class RateBreakdown
            {
                public decimal RoomRate { get; set; }
                public decimal PackageRate { get; set; }
                public decimal SingleRate { get; set; }
                public decimal DoubleRate { get; set; }
                public decimal TripleRate { get; set; }
                public decimal QuadRate { get; set; }
                public decimal ExtraAdultRate { get; set; }
                public decimal ChildRate { get; set; }
                public decimal FirstChildRate { get; set; }
                public decimal SecondChildRate { get; set; }

                public bool ShouldSerializeRoomRate()
                {
                    return RoomRate > 0m;
                }

                public bool ShouldSerializePackageRate()
                {
                    return PackageRate > 0m;
                }

                public bool ShouldSerializeSingleRate()
                {
                    return SingleRate > 0m;
                }

                public bool ShouldSerializeDoubleRate()
                {
                    return DoubleRate > 0m;
                }

                public bool ShouldSerializeTripleRate()
                {
                    return TripleRate > 0m;
                }

                public bool ShouldSerializeQuadRate()
                {
                    return QuadRate > 0m;
                }

                public bool ShouldSerializeExtraAdultRate()
                {
                    return ExtraAdultRate > 0m;
                }

                public bool ShouldSerializeChildRate()
                {
                    return ChildRate > 0m;
                }

                public bool ShouldSerializeFirstChildRate()
                {
                    return FirstChildRate > 0m;
                }

                public bool ShouldSerializeSecondChildRate()
                {
                    return SecondChildRate > 0m;
                }
            }
        }
    }
}
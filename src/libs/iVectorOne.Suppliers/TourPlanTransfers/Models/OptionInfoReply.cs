using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{

    [XmlRoot(ElementName = "OptionInfoReply")]
    public class OptionInfoReply
    {
        [XmlElement(ElementName = "Option")]
        public List<Option> Option { get; set; } = new List<Option>();
    }

    public class Option
    {
        public string Opt { get; set; }
        public int OptionNumber { get; set; }
        [XmlElement(ElementName = "OptGeneral")]
        public List<OptGeneral> OptGeneral { get; set; }
        [XmlElement(ElementName = "OptionNotes")]
        public OptionNotes OptionNotes { get; set; }
        [XmlElement(ElementName = "OptStayResults")]
        public OptStayResults OptStayResults { get; set; }
    }
    public class OptGeneral
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Locality { get; set; }
        public string LocalityDescription { get; set; }
        public string Class { get; set; }
        public string ClassDescription { get; set; }
        public string SType { get; set; }
        public int MPFUC { get; set; }
        public List<OptExtra> OptExtras { get; set; }
        public string VoucherName { get; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string Postcode { get; set; }
        public string InfantsAllowed { get; set; }
        public int Infant_From { get; set; }

        public int Infant_To { get; set; }
        public string ChildrenAllowed { get; set; }
        public int Child_From { get; set; }
        public int Child_To { get; set; }
        public string AdultsAllowed { get; set; }
        public int Adult_From { get; set; }
        public int Adult_To { get; set; }
        public string ButtonName { get; set; }
        public string DBAnalysisCode1 { get; set; }
        public string DBAnalysisDescription1 { get; set; }
        public string DBAnalysisCode2 { get; set; }
        public string DBAnalysisDescription2 { get; set; }
        public string DBAnalysisCode3 { get; set; }
        public string DBAnalysisDescription3 { get; set; }
        public string DBAnalysisCode4 { get; set; }
        public string DBAnalysisDescription4 { get; set; }
        public string DBAnalysisCode5 { get; set; }
        public string DBAnalysisDescription5 { get; set; }
        public string DBAnalysisCode6 { get; set; }
        public string DBAnalysisDescription6 { get; set; }
        public string LastUpdate { get; set; }


    }

    public class OptionNote
    {
        public string NoteCategory { get; set; }
        public string NoteText { get; set; }
        public string LastUpdate { get; set; }
    }

    [XmlRoot(ElementName = "OptionNotes")]
    public class OptionNotes
    {
        [XmlElement(ElementName = "OptionNote")]
        public List<OptionNote> OptionNote { get; set; }
    }
    public class OptStayResults
    {
        public string Availability { get; set; }
        public string Currency { get; set; }
        public int TotalPrice { get; set; }

        public decimal CommissionPercent { get; set; }

        public int AgentPrice { get; set; }
        public string RateId { get; set; }

        public string RateName { get; set; }

        [XmlElement(ElementName = "PeriodValueAdds")]
        public PeriodValueAdds PeriodValueAdds { get; set; }
        public int CancelHours { get; set; }
        public CancelPolicies CancelPolicies { get; set; }
    }

    [XmlRoot(ElementName = "PeriodValueAdds")]
    public class PeriodValueAdds
    {
        [XmlElement(ElementName = "PeriodValueAdd")]
        public PeriodValueAdd PeriodValueAdd { get; set; }
    }
    public class PeriodValueAdd
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string RateName { get; set; }

    }
    public class CancelPenalty
    {
        public Deadline Deadline { get; set; }
        public int InEffect { get; set; }
        public int LinePrice { get; set; }
        public int AgentPrice { get; set; }
    }

    public class Deadline
    {
        public int OffsetUnitMultiplier { get; set; }
        public string OffsetTimeUnit { get; set; }
        public DateTime DeadlineDateTime { get; set; }
    }

    public class OptExtra
    {
        public int SequenceNumber { get; set; }
        public string Description { get; set; }
        public string IsPricePerPerson { get; set; }
        public string ChargeBasis { get; set; }
        public string IsCompulsory { get; set; }

    }
}

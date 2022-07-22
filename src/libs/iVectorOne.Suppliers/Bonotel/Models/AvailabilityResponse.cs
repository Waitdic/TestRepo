namespace iVectorOne.CSSuppliers.Bonotel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;


    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("availabilityResponse")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class AvailabilityResponse
    {

        private Errors errorsField;

        private HotelList hotelListField;

        private string statusField;

        public Errors errors
        {
            get
            {
                return errorsField;
            }
            set
            {
                errorsField = value;
            }
        }

        public HotelList hotelList
        {
            get
            {
                return hotelListField;
            }
            set
            {
                hotelListField = value;
            }
        }

        [XmlAttribute()]
        public string status
        {
            get
            {
                return statusField;
            }
            set
            {
                statusField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Errors
    {

        private object codeField;

        private object descriptionField;

        public object code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        public object description
        {
            get
            {
                return descriptionField;
            }
            set
            {
                descriptionField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class HotelList
    {

        private List<Hotel> hotelField;

        [XmlElement(Type = typeof(Hotel), ElementName = "hotel")]
        public List<Hotel> hotel
        {
            get
            {
                return hotelField;
            }
            set
            {
                hotelField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType("hotel")]
    public partial class Hotel
    {

        private string hotelCodeField;

        private string nameField;

        private string addressField;

        private string cityField;

        private string stateProvinceField;

        private string countryField;

        private string postalCodeField;

        private string rateCurrencyCodeField;

        private string shortDescriptionField;

        private object starRatingField;

        private string thumbNailUrlField;

        private string hotelUrlField;

        private object maintenanceField;

        private object bookingPolicyField;

        private string policyDescriptionField;

        private List<RoomInformation> roomInformationField;

        public string hotelCode
        {
            get
            {
                return hotelCodeField;
            }
            set
            {
                hotelCodeField = value;
            }
        }

        public string name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        public string address
        {
            get
            {
                return addressField;
            }
            set
            {
                addressField = value;
            }
        }

        public string city
        {
            get
            {
                return cityField;
            }
            set
            {
                cityField = value;
            }
        }

        public string stateProvince
        {
            get
            {
                return stateProvinceField;
            }
            set
            {
                stateProvinceField = value;
            }
        }

        public string country
        {
            get
            {
                return countryField;
            }
            set
            {
                countryField = value;
            }
        }

        public string postalCode
        {
            get
            {
                return postalCodeField;
            }
            set
            {
                postalCodeField = value;
            }
        }

        public string rateCurrencyCode
        {
            get
            {
                return rateCurrencyCodeField;
            }
            set
            {
                rateCurrencyCodeField = value;
            }
        }

        public string shortDescription
        {
            get
            {
                return shortDescriptionField;
            }
            set
            {
                shortDescriptionField = value;
            }
        }

        public object starRating
        {
            get
            {
                return starRatingField;
            }
            set
            {
                starRatingField = value;
            }
        }

        public string thumbNailUrl
        {
            get
            {
                return thumbNailUrlField;
            }
            set
            {
                thumbNailUrlField = value;
            }
        }

        public string hotelUrl
        {
            get
            {
                return hotelUrlField;
            }
            set
            {
                hotelUrlField = value;
            }
        }

        public object maintenance
        {
            get
            {
                return maintenanceField;
            }
            set
            {
                maintenanceField = value;
            }
        }

        public object bookingPolicy
        {
            get
            {
                return bookingPolicyField;
            }
            set
            {
                bookingPolicyField = value;
            }
        }

        public string policyDescription
        {
            get
            {
                return policyDescriptionField;
            }
            set
            {
                policyDescriptionField = value;
            }
        }

        [XmlElement(Type = typeof(RoomInformation), ElementName = "roomInformation")]
        public List<RoomInformation> roomInformation
        {
            get
            {
                return roomInformationField;
            }
            set
            {
                roomInformationField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RoomInformation
    {

        private string roomNoField;

        private string roomCodeField;

        private string roomTypeCodeField;

        private string roomTypeField;

        private string roomDescriptionField;

        private string bedTypeCodeField;

        private string bedTypeField;

        private string stdAdultsField;

        private string promotionCodeField;

        private string confirmationTypeField;

        private object confirmationConditionsField;

        private RoomBookingPolicy[] roomBookingPolicyField;

        private RateInformation rateInformationField;

        private string[] textField;

        public string roomNo
        {
            get
            {
                return roomNoField;
            }
            set
            {
                roomNoField = value;
            }
        }

        public string roomCode
        {
            get
            {
                return roomCodeField;
            }
            set
            {
                roomCodeField = value;
            }
        }

        public string roomTypeCode
        {
            get
            {
                return roomTypeCodeField;
            }
            set
            {
                roomTypeCodeField = value;
            }
        }

        public string roomType
        {
            get
            {
                return roomTypeField;
            }
            set
            {
                roomTypeField = value;
            }
        }

        public string roomDescription
        {
            get
            {
                return roomDescriptionField;
            }
            set
            {
                roomDescriptionField = value;
            }
        }

        public string bedTypeCode
        {
            get
            {
                return bedTypeCodeField;
            }
            set
            {
                bedTypeCodeField = value;
            }
        }

        public string bedType
        {
            get
            {
                return bedTypeField;
            }
            set
            {
                bedTypeField = value;
            }
        }

        public string stdAdults
        {
            get
            {
                return stdAdultsField;
            }
            set
            {
                stdAdultsField = value;
            }
        }

        public string promotionCode
        {
            get
            {
                return promotionCodeField;
            }
            set
            {
                promotionCodeField = value;
            }
        }

        public string confirmationType
        {
            get
            {
                return confirmationTypeField;
            }
            set
            {
                confirmationTypeField = value;
            }
        }

        public object confirmationConditions
        {
            get
            {
                return confirmationConditionsField;
            }
            set
            {
                confirmationConditionsField = value;
            }
        }

        [XmlElement("roomBookingPolicy")]
        public RoomBookingPolicy[] roomBookingPolicy
        {
            get
            {
                return roomBookingPolicyField;
            }
            set
            {
                roomBookingPolicyField = value;
            }
        }

        public RateInformation rateInformation
        {
            get
            {
                return rateInformationField;
            }
            set
            {
                rateInformationField = value;
            }
        }

        [XmlText()]
        public string[] Text
        {
            get
            {
                return textField;
            }
            set
            {
                textField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RoomBookingPolicy
    {

        private DateTime policyFromField;

        private DateTime policyToField;

        private string amendmentTypeField;

        private string policyBasedOnField;

        private string policyBasedOnValueField;

        private string cancellationTypeField;

        private string stayDateRequirementField;

        private string arrivalRangeField;

        private string arrivalRangeValueField;

        private string policyFeeField;

        private string noShowBasedOnField;

        private string noShowBasedOnValueField;

        private string noShowPolicyFeeField;

        [XmlElement(DataType = "date")]
        public DateTime policyFrom
        {
            get
            {
                return policyFromField;
            }
            set
            {
                policyFromField = value;
            }
        }

        [XmlElement(DataType = "date")]
        public DateTime policyTo
        {
            get
            {
                return policyToField;
            }
            set
            {
                policyToField = value;
            }
        }

        public string amendmentType
        {
            get
            {
                return amendmentTypeField;
            }
            set
            {
                amendmentTypeField = value;
            }
        }

        public string policyBasedOn
        {
            get
            {
                return policyBasedOnField;
            }
            set
            {
                policyBasedOnField = value;
            }
        }

        public string policyBasedOnValue
        {
            get
            {
                return policyBasedOnValueField;
            }
            set
            {
                policyBasedOnValueField = value;
            }
        }

        public string cancellationType
        {
            get
            {
                return cancellationTypeField;
            }
            set
            {
                cancellationTypeField = value;
            }
        }

        public string stayDateRequirement
        {
            get
            {
                return stayDateRequirementField;
            }
            set
            {
                stayDateRequirementField = value;
            }
        }

        public string arrivalRange
        {
            get
            {
                return arrivalRangeField;
            }
            set
            {
                arrivalRangeField = value;
            }
        }

        public string arrivalRangeValue
        {
            get
            {
                return arrivalRangeValueField;
            }
            set
            {
                arrivalRangeValueField = value;
            }
        }

        public string policyFee
        {
            get
            {
                return policyFeeField;
            }
            set
            {
                policyFeeField = value;
            }
        }

        public string noShowBasedOn
        {
            get
            {
                return noShowBasedOnField;
            }
            set
            {
                noShowBasedOnField = value;
            }
        }

        public string noShowBasedOnValue
        {
            get
            {
                return noShowBasedOnValueField;
            }
            set
            {
                noShowBasedOnValueField = value;
            }
        }

        public string noShowPolicyFee
        {
            get
            {
                return noShowPolicyFeeField;
            }
            set
            {
                noShowPolicyFeeField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RateInformation
    {

        private string ratePlanCodeField;

        private string ratePlanField;

        private decimal avarageRateField;

        private decimal totalRateField;

        private NightlyRate[] dailyRatesField;

        private List<Tax> taxInformationField;

        public string ratePlanCode
        {
            get
            {
                return ratePlanCodeField;
            }
            set
            {
                ratePlanCodeField = value;
            }
        }

        public string ratePlan
        {
            get
            {
                return ratePlanField;
            }
            set
            {
                ratePlanField = value;
            }
        }

        public decimal avarageRate
        {
            get
            {
                return avarageRateField;
            }
            set
            {
                avarageRateField = value;
            }
        }

        public decimal totalRate
        {
            get
            {
                return totalRateField;
            }
            set
            {
                totalRateField = value;
            }
        }

        [XmlArrayItem("nightlyRate", IsNullable = false)]
        public NightlyRate[] dailyRates
        {
            get
            {
                return dailyRatesField;
            }
            set
            {
                dailyRatesField = value;
            }
        }

        [XmlArray("taxInformation")]
        [XmlArrayItem("tax")]
        public List<Tax> taxInformation
        {
            get
            {
                return taxInformationField;
            }
            set
            {
                taxInformationField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class NightlyRate
    {

        private decimal stdAdultRateField;

        private decimal additionalAdultRateField;

        private decimal totalField;

        private object rateCodeField;

        private string dateField;

        private string dailyConditionField;

        public decimal stdAdultRate
        {
            get
            {
                return stdAdultRateField;
            }
            set
            {
                stdAdultRateField = value;
            }
        }

        public decimal additionalAdultRate
        {
            get
            {
                return additionalAdultRateField;
            }
            set
            {
                additionalAdultRateField = value;
            }
        }

        public decimal total
        {
            get
            {
                return totalField;
            }
            set
            {
                totalField = value;
            }
        }

        public object rateCode
        {
            get
            {
                return rateCodeField;
            }
            set
            {
                rateCodeField = value;
            }
        }

        [XmlAttribute()]
        public string date
        {
            get
            {
                return dateField;
            }
            set
            {
                dateField = value;
            }
        }

        [XmlAttribute()]
        public string dailyCondition
        {
            get
            {
                return dailyConditionField;
            }
            set
            {
                dailyConditionField = value;
            }
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Tax
    {

        private decimal taxAmountField;

        private string taxNameField;

        public decimal taxAmount
        {
            get
            {
                return taxAmountField;
            }
            set
            {
                taxAmountField = value;
            }
        }

        [XmlAttribute()]
        public string taxName
        {
            get
            {
                return taxNameField;
            }
            set
            {
                taxNameField = value;
            }
        }
    }
}
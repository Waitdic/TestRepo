namespace ThirdParty.Services
{
    using ThirdParty.Constants;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.SDK.V2;
    using ThirdParty.SDK.V2.PropertyBook;

    /// <summary>Class to Validate which suppliers need which supplier references</summary>
    public class SuppierReferenceValidator : ISuppierReferenceValidator
    {
        // todo - change from source string to IThirdParty setting
        /// <summary>Validates the supplier references needed for each supplier at book.</summary>
        /// <param name="details">The property details.</param>
        /// <param name="bookRequest">The booking request.</param>
        public void ValidateBook(PropertyDetails details, Request bookRequest)
        {
            var tpRef1Required = false;
            var tpRef2Required = false;

            switch (details.Source)
            {
                case ThirdParties.ACEROOMS:
                case ThirdParties.BONOTEL:
                case ThirdParties.DERBYSOFTCLUBMED:
                case ThirdParties.DERBYSOFTSYNXIS:
                case ThirdParties.DERBYSOFTMARRIOTT:
                case ThirdParties.DOTW:
                case ThirdParties.GOGLOBAL:
                case ThirdParties.HOTELBEDSV2:
                case ThirdParties.JONVIEW:
                case ThirdParties.JUMBO:
                case ThirdParties.MTS:
                case ThirdParties.SUNHOTELS:
                case ThirdParties.STUBA:
                case ThirdParties.YALAGO:
                case ThirdParties.YOUTRAVEL:
                    break; // neither required
                case ThirdParties.BOOKABED:
                case ThirdParties.IMPERATORE:
                case ThirdParties.JUNIPERECTRAVEL:
                case ThirdParties.JUNIPERELEVATE:
                case ThirdParties.JUNIPERFASTPAYHOTELS:
                case ThirdParties.OWNSTOCK:
                case ThirdParties.TEAMAMERICA:
                case ThirdParties.TRAVELGATEARABIANA:
                case ThirdParties.TRAVELGATEBOOKOHOTEL:
                case ThirdParties.TRAVELGATEDARINA:
                case ThirdParties.TRAVELGATEDERBYSOFT:
                case ThirdParties.TRAVELGATEDERBYSOFTBESTWESTERN:
                case ThirdParties.TRAVELGATEDERBYSOFTIHG:
                case ThirdParties.TRAVELGATEDERBYSOFTNAVH:
                case ThirdParties.TRAVELGATEDERBYSOFTUOR:
                case ThirdParties.TRAVELGATEDINGUS:
                case ThirdParties.TRAVELGATEDINGUSBLUESEA:
                case ThirdParties.TRAVELGATEDINGUSSPRINGHOTELS:
                case ThirdParties.TRAVELGATEDINGUSTHB:
                case ThirdParties.TRAVELGATEDOTWV3:
                case ThirdParties.TRAVELGATEEETGLOBAL:
                case ThirdParties.TRAVELGATEEUROPLAYAS:
                case ThirdParties.TRAVELGATEGEKKO:
                case ThirdParties.TRAVELGATEHOTELTRADER:
                case ThirdParties.TRAVELGATEIXPIRA:
                case ThirdParties.TRAVELGATEMETHABOOK:
                case ThirdParties.TRAVELGATEOSWALDARRIGO:
                case ThirdParties.TRAVELGATEPERLATOURS:
                case ThirdParties.TRAVELGATETBO:
                case ThirdParties.TRAVELGATETRAVELLANDA:
                case ThirdParties.TRAVELGATETRAVELTINO:
                case ThirdParties.TRAVELGATEVIAJESOLYMPIA:
                case ThirdParties.TRAVELGATEWHL:
                case ThirdParties.TRAVELGATEYALAGO:
                    tpRef1Required = true;
                    tpRef2Required = false;
                    break; // only tpref1 required
                case ThirdParties.EXPEDIARAPID:
                    tpRef1Required = true;
                    tpRef2Required = true;
                    break; // both required
                default:
                    tpRef1Required = true;
                    tpRef2Required = true;
                    break;
            }

            if (tpRef1Required && string.IsNullOrWhiteSpace(details.TPRef1))
            {
                details.Warnings.AddNew("Validation Failure", WarningMessages.InvalidSupplierReference1);
            }

            if (tpRef2Required && string.IsNullOrWhiteSpace(details.TPRef2))
            {
                details.Warnings.AddNew("Validation Failure", WarningMessages.InvalidSupplierReference2);
            }

            ValidateBookingReference(details, bookRequest);
        }

        /// <summary>Validates the supplier references needed for each supplier at book.</summary>
        /// <param name="details">The property details.</param>
        public void ValidateCancel(PropertyDetails details)
        {
            var tpRef1Required = false;
            var sourceSecondaryReferenceRequired = false;

            switch (details.Source)
            {
                case ThirdParties.EXPEDIARAPID:
                    tpRef1Required = true;
                    sourceSecondaryReferenceRequired = true;
                    break;
                case ThirdParties.BOOKABED:
                case ThirdParties.IMPERATORE:
                case ThirdParties.OWNSTOCK:
                case ThirdParties.STUBA:
                case ThirdParties.HOTELBEDSV2:
                case ThirdParties.MTS:
                case ThirdParties.YOUTRAVEL:
                case ThirdParties.BONOTEL:
                case ThirdParties.JONVIEW:
                case ThirdParties.ACEROOMS:
                case ThirdParties.DOTW:
                case ThirdParties.SUNHOTELS:
                case ThirdParties.YALAGO:
                case ThirdParties.JUMBO:
                case ThirdParties.GOGLOBAL:
                case ThirdParties.TEAMAMERICA:
                case ThirdParties.JUNIPERECTRAVEL:
                case ThirdParties.JUNIPERELEVATE:
                case ThirdParties.JUNIPERFASTPAYHOTELS:
                    break;
                case ThirdParties.TRAVELGATEVIAJESOLYMPIA:
                case ThirdParties.TRAVELGATEARABIANA:
                case ThirdParties.TRAVELGATEEUROPLAYAS:
                case ThirdParties.TRAVELGATEDARINA:
                case ThirdParties.TRAVELGATEBOOKOHOTEL:
                case ThirdParties.TRAVELGATEDERBYSOFT:
                case ThirdParties.TRAVELGATEDINGUSBLUESEA:
                case ThirdParties.TRAVELGATEDINGUSSPRINGHOTELS:
                case ThirdParties.TRAVELGATEDINGUSTHB:
                case ThirdParties.TRAVELGATEDOTWV3:
                case ThirdParties.TRAVELGATEEETGLOBAL:
                case ThirdParties.TRAVELGATEGEKKO:
                case ThirdParties.TRAVELGATEHOTELTRADER:
                case ThirdParties.TRAVELGATEIXPIRA:
                case ThirdParties.TRAVELGATEMETHABOOK:
                case ThirdParties.TRAVELGATEOSWALDARRIGO:
                case ThirdParties.TRAVELGATEPERLATOURS:
                case ThirdParties.TRAVELGATETRAVELLANDA:
                case ThirdParties.TRAVELGATEWHL:
                case ThirdParties.TRAVELGATEYALAGO:
                case ThirdParties.TRAVELGATEDERBYSOFTBESTWESTERN:
                case ThirdParties.TRAVELGATEDERBYSOFTIHG:
                case ThirdParties.TRAVELGATEDERBYSOFTNAVH:
                case ThirdParties.TRAVELGATEDERBYSOFTUOR:
                case ThirdParties.TRAVELGATEDINGUS:
                case ThirdParties.TRAVELGATETRAVELTINO:
                case ThirdParties.TRAVELGATETBO:
                    tpRef1Required = true;
                    sourceSecondaryReferenceRequired = false;
                    break;
                case ThirdParties.DERBYSOFTSYNXIS:
                case ThirdParties.DERBYSOFTCLUBMED:
                case ThirdParties.DERBYSOFTMARRIOTT:
                    tpRef1Required = true;
                    sourceSecondaryReferenceRequired = false;
                    break;
                default:
                    tpRef1Required = true;
                    sourceSecondaryReferenceRequired = true;
                    break;
            }

            if (tpRef1Required && string.IsNullOrWhiteSpace(details.TPRef1))
            {
                details.Warnings.AddNew("Validation Failure", WarningMessages.InvalidSupplierReference1);
            }

            if (sourceSecondaryReferenceRequired && string.IsNullOrWhiteSpace(details.SourceSecondaryReference))
            {
                details.Warnings.AddNew("Validation Failure", WarningMessages.InvalidSupplierReference2);
            }
        }

        /// <summary>Validates the booking reference.</summary>
        /// <param name="details">The property details.</param>
        /// <param name="bookRequest">The booking request.</param>
        public void ValidateBookingReference(PropertyDetails details, Request bookRequest)
        {
            int maxLength = 0;
            switch (details.Source)
            {
                case ThirdParties.EXPEDIARAPID:
                    maxLength = 28;
                    break;
                default:
                    break;
            }

            if (maxLength != 0 && bookRequest.BookingReference.Length > maxLength)
            {
                details.Warnings.AddNew("Validation Failure", WarningMessages.InvalidBookingReferenceLength + maxLength);
            }
        }
    }
}

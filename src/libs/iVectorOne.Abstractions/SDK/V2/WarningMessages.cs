﻿namespace iVectorOne.SDK.V2
{
    /// <summary>A class of common warning messages for request validation</summary>
    public class WarningMessages
    {
        /// <summary>
        /// At least one property reference must be specified
        /// </summary>
        public const string PropertyNotSpecified = "At least one Property must be specified";

        /// <summary>
        /// A maximum of 500 properties can be passed into any search
        /// </summary>
        public const string PropertiesOverLimit = "A maximum of 500 properties can be provided";

        /// <summary>
        /// An Arrival Date must be specified
        /// </summary>
        public const string ArrivalDateNotSpecified = "An Arrival Date must be specified";

        /// <summary>
        /// An Arrival Date on or after today must be specified
        /// </summary>
        public const string ArrivalDateInThePast = "An Arrival Date on or after today must be specified";

        /// <summary>
        /// An Arrival Date on or after today must be specified
        /// </summary>
        public const string ArrivalDateToFarInTheFuture = "The arrival date can not be more than 3 years in the future";

        /// <summary>
        /// A Duration must be specified
        /// </summary>
        public const string DurationNotSpecified = "A Duration must be specified";

        /// <summary>
        /// A Duration must be specified
        /// </summary>
        public const string DurationInvalid = "A Duration between 1 and 63 must be specified";

        /// <summary>
        /// At least one Room must be specified
        /// </summary>
        public const string RoomsNotSpecified = "At least one Room must be specified";

        /// <summary>
        /// At least one Adult per room must be specified
        /// </summary>
        public const string AdultsNotSpecifiedInAllRooms = "At least one Adult per room must be specified";

        /// <summary>
        /// A Maximum of 15 adults can be specified in a single room"
        /// </summary>
        public const string Only15AdultsAllowed = "A Maximum of 15 adults can be specified in a single room";

        /// <summary>
        /// At least one Adult per room must be specified
        /// </summary>
        public const string Only8ChildrenAllowed = "A Maximum of 8 children can be specified in a single room";

        /// <summary>
        /// At least one Adult per room must be specified
        /// </summary>
        public const string Only7InfantsAllowed = "A Maximum of 7 infants can be specified in a single room";

        /// <summary>
        /// The number of Child Ages must match the number of Children
        /// </summary>
        public const string ChildAgesDontMatchChildren = "The number of Child Ages must match the number of Children";

        /// <summary>
        /// A valid Child Age must be specified for every Child
        /// </summary>
        public const string InvalidChildAge = "A valid Child Age between 2 and 17 must be specified for every Child";

        /// <summary>
        ///   <para>At least one guest details must be included in every book request</para>
        /// </summary>
        public const string InvalidGuestDetails = "At least one Guest Details is required";

        /// <summary>
        ///   <para>A valid type must be specified for every guest</para>
        /// </summary>
        public const string InvalidPaxType = "A Type of Child, Adult or Infant is required for all Guests";

        /// <summary>
        ///   <para>A valid Title must be specified for every guest</para>
        /// </summary>
        public const string InvalidGuestTitle = "Every Guest must have a title";

        /// <summary>
        ///   <para>A valid First Name must be specified for every guest</para>
        /// </summary>
        public const string InvalidGuestFirstName = "Every Guest must have a First Name";

        /// <summary>
        ///   <para>A valid Last Name must be specified for every guest</para>
        /// </summary>
        public const string InvalidGuestLastName = "Every Guest must have a Last Name";

        /// <summary>
        ///   <para>A valid Date of Birth must be specified for every guest</para>
        /// </summary>
        public const string InvalidDateOfBirth = "Every Guest must have a valid Date of Birth";

        /// <summary>
        ///   <para>Every prebook and book call must include one room booking</para>
        /// </summary>
        public const string InvalidRoomBookings = "At least one Room Booking is required";

        /// <summary>
        ///   <para>Every prebook and book call must include a booking token</para>
        /// </summary>
        public const string InvalidBookingToken = "A valid Booking Token is required";

        /// <summary>
        ///   <para>Every Room Booking must include a valid bookign token</para>
        /// </summary>
        public const string InvalidRoomBookingToken = "A valid Booking Token is required for every Room Booking";

        /// <summary>
        ///   <para>Every prebook and book call must include a booking token</para>
        /// </summary>
        public const string InvalidBookingReference= "A Booking Reference is required";

        /// <summary>
        ///   <para>Every cancel request must have a supplier booking reference</para>
        /// </summary>
        public const string InvalidSupplierBookingReference = "A Supplier Booking Reference is required";

        /// <summary>
        ///   <para>Every book request must have a supplier reference 1</para>
        /// </summary>
        public const string InvalidSupplierReference1 = "A Supplier Reference 1 is required";

        /// <summary>
        ///   <para>Every book request must have a supplier reference 2</para>
        /// </summary>
        public const string InvalidSupplierReference2 = "A Supplier Reference 2 is required";

        /// <summary>
        ///   <para>Every room request must have a supplier booking reference</para>
        /// </summary>
        public const string InvalidRoomSupplierBookingReference = "A Supplier Reference is required for each Room";

        /// <summary>
        ///   <para>Every room request must have a supplier booking reference</para>
        /// </summary>
        public const string InvalidRoomSupplierBooking1Reference = "A Supplier Reference 1 is required for each Room";

        /// <summary>
        ///   <para>Every room request must have a supplier booking reference</para>
        /// </summary>
        public const string InvalidRoomSupplierBooking2Reference = "A Supplier Reference 2 is required for each Room";

        /// <summary>
        ///   <para>Every book call must include a lead guest</para>
        /// </summary>
        public const string InvalidLeadCustomer = "A Lead customer is required";

        /// <summary>
        ///   <para>A valid Title must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidLeadGuestTitle = "The Lead Guest must have a title";

        /// <summary>
        ///   <para>A valid First Name must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidLeadGuestFirstName = "The Lead Guest must have a First Name";

        /// <summary>
        ///   <para>A valid Last Name must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidLeadGuestLastName = "The Lead Guest must have a Last Name";

        /// <summary>
        ///   <para>A valid DoB must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidLeadGuestDateOfBirth = "The Lead Guest must have a Date of Birth";

        /// <summary>
        ///   <para>A valid CustomerAddress1 must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerAddress1 = "The Lead Guest must have a CustomerAddress1";

        /// <summary>
        ///   <para>A valid CustomerTownCity must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerTownCity = "The Lead Guest must have a CustomerTownCity";

        /// <summary>
        ///   <para>A valid CustomerCounty must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerCounty = "The Lead Guest must have a CustomerCounty";

        /// <summary>
        ///   <para>A valid CustomerPostcode must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerPostcode = "The Lead Guest must have a CustomerPostcode";

        /// <summary>
        ///   <para>A valid CustomerBookingCountryCode must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerBookingCountryCode = "The Lead Guest must have a CustomerBookingCountryCode";

        /// <summary>
        ///   <para>A valid CustomerPhone must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerPhone = "The Lead Guest must have a CustomerPhone and it must be a valid phone number";

        /// <summary>
        ///   <para>A valid CustomerMobile must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerMobile = "The Lead Guest must have a CustomerMobile and it must be a valid phone number";

        /// <summary>
        ///   <para>A valid CustomerEmail must be specified for the lead guest</para>
        /// </summary>
        public const string InvalidCustomerEmail = "The Lead Guest must have a CustomerEmail and it must be a valid email address";

        /// <summary>
        ///   <para>At least one property identifier must be passed into every request</para>
        /// </summary>
        public const string InvalidPropertyID = "At least one Property ID must be provided";

        /// <summary>
        ///   <para>The Rooms must match those passed in at search</para>
        /// </summary>
        public const string InvalidRooms = "The number of rooms in the request must match what was searched for";


        /// <summary>
        ///   <para>The booking reference cannot exceed the supplier permitted length</para>
        /// </summary>
        public const string InvalidBookingReferenceLength = "The booking reference exceeds the supplier permitted length, the maximum allowed length is ";

    }   
}
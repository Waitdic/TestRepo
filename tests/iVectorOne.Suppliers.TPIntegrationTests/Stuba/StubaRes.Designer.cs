﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.Stuba {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class StubaRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StubaRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.Stuba.StubaRes", typeof(StubaRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URL: http://www.stubademo.com/RXLStagingServices/ASMX/XmlService.asmx
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;AvailabilitySearch&gt;&lt;Authority&gt;&lt;Org&gt;travelpack&lt;/Org&gt;&lt;User&gt;xmltest&lt;/User&gt;&lt;Password&gt;xmltest&lt;/Password&gt;&lt;Currency&gt;GBP&lt;/Currency&gt;&lt;Version&gt;1.28&lt;/Version&gt;&lt;/Authority&gt;&lt;RegionId&gt;MAD_12&lt;/RegionId&gt;&lt;Hotels&gt;&lt;Id&gt;102245&lt;/Id&gt;&lt;/Hotels&gt;&lt;HotelStayDetails&gt;&lt;ArrivalDate&gt;2021-09-01&lt;/ArrivalDate&gt;&lt;Nights&gt;5&lt;/Nights&gt;&lt;Nationality&gt;GB&lt;/Nationality&gt;&lt;Room&gt;&lt;Guests&gt;&lt;Adult/&gt;&lt;/Guests&gt;&lt;/Room&gt;&lt;/HotelStayDetails&gt;&lt;/AvailabilitySe [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;AvailabilitySearchResult xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;&lt;Currency&gt;GBP&lt;/Currency&gt;&lt;Warning&gt;&lt;/Warning&gt;&lt;TestMode&gt;true&lt;/TestMode&gt;&lt;HotelAvailability hotelQuoteId=&quot;21470068_61412&quot;&gt;&lt;Hotel id=&quot;61412&quot; name=&quot;Paramount&quot; /&gt;&lt;Result id=&quot;21470068-0&quot;&gt;&lt;Room&gt;&lt;RoomType code=&quot;2174054&quot; text=&quot;Luxury Room&quot; /&gt;&lt;MealType code=&quot;1&quot; text=&quot;Breakfast&quot; /&gt;&lt;Price amt=&quot;697.50&quot; /&gt;&lt;Extras&gt;&lt;Extra name=&quot;Cot&quot; /&gt;&lt;/Extras&gt;&lt;Messages /&gt;&lt;Cancellat [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ResponseString {
            get {
                return ResourceManager.GetString("ResponseString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;&lt;Results&gt;&lt;Result MID=&quot;61412&quot; PRBID=&quot;1&quot; MBC=&quot;1&quot; TPK=&quot;61412&quot; CC=&quot;GBP&quot; RT=&quot;Luxury Room&quot; AMT=&quot;697.50&quot; TPR=&quot;21470068_61412|21470068-0&quot; RTC=&quot;2174054&quot; NRF=&quot;0&quot; /&gt;&lt;Result MID=&quot;61412&quot; PRBID=&quot;1&quot; MBC=&quot;1000019&quot; TPK=&quot;61412&quot; CC=&quot;GBP&quot; RT=&quot;Luxury Room&quot; AMT=&quot;775.00&quot; TPR=&quot;21470068_61412|21470068-1&quot; RTC=&quot;2174054&quot; NRF=&quot;0&quot; /&gt;&lt;Result MID=&quot;61412&quot; PRBID=&quot;1&quot; MBC=&quot;&quot; TPK=&quot;61412&quot; CC=&quot;GBP&quot; RT=&quot;Luxury Room&quot; AMT=&quot;581.50&quot; TPR=&quot;21470068_61412|21470068-2&quot; RTC=&quot;2174054&quot; NRF=&quot;0&quot; /&gt;&lt;Result MID=&quot;61412&quot; PRBI [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResultXML {
            get {
                return ResourceManager.GetString("TransformedResultXML", resourceCulture);
            }
        }
    }
}

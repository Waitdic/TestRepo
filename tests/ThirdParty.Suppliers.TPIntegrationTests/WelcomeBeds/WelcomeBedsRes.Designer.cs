﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThirdParty.Tests.WelcomeBeds {
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
    internal class WelcomeBedsRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WelcomeBedsRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThirdParty.Tests.WelcomeBeds.WelcomeBedsRes", typeof(WelcomeBedsRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://test.gsisservices.com:80/GServices/WSHOT-PS
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soapenv:Envelope xmlns:ns0=&quot;http://www.opentravel.org/OTA/2003/05&quot; xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;&lt;soapenv:Header /&gt;&lt;soapenv:Body&gt;&lt;ns0:OTA_HotelAvailRQ Version=&quot;1&quot;&gt;&lt;ns0:AvailRequestSegments&gt;&lt;ns0:AvailRequestSegment&gt;&lt;ns0:StayDateRange Start=&quot;2021-09-01&quot; End=&quot;2021-09-06&quot; /&gt;&lt;ns0:RoomStayCandidates&gt;&lt;ns0:RoomStayCandidate&gt;&lt;ns0:GuestCounts&gt;&lt;ns0:GuestCount Count=&quot;1&quot; Age=&quot;30&quot; /&gt;&lt;/ns0: [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;soapenv:Envelope xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;
        ///    &lt;soapenv:Body&gt; 
        ///        &lt;ns0:OTA_HotelAvailRS Version=&quot;1&quot; xmlns:ns0=&quot;http://www.opentravel.org/OTA/2003/05&quot;&gt; 
        ///            &lt;ns0:Success/&gt; 
        ///            &lt;ns0:POS&gt; 
        ///                &lt;ns0:Source&gt; 
        ///                    &lt;ns0:BookingChannel Type=&quot;&quot;/&gt; 
        ///                &lt;/ns0:Source&gt; 
        ///            &lt;/ns0:POS&gt; 
        ///            &lt;ns0:Errors/&gt; 
        ///            &lt;ns0:RoomStays&gt; 
        ///                &lt;ns0:RoomStay&gt; 
        ///                    &lt;ns0:RoomTypes [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;7019&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC=&quot;DBL ES&quot; RT=&quot;DOUBLE-STANDARD&quot; MBC=&quot;AD&quot; AMT=&quot;502.92&quot; TPR=&quot;eJxdTtEKgzAM/JWBLxpYaTprWnzoqq1DkCq2sv//k9XOvQxy4eCSu5NcMC3rcew+HtMNji7O/DHDoIjouB0EWzBDcvthI95QH5fjTFAHDVYB+GdYJoTjNX08qEqtBz8Y05bGNdYeCj7TPyp8KRikx2vkA6R5QK5B1NakahF0xOxtlMPWcum/wD3RC0D|DBL ES|AD|TMP_CODE-S|EUR&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs /&gt;&lt;/Result&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;7019&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}
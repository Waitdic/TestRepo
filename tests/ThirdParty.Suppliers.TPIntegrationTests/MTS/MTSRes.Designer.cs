﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThirdParty.Tests.MTS {
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
    internal class MTSRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MTSRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThirdParty.Tests.MTS.MTSRes", typeof(MTSRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://intesb.axisdata.net:54321/apu-test/ota
        ///
        ///**HEADERS**
        ///
        ///Content-Type: application/json
        ///Host: intesb.axisdata.net:54321
        ///Content-Length: 798
        ///Expect: 100-continue
        ///Connection: Close
        ///Timeout: 58
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;OTA_HotelAvailRQ xmlns = &quot;http://www.opentravel.org/OTA/2003/05&quot; Version = &quot;0.1&quot;&gt;&lt;POS&gt;&lt;Source&gt;&lt;RequestorID Instance = &quot;MF001&quot; ID_Context = &quot;AxisData&quot; ID = &quot;TEST&quot; Type = &quot;22&quot;/&gt;&lt;BookingChannel Type = &quot;2&quot;/&gt;&lt;/Source&gt;&lt;Source&gt;&lt;RequestorID Type=&quot;88&quot; ID=&quot;TEST&quot; MessagePassword=&quot;testpass&quot;/&gt;&lt;/ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;?&gt;&lt;OTA_HotelAvailRS xmlns=&quot;http://www.opentravel.org/OTA/2003/05&quot; Version=&quot;0.1&quot; TransactionIdentifier=&quot;1-1/1&quot;&gt;&lt;Success /&gt;&lt;HotelStays&gt;&lt;HotelStay&gt;&lt;BasicPropertyInfo HotelCode=&quot;102245&quot; HotelName=&quot;Eden Nord Hotel&quot; HotelCodeContext=&quot;22290&quot; AreaID=&quot;0937PU&quot;&gt;&lt;VendorMessages&gt;&lt;VendorMessage Title=&quot;Images&quot;&gt;&lt;SubSection&gt;&lt;Paragraph&gt;&lt;Image&gt;102245_915002.jpg&lt;/Image&gt;&lt;/Paragraph&gt;&lt;/SubSection&gt;&lt;/VendorMessage&gt;&lt;/VendorMessages&gt;&lt;Award Rating=&quot;3 Stars&quot; /&gt;&lt;/BasicPropertyInfo&gt;&lt;/H [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ResponseXML {
            get {
                return ResourceManager.GetString("ResponseXML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;&lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;102245&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; NRF=&quot;0&quot; RT=&quot;Single Standard Room (Balcony)&quot; MBC=&quot;BB&quot; AD=&quot;0&quot; CH=&quot;0&quot; hlpCHA=&quot;&quot; INF=&quot;0&quot; AMT=&quot;669.00&quot; TPR=&quot;RMSDSG00B0|BB|Spain&quot; RTC=&quot;RMSDSG00B0&quot; /&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;102245&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; NRF=&quot;0&quot; RT=&quot;Single Standard Room (Balcony)&quot; MBC=&quot;HB&quot; AD=&quot;0&quot; CH=&quot;0&quot; hlpCHA=&quot;&quot; INF=&quot;0&quot; AMT=&quot;691.73&quot; TPR=&quot;RMSDSG00B0|HB|Spain&quot; RTC=&quot;RMSDSG00B0&quot; /&gt;&lt;/Results&gt;.
        /// </summary>
        internal static string TransformedResultXML {
            get {
                return ResourceManager.GetString("TransformedResultXML", resourceCulture);
            }
        }
    }
}

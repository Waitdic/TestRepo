﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.Jumbo {
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
    internal class JumboRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal JumboRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.Jumbo.JumboRes", typeof(JumboRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://live.xtravelsystem.com/public/v1_0rc1/hotelBookingHandler
        ///
        ///**HEADERS**
        ///
        ///Accept-Encoding: gzip
        ///Content-Type: text/xml
        ///Host: live.xtravelsystem.com
        ///Content-Length: 974
        ///Expect: 100-continue
        ///Connection: Close
        ///Timeout: 58
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soapenv:Envelope xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot; xmlns:typ=&quot;http://xtravelsystem.com/v1_0rc1/hotel/types&quot;&gt;&lt;soapenv:Header/&gt;&lt;soapenv:Body&gt;&lt;typ:availableHotelsByMultiQueryV12&gt;&lt;AvailableHotelsByMultiQueryRQ_1&gt;&lt;agencyCode&gt;243008&lt;/ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLogs {
            get {
                return ResourceManager.GetString("RequestLogs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;env:Envelope xmlns:env=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:enc=&quot;http://schemas.xmlsoap.org/soap/encoding/&quot; xmlns:ns0=&quot;http://xtravelsystem.com/v1_0rc1/hotel/types&quot;&gt;
        ///  &lt;env:Body&gt;
        ///    &lt;ns0:availableHotelsByMultiQueryV12Response&gt;
        ///      &lt;result&gt;
        ///        &lt;fromRow&gt;0&lt;/fromRow&gt;
        ///        &lt;numRows&gt;1000&lt;/numRows&gt;
        ///        &lt;totalRows&gt;1&lt;/totalRows&gt;
        ///        &lt;availa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;7172&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RT=&quot;SINGLE&quot; RTC=&quot;0&quot; MBC=&quot;BB&quot; AD=&quot;0&quot; CH=&quot;0&quot; hlpCHA=&quot;0&quot; INF=&quot;0&quot; AMT=&quot;632.36&quot; TPR=&quot;SGL|BB&quot; NRF=&quot;0&quot; /&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;7172&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RT=&quot;SINGLE&quot; RTC=&quot;0&quot; MBC=&quot;HB&quot; AD=&quot;0&quot; CH=&quot;0&quot; hlpCHA=&quot;0&quot; INF=&quot;0&quot; AMT=&quot;775.6&quot; TPR=&quot;SGL|HB&quot; NRF=&quot;0&quot; /&gt;&lt;/Results&gt;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}

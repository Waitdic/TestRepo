﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.Hotelston {
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
    internal class HotelstonRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal HotelstonRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.Hotelston.HotelstonRes", typeof(HotelstonRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://dev.hotelston.com/ws/HotelService.HotelServiceHttpSoap11Endpoint/
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soapenv:Envelope xmlns:xsd=&quot;http://request.ws.hotelston.com/xsd&quot; xmlns:xsd1=&quot;http://types.ws.hotelston.com/xsd&quot; xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;
        ///  &lt;soapenv:Header /&gt;
        ///  &lt;soapenv:Body&gt;
        ///    &lt;xsd:SearchHotelsRequest&gt;
        ///      &lt;xsd:locale&gt;en&lt;/xsd:locale&gt;
        ///      &lt;xsd:loginDetails xsd1:email=&quot;booking@imperatours.com&quot; xsd1:password=&quot;ImperaTours2017&quot; /&gt;
        ///      &lt;xs [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;soapenv:Envelope xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;
        ///  &lt;soapenv:Body&gt;
        ///    &lt;xsd:SearchHotelsResponse xmlns:xsd=&quot;http://request.ws.hotelston.com/xsd&quot;&gt;
        ///      &lt;xsd:success xsd1:trackingId=&quot;4241477944478376:2101347183&quot; xmlns:xsd1=&quot;http://types.ws.hotelston.com/xsd&quot;&gt;true&lt;/xsd:success&gt;
        ///      &lt;xsd:searchId&gt;3770913640&lt;/xsd:searchId&gt;
        ///      &lt;xsd:hotel xsd:id=&quot;52700741&quot; xsd:name=&quot;Micon Lofts&quot; xsd:lastUpdated=&quot;2021-06-19T03:38:42.586+03:00&quot;&gt;
        ///        &lt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;52700741&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC=&quot;20744729856|3089463496&quot; RT=&quot;Loft Suite&quot; MBC=&quot;2359299&quot; AMT=&quot;299.52&quot; TPR=&quot;52700741|3770913640|2359299&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs /&gt;&lt;/Result&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;52700741&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC=&quot;20744729854|3089463496&quot; RT=&quot;Loft Suite&quot; MBC=&quot;2359299&quot; AMT=&quot;332.80&quot; TPR=&quot;52700741|3770913640|2359299&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;fals [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}

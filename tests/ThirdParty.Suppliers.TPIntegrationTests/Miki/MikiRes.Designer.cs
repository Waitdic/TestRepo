﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThirdParty.Suppliers.TPIntegrationTests.Miki {
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
    internal class MikiRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MikiRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThirdParty.Suppliers.TPIntegrationTests.Miki.MikiRes", typeof(MikiRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://xml-uat.bookingengine.es/webservice
        ///
        ///**HEADERS**
        ///
        ///Accept-Encoding: gzip
        ///Content-Type: application/soap+xml;charset=UTF-8;action=&quot;hotelSearch&quot;
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soap:Envelope xmlns:soap=&quot;http://www.w3.org/2003/05/soap-envelope&quot;&gt;
        ///  &lt;soap:Header /&gt;
        ///  &lt;soap:Body&gt;
        ///    &lt;hotelSearchRequest versionNumber=&quot;7.0&quot;&gt;
        ///      &lt;requestAuditInfo&gt;
        ///        &lt;agentCode&gt;XXX&lt;/agentCode&gt;
        ///        &lt;requestPassword /&gt;
        ///        &lt;requestID&gt;&lt;/requestID&gt;
        ///        &lt;requestDateTime&gt;&lt;/requestDateTime&gt;
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;soap:Envelope xmlns:soap=&quot;http://www.w3.org/2003/05/soap-envelope&quot;&gt;
        /// &lt;soap:Header/&gt;
        /// &lt;soap:Body&gt;
        ///  &lt;hotelSearchResponse versionNumber=&quot;7.0&quot;&gt;
        ///   &lt;responseAuditInfo&gt;
        ///    &lt;agentCode&gt;XXX&lt;/agentCode&gt;
        ///    &lt;responseID&gt;505589839&lt;/responseID&gt;
        ///    &lt;responseDateTime&gt;2016-06-15T11:26:33.850+01:00&lt;/responseDateTime&gt;
        ///    &lt;transactionID&gt;8f3ed181-68aa-4a45-9f4d-7ba7a6db54c3&lt;/transactionID&gt;
        ///    &lt;!--Audit ID : 8f3ed181-68aa-4a45-9f4d-7ba7a6db54c3--&gt;
        ///    &lt;!--WS : MSI--&gt;
        ///   &lt;/responseAuditInfo&gt;
        ///   &lt;availabilitySum [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;CAH984700&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RT=&quot;TSU With Cot&quot; MBC=&quot;RO&quot; AMT=&quot;992.25&quot; TPR=&quot;00004~XXX~6ht6hbdpTeNBpElHnshsLHOe-nNJqSzEXt6HpSJ-Qkek5HUb9rzQ5kPnji0ICQhv9z91iyUInJqK299JyHHxlck3BM4JavAJJ68vpud4127Z57DHLUoPcWGCdFR5-Mt7vbynFqOpPFkhO01-X1ADZ1vRWPMNKg0hYxNGnn4WGZaWra8cIWo_9vXz1BlF7mff1hWMa_IX0POn2umxnw3-nhWFCYEDnZv-gTWRjqRJb0jZnc1_IHNsro2bGoA5-j2FskYKqwlYp3ClvguXbnY3XQRxt1GXzcSmeKl6JsgkNMM&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}
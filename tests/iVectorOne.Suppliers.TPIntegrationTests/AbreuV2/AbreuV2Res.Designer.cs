﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Suppliers.TPIntegrationTests.AbreuV2 {
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
    internal class AbreuV2Res {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AbreuV2Res() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThirdParty.Suppliers.TPIntegrationTests.AbreuV2.AbreuV2Res", typeof(AbreuV2Res).Assembly);
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
        ///   Looks up a localized string similar to URL: http://site.tst.abreuonline.com/NewAvailabilityServlet/hotelavail/OTA2014Compact
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soap-env:Envelope xmlns:soap-env=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot;&gt;
        ///  &lt;soap-env:Header&gt;
        ///    &lt;wsse:Security xmlns:wsse=&quot;http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd&quot;&gt;
        ///      &lt;wsse:Username&gt;17468&lt;/wsse:Username&gt;
        ///      &lt;wsse:Password&gt;bookabedxml&lt;/wsse:Password&gt;
        ///      &lt;Context&gt;abreu_tr&lt;/Context&gt;
        ///    &lt;/wsse:Security&gt;
        ///  &lt;/soap- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLogs {
            get {
                return ResourceManager.GetString("RequestLogs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;OTA_HotelAvailRS xmlns=&quot;http://parsec.es/hotelapi/OTA2014Compact&quot; TimeStamp=&quot;2015-03-27T12:21:57Z&quot; PrimaryLangID=&quot;en-GB&quot; Id=&quot;101,483699182,8840&quot;&gt;
        ///    &lt;Hotels HotelCount=&quot;3&quot;&gt;
        ///        &lt;DateRange Start=&quot;2015-09-25&quot; End=&quot;2015-09-29&quot; /&gt;
        ///        &lt;RoomCandidates&gt;
        ///            &lt;RoomCandidate RPH=&quot;1&quot;&gt;
        ///                &lt;Guests&gt;
        ///                    &lt;Guest AgeCode=&quot;A&quot; Count=&quot;2&quot; /&gt;
        ///                &lt;/Guests&gt;
        ///            &lt;/RoomCandidate&gt;
        ///        &lt;/RoomCandidates&gt;
        ///        &lt;H [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;92860&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC=&quot;177783&quot; RT=&quot;Standard room&quot; MBC=&quot;BB&quot; AMT=&quot;186.00&quot; TPR=&quot;UKXR7U0mkHIdzKvU1aAVRCM1XRd2NFVEUXUZYsSgf3ncSnQaeCLPvcNiCS/q60hwey9NRmo20tRe4k9nkkEEwQ==|&amp;lt;CancelPenalties NonRefundable=&amp;quot;&amp;quot; /&amp;gt;&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs /&gt;&lt;/Result&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;2242&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RTC=&quot;4655&quot; RT=&quot;Standard room&quot; MBC=&quot;BB&quot; AMT=&quot;246.00&quot; TPR=&quot;A6daSajEfa0j8BzeRR [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}
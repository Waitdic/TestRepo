﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.W2M {
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
    internal class W2MRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal W2MRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.W2M.W2MRes", typeof(W2MRes).Assembly);
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
        ///   Looks up a localized string similar to URL: https://xml-uat.bookingengine.es/WebService/jp/operations/availtransactions.asmx
        ///
        ///**HEADERS**
        ///
        ///Timeout: 100
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;soapenv:Envelope xmlns:soapenv=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot; xmlns=&quot;http://www.juniper.es/webservice/2007/&quot;&gt;
        ///  &lt;soapenv:Header /&gt;
        ///  &lt;soapenv:Body&gt;
        ///    &lt;HotelAvail&gt;
        ///      &lt;HotelAvailRQ Version=&quot;1.1&quot; Language=&quot;en&quot;&gt;
        ///        &lt;Login Email=&quot;XML_IntuitiveSystems-iVector&quot; Password=&quot;H&apos;LGw.m(6CVzdj{F&quot;&gt;
        ///        &lt;/Login&gt;
        ///        &lt;Paxes&gt;
        ///          &lt;Pax IdPax=&quot;1&quot;&gt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;soap:Envelope xmlns:soap=&quot;http://schemas.xmlsoap.org/soap/envelope/&quot; xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot;&gt;&lt;soap:Body&gt;&lt;HotelAvailResponse xmlns=&quot;http://www.juniper.es/webservice/2007/&quot;&gt;&lt;AvailabilityRS Url=&quot;http://xml-uat.bookingengine.es&quot; TimeStamp=&quot;2022-01-20T17:34:11.2245592+01:00&quot; IntCode=&quot;d+TBVHEOCsK0LWuPc+yvnPc+0cqZtYW171/JIC3h9Fk=&quot;&gt;&lt;Results&gt;&lt;HotelResult Code=&quot;JP046300&quot; JPCode=&quot;JP046300&quot; JPDCode=&quot;JPD0868 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ResponseXML {
            get {
                return ResourceManager.GetString("ResponseXML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;JP046300&quot; CC=&quot;EUR&quot; RTC=&quot;4-Standard&quot; RT=&quot;Single&quot; MBC=&quot;SA&quot; AMT=&quot;168.43&quot; TPR=&quot;ya79dM4dS6R6EywV4XhfEuAjcad0DPbFEmp6L8qW4BRaK/oXqxieLH1jauXRPShdAgqfTQ6DZM3HmE4jnSF6gY/eD5FDqvrQBqjInEKoeTdDoHj16Fxtonybpt3X8YT263ngMhrwpuX4s1C8c8sURZOW2qc8TqFr2NGWNDfgTkDnDMjeRTpxzGhWznhi4M3en4QoRnMeFMPhsNvwGQYUuDtZEVIOFKB2LFa2aSwn/x3Egb7i/5uILrbWET5HYIFF0OJ7snOPcg6Enb1Kglci3SSjyXUeGltVGxGddir5QB2ynyXNjjqJGXJyXUfDa4RfgO5zMPeDcPoBEExUZtYFsw==&quot; DSC=&quot;0&quot; SO=&quot;Promotional Notes&quot; AR=&quot;800&quot; COM=&quot;0&quot; DP=&quot;false&quot; NP [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResultXML {
            get {
                return ResourceManager.GetString("TransformedResultXML", resourceCulture);
            }
        }
    }
}
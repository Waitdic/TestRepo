﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.RMI {
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
    internal class RMIRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RMIRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.RMI.RMIRes", typeof(RMIRes).Assembly);
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
        ///   Looks up a localized string similar to URL: https://xmldev-xml.centriumres.com/
        ///
        ///**HEADERS**
        ///
        ///Timeout: 100
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;&lt;SearchRequest xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot;&gt;&lt;LoginDetails&gt;&lt;Login&gt;ClassicCollection&lt;/Login&gt;&lt;Password&gt;**********&lt;/Password&gt;&lt;Version&gt;6.0&lt;/Version&gt;&lt;/LoginDetails&gt;&lt;SearchDetails&gt;&lt;ArrivalDate&gt;2021-09-01&lt;/ArrivalDate&gt;&lt;Duration&gt;5&lt;/Duration&gt;&lt;PropertyID&gt;102245&lt;/PropertyID&gt;&lt;MealBasisID&gt;0&lt;/MealBasisID&gt;&lt;MinStarRating&gt;0&lt;/Min [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;SearchResponse xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot;
        ///    xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
        ///    &lt;RequestInfo&gt;
        ///        &lt;Timestamp&gt;1650658518&lt;/Timestamp&gt;
        ///        &lt;TimestampISO&gt;2022-04-22T20:15:18+00:00&lt;/TimestampISO&gt;
        ///        &lt;Host&gt;xmldev-xml.centriumres.com&lt;/Host&gt;
        ///        &lt;HostIP&gt;172.31.11.218&lt;/HostIP&gt;
        ///        &lt;ReqID&gt;62630cd63e3182.08315929&lt;/ReqID&gt;
        ///    &lt;/RequestInfo&gt;
        ///    &lt;ReturnStatus&gt;
        ///        &lt;Success&gt;True&lt;/Success&gt;
        ///        &lt;Exception/&gt;
        ///    &lt;/ReturnStatus&gt;
        ///    &lt;Proper [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;17&quot; TPK=&quot;17&quot; CC=&quot;USD&quot; PRBID=&quot;1&quot; RTC=&quot;436430006&quot; RT=&quot;Palm View Garden View&quot; MBC=&quot;1&quot; AMT=&quot;5986&quot; TPR=&quot;436430006&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs /&gt;&lt;/Result&gt;&lt;Result MID=&quot;17&quot; TPK=&quot;17&quot; CC=&quot;USD&quot; PRBID=&quot;1&quot; RTC=&quot;436440006&quot; RT=&quot;Beach Front Beachfront&quot; MBC=&quot;1&quot; AMT=&quot;6742&quot; TPR=&quot;436440006&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs / [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVectorOne.Tests.OceanBeds {
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
    internal class OceanBedsRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal OceanBedsRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("iVectorOne.Tests.OceanBeds.OceanBedsRes", typeof(OceanBedsRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://oceanbeds.com/service.svc/GetPropertyAvailability
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;PropertyAvailabilityRQ xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns=&quot;http://oceanbeds.com/2014/10&quot;&gt;
        ///  &lt;Credential&gt;
        ///    &lt;User&gt;xmluser&lt;/User&gt;
        ///    &lt;Password&gt;test&lt;/Password&gt;
        ///  &lt;/Credential&gt;
        ///  &lt;CheckInDate&gt;2021-Sep-01&lt;/CheckInDate&gt;
        ///  &lt;CheckOutDate&gt;2021-Sep-06&lt;/CheckOutDate&gt;
        ///  &lt;RoomList&gt;
        ///    &lt;RequestRoom&gt;
        ///      &lt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;PropertyAvailabilityRS xmlns=&quot;http://oceanbeds.com/2014/10&quot; xmlns:i=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
        ///  &lt;Request&gt;
        ///    &lt;Credential&gt;
        ///      &lt;User&gt;Trevor&lt;/User&gt;
        ///      &lt;Password&gt;******&lt;/Password&gt;
        ///    &lt;/Credential&gt;
        ///    &lt;CheckInDate&gt;02-nov-2014&lt;/CheckInDate&gt;
        ///    &lt;CheckOutDate&gt;20-nov-2014&lt;/CheckOutDate&gt;
        ///    &lt;RoomList&gt;
        ///      &lt;RequestRoom&gt;
        ///        &lt;Adults&gt;1&lt;/Adults&gt;
        ///        &lt;Children&gt;1&lt;/Children&gt;
        ///      &lt;/RequestRoom&gt;
        ///      &lt;RequestRoom&gt;
        ///        &lt;Adults&gt;1&lt;/Adults&gt;
        ///        &lt;Children&gt;1&lt;/Childr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;h27&quot; CC=&quot;USD&quot; PRBID=&quot;1&quot; RTC=&quot;35|1BED&quot; RT=&quot;1 Bedroom Condo Deluxe Queen or King&quot; MBC=&quot;&quot; AMT=&quot;822&quot; TPR=&quot;h27||35|1BED&quot; DSC=&quot;0&quot; SO=&quot;Stay 7 Nights pay for 6&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs /&gt;&lt;/Result&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;h24&quot; CC=&quot;USD&quot; PRBID=&quot;1&quot; RTC=&quot;30|1BCDO&quot; RT=&quot;1 Bed Condo&quot; MBC=&quot;&quot; AMT=&quot;965&quot; TPR=&quot;h24||30|1BCDO&quot; DSC=&quot;0&quot; SO=&quot;Stay 7 Nights pay for 6&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot;  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}

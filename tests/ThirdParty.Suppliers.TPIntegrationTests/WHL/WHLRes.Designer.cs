﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThirdParty.Tests.WHL {
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
    internal class WHLRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WHLRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThirdParty.Tests.WHL.WHLRes", typeof(WHLRes).Assembly);
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
        ///   Looks up a localized string similar to URL: http://towers.netstorming.net/kalima/call.php
        ///
        ///**HEADERS**
        ///
        ///Timeout: 1
        ///
        ///
        ///**REQUEST**
        ///
        ///&lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;envelope&gt;
        ///  &lt;header&gt;
        ///    &lt;actor&gt;Imperatours&lt;/actor&gt;
        ///    &lt;user&gt;xmluser&lt;/user&gt;
        ///    &lt;password&gt;imperatxml&lt;/password&gt;
        ///    &lt;version&gt;1.6.0&lt;/version&gt;
        ///    &lt;timestamp&gt;
        ///    &lt;/timestamp&gt;
        ///  &lt;/header&gt;
        ///  &lt;query type=&quot;availability&quot; product=&quot;hotel&quot;&gt;
        ///    &lt;city code=&quot;MAD_12&quot; /&gt;
        ///    &lt;filters&gt;
        ///      &lt;filter&gt;AVAILONLY&lt;/filter&gt;
        ///    &lt;/filters&gt;
        ///    &lt;checkin date=&quot;2021-09-01&quot; /&gt;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RequestLog {
            get {
                return ResourceManager.GetString("RequestLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;envelope&gt;
        ///    &lt;header&gt;
        ///        &lt;version&gt;1.6.3&lt;/version&gt;
        ///        &lt;timestamp&gt;20200129154400&lt;/timestamp&gt;
        ///    &lt;/header&gt;
        ///    &lt;response type=&quot;availability&quot; product=&quot;hotel&quot;&gt;
        ///        &lt;search number=&quot;100045951727573&quot; time=&quot;2.51&quot; /&gt;
        ///        &lt;nights number=&quot;18&quot; /&gt;
        ///        &lt;checkin date=&quot;2020-09-05&quot; /&gt;
        ///        &lt;checkout date=&quot;2020-09-23&quot; /&gt;
        ///        &lt;hotels total=&quot;2&quot;&gt;
        ///            &lt;hotel code=&quot;43016&quot; name=&quot;UNIVERSO&quot; stars=&quot;3&quot; location=&quot;01&quot; address=&quot;Piazza Santa Maria No [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Response {
            get {
                return ResourceManager.GetString("Response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Results&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;43016&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RT=&quot;Single STANDARD ROOM&quot; MBC=&quot;RB&quot; AMT=&quot;4248.0&quot; TPR=&quot;sgl_false_0_false_43016_4248_FLR_UNIVERSO_0xbd.43016.7.RB.C.Y2JhYTkw.0.BB.MGFjMDRk_RB_C_7_true_false_100045951727573&quot; DSC=&quot;0&quot; COM=&quot;0&quot; DP=&quot;false&quot; NPA=&quot;0&quot; NRF=&quot;false&quot; FIXPR=&quot;false&quot; FREEC=&quot;false&quot; RQ=&quot;false&quot; PLA=&quot;false&quot; PLR=&quot;false&quot; MSP=&quot;0&quot;&gt;&lt;Cs&gt;&lt;C SD=&quot;&quot; ED=&quot;&quot; AMT=&quot;236.1888&quot; /&gt;&lt;/Cs&gt;&lt;/Result&gt;&lt;Result MID=&quot;0&quot; TPK=&quot;103945&quot; CC=&quot;EUR&quot; PRBID=&quot;1&quot; RT=&quot;Twin for sole use STANDARD ROOM&quot; MBC=&quot;RO&quot; AMT=&quot;4734.0&quot; TPR=&quot;tsu_f [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransformedResponse {
            get {
                return ResourceManager.GetString("TransformedResponse", resourceCulture);
            }
        }
    }
}

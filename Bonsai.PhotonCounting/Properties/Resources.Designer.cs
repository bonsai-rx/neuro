﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bonsai.PhotonCounting.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bonsai.PhotonCounting.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Unable to close the connection to the C8855 device..
        /// </summary>
        internal static string CloseException {
            get {
                return ResourceManager.GetString("CloseException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to start counting with the C8855 device..
        /// </summary>
        internal static string CountStartException {
            get {
                return ResourceManager.GetString("CountStartException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to stop counting with the C8855 device..
        /// </summary>
        internal static string CountStopException {
            get {
                return ResourceManager.GetString("CountStopException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an error in the block transfer of data from the C8855 device..
        /// </summary>
        internal static string ErrorTransferException {
            get {
                return ResourceManager.GetString("ErrorTransferException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to open a connection to the C8855 device. Make sure the device driver has been successfully installed..
        /// </summary>
        internal static string InvalidHandleException {
            get {
                return ResourceManager.GetString("InvalidHandleException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to read data from the C8855 device..
        /// </summary>
        internal static string ReadDataException {
            get {
                return ResourceManager.GetString("ReadDataException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to reset the C8855 device..
        /// </summary>
        internal static string ResetException {
            get {
                return ResourceManager.GetString("ResetException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to set the power state on the C8855 device..
        /// </summary>
        internal static string SetPmtPowerException {
            get {
                return ResourceManager.GetString("SetPmtPowerException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to configure the C8855 device. Make sure all the parameters are valid..
        /// </summary>
        internal static string SetupException {
            get {
                return ResourceManager.GetString("SetupException", resourceCulture);
            }
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xps2Img.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::CommandLine.Localization.SingleAssemblyResourceManager(typeof(Strings));
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
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string Error_Header {
            get {
                return ResourceManager.GetString("Error_Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pages specified are out of document pages range. {0} is the last one..
        /// </summary>
        internal static string Error_PagesRange {
            get {
                return ResourceManager.GetString("Error_PagesRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make sure you have sufficient drive access rights and enough storage space..
        /// </summary>
        internal static string Message_DiskStorage {
            get {
                return ResourceManager.GetString("Message_DiskStorage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make sure file is accessible and not read-only..
        /// </summary>
        internal static string Message_FileAccess {
            get {
                return ResourceManager.GetString("Message_FileAccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please reduce one or some of the following: Processors / Image DPI / Image Size..
        /// </summary>
        internal static string Message_Memory {
            get {
                return ResourceManager.GetString("Message_Memory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [{{4,3}}%] {{5}}/{{6}} Deleting image {0} ({1}/{{2}}) at &apos;{{3}}&apos;....
        /// </summary>
        internal static string Template_CleanProgress {
            get {
                return ResourceManager.GetString("Template_CleanProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleted {0} file(s).
        /// </summary>
        internal static string Template_Cleared {
            get {
                return ResourceManager.GetString("Template_Cleared", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Converted {{0}} page(s) in {{1}} ({3}{4}/{5}).
        /// </summary>
        internal static string Template_Done {
            get {
                return ResourceManager.GetString("Template_Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [{{4,3}}%] {{5}}/{{6}} Converting page {0} ({1}/{{2}}) into &apos;{{3}}&apos;....
        /// </summary>
        internal static string Template_Progress {
            get {
                return ResourceManager.GetString("Template_Progress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}% {5}/{6} ({1}/{2}) [{3}] - {4} - xps2img.
        /// </summary>
        internal static string Template_ProgressTitle {
            get {
                return ResourceManager.GetString("Template_ProgressTitle", resourceCulture);
            }
        }
    }
}

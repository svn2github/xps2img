//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xps2Img.Shared.Resources {
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
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auto.
        /// </summary>
        public static string Auto {
            get {
                return ResourceManager.GetString("Auto", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All Files (*.*)|*.*.
        /// </summary>
        public static string FilterAllFiles {
            get {
                return ResourceManager.GetString("FilterAllFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executable Files (*.exe)|*.exe|.
        /// </summary>
        public static string FilterExeFiles {
            get {
                return ResourceManager.GetString("FilterExeFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XPS Files (*.xps)|*.xps|.
        /// </summary>
        public static string FilterXPSFiles {
            get {
                return ResourceManager.GetString("FilterXPSFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BMP.
        /// </summary>
        public static string ImageType_BmpValue {
            get {
                return ResourceManager.GetString("ImageType_BmpValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GIF.
        /// </summary>
        public static string ImageType_GifValue {
            get {
                return ResourceManager.GetString("ImageType_GifValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to JPEG.
        /// </summary>
        public static string ImageType_JpegValue {
            get {
                return ResourceManager.GetString("ImageType_JpegValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PNG.
        /// </summary>
        public static string ImageType_PngValue {
            get {
                return ResourceManager.GetString("ImageType_PngValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TIFF.
        /// </summary>
        public static string ImageType_TiffValue {
            get {
                return ResourceManager.GetString("ImageType_TiffValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No.
        /// </summary>
        public static string No {
            get {
                return ResourceManager.GetString("No", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Clean (delete images).
        /// </summary>
        public static string Options_CleanDescription {
            get {
                return ResourceManager.GetString("Options_CleanDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CPUs process will be executed on
        ///  all by default
        ///Syntax:
        ///  all:		0-
        ///  single:	0
        ///  set:		0,2
        ///  range:	0-2 or -2 or 2-
        ///  combined:	0,2-.
        /// </summary>
        public static string Options_CpuAffinityCmdDescription {
            get {
                return ResourceManager.GetString("Options_CpuAffinityCmdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CPUs processors will be executed on
        ///  all by default
        ///Syntax:
        ///  all:		0-
        ///  single:	0
        ///  set:		0,2
        ///  range:	0-2 or -2 or 2-
        ///  combined:	0,2-.
        /// </summary>
        public static string Options_CpuAffinityDescription {
            get {
                return ResourceManager.GetString("Options_CpuAffinityDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processors Affinity.
        /// </summary>
        public static string Options_CpuAffinityName {
            get {
                return ResourceManager.GetString("Options_CpuAffinityName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Converts XPS (XML Paper Specification) document to set of images..
        /// </summary>
        public static string Options_Description {
            get {
                return ResourceManager.GetString("Options_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image DPI (16-2350)
        ///  DPI will be ignored if image size is specified.
        /// </summary>
        public static string Options_DpiDescription {
            get {
                return ResourceManager.GetString("Options_DpiDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image DPI.
        /// </summary>
        public static string Options_DpiName {
            get {
                return ResourceManager.GetString("Options_DpiName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image type.
        /// </summary>
        public static string Options_FileTypeDescription {
            get {
                return ResourceManager.GetString("Options_FileTypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image Type.
        /// </summary>
        public static string Options_FileTypeName {
            get {
                return ResourceManager.GetString("Options_FileTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Document body first page index or desired and actual page numbers in any order
        ///  1 by default
        ///Syntax:
        ///  numeric: 5
        ///  desired and actual page numbers: &quot;96 100&quot; or 96-100.
        /// </summary>
        public static string Options_FirstPageIndexDescription {
            get {
                return ResourceManager.GetString("Options_FirstPageIndexDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to First Page Index.
        /// </summary>
        public static string Options_FirstPageIndexName {
            get {
                return ResourceManager.GetString("Options_FirstPageIndexName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ignore conversion errors.
        /// </summary>
        public static string Options_IgnoreErrorsDescription {
            get {
                return ResourceManager.GetString("Options_IgnoreErrorsDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ignore Conversion Errors.
        /// </summary>
        public static string Options_IgnoreErrorsName {
            get {
                return ResourceManager.GetString("Options_IgnoreErrorsName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not overwrite existing images.
        /// </summary>
        public static string Options_IgnoreExistingCmdDescription {
            get {
                return ResourceManager.GetString("Options_IgnoreExistingCmdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not overwrite existing images. Will be reset in case of any image related property change.
        /// </summary>
        public static string Options_IgnoreExistingDescription {
            get {
                return ResourceManager.GetString("Options_IgnoreExistingDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ignore Existing Images.
        /// </summary>
        public static string Options_IgnoreExistingName {
            get {
                return ResourceManager.GetString("Options_IgnoreExistingName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image name prefix. &lt;&gt;:&quot;/\|?* characters are not allowed
        ///  numeric if omitted: 01.png
        ///  name of src file if empty (-i &quot;&quot;): src_file-01.png.
        /// </summary>
        public static string Options_ImageNameCmdDescription {
            get {
                return ResourceManager.GetString("Options_ImageNameCmdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image name prefix. &lt;&gt;:&quot;/\|?* characters are not allowed
        ///  numeric if omitted: 01.png.
        /// </summary>
        public static string Options_ImageNameDescription {
            get {
                return ResourceManager.GetString("Options_ImageNameDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image Prefix.
        /// </summary>
        public static string Options_ImageNameName {
            get {
                return ResourceManager.GetString("Options_ImageNameName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to JPEG quality level (10-100).
        /// </summary>
        public static string Options_JpegQualityDescription {
            get {
                return ResourceManager.GetString("Options_JpegQualityDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to JPEG Quality.
        /// </summary>
        public static string Options_JpegQualityName {
            get {
                return ResourceManager.GetString("Options_JpegQualityName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Options.
        /// </summary>
        public static string Options_OptionsCategory {
            get {
                return ResourceManager.GetString("Options_OptionsCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output folder
        ///  new folder named as document will be created in folder where document is by default.
        /// </summary>
        public static string Options_OutDirDescription {
            get {
                return ResourceManager.GetString("Options_OutDirDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output Folder.
        /// </summary>
        public static string Options_OutDirName {
            get {
                return ResourceManager.GetString("Options_OutDirName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Page number(s)
        ///  all pages by default
        ///Syntax:
        ///  all:		1-
        ///  single:	1
        ///  set:		1,3
        ///  range:	1-10 or -10 or 10-
        ///  combined:	1,3-5,7-9,15-.
        /// </summary>
        public static string Options_PagesDescription {
            get {
                return ResourceManager.GetString("Options_PagesDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Page Number(s).
        /// </summary>
        public static string Options_PagesName {
            get {
                return ResourceManager.GetString("Options_PagesName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 	Parameters.
        /// </summary>
        public static string Options_ParametersCategory {
            get {
                return ResourceManager.GetString("Options_ParametersCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Action to execute after conversion completed.
        /// </summary>
        public static string Options_PostActionDescription {
            get {
                return ResourceManager.GetString("Options_PostActionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to After Conversion.
        /// </summary>
        public static string Options_PostActionName {
            get {
                return ResourceManager.GetString("Options_PostActionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preliminaries prefix. &lt;&gt;:&quot;/\|?* characters are not allowed.
        /// </summary>
        public static string Options_PrelimsPrefixDescription {
            get {
                return ResourceManager.GetString("Options_PrelimsPrefixDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preliminaries Prefix.
        /// </summary>
        public static string Options_PrelimsPrefixName {
            get {
                return ResourceManager.GetString("Options_PrelimsPrefixName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of simultaneously running document processors
        ///  number of logical CPUs by default.
        /// </summary>
        public static string Options_ProcessorsNumberDescription {
            get {
                return ResourceManager.GetString("Options_ProcessorsNumberDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processors.
        /// </summary>
        public static string Options_ProcessorsNumberName {
            get {
                return ResourceManager.GetString("Options_ProcessorsNumberName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Process priority
        ///  normal by default.
        /// </summary>
        public static string Options_ProcessPriorityCmdDescription {
            get {
                return ResourceManager.GetString("Options_ProcessPriorityCmdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Document processors priority
        ///  Normal by default.
        /// </summary>
        public static string Options_ProcessPriorityDescription {
            get {
                return ResourceManager.GetString("Options_ProcessPriorityDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processors Priority.
        /// </summary>
        public static string Options_ProcessPriorityName {
            get {
                return ResourceManager.GetString("Options_ProcessPriorityName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Desired image size (greater or equal 10)
        ///  DPI will be ignored if image size is specified
        ///Syntax:
        ///  width only:	2000
        ///  height only:	x1000
        ///  both:		2000x1000
        ///		width for landscape orientation
        ///		height for portrait orientation.
        /// </summary>
        public static string Options_RequiredSizeDescription {
            get {
                return ResourceManager.GetString("Options_RequiredSizeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image Size.
        /// </summary>
        public static string Options_RequiredSizeName {
            get {
                return ResourceManager.GetString("Options_RequiredSizeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shorten image extension down to three characters.
        /// </summary>
        public static string Options_ShortenExtensionDescription {
            get {
                return ResourceManager.GetString("Options_ShortenExtensionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Silent mode (no progress will be shown).
        /// </summary>
        public static string Options_SilentDescription {
            get {
                return ResourceManager.GetString("Options_SilentDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Silent.
        /// </summary>
        public static string Options_SilentName {
            get {
                return ResourceManager.GetString("Options_SilentName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XPS file to process.
        /// </summary>
        public static string Options_SrcFileCmdDescription {
            get {
                return ResourceManager.GetString("Options_SrcFileCmdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XPS file to process (required).
        /// </summary>
        public static string Options_SrcFileDescription {
            get {
                return ResourceManager.GetString("Options_SrcFileDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XPS File.
        /// </summary>
        public static string Options_SrcFileName {
            get {
                return ResourceManager.GetString("Options_SrcFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test mode (no files will be written).
        /// </summary>
        public static string Options_TestDescription {
            get {
                return ResourceManager.GetString("Options_TestDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test Mode.
        /// </summary>
        public static string Options_TestName {
            get {
                return ResourceManager.GetString("Options_TestName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TIFF compression method
        ///  CCITT3, CCITT4 and RLE produce black and white images.
        /// </summary>
        public static string Options_TiffCompressionDescription {
            get {
                return ResourceManager.GetString("Options_TiffCompressionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TIFF Compression.
        /// </summary>
        public static string Options_TiffCompressionName {
            get {
                return ResourceManager.GetString("Options_TiffCompressionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use source file name as image prefix: src_file-01.png.
        /// </summary>
        public static string Options_UseFileNameAsImageNameDescription {
            get {
                return ResourceManager.GetString("Options_UseFileNameAsImageNameDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File Name As Image Prefix.
        /// </summary>
        public static string Options_UseFileNameAsImageNameName {
            get {
                return ResourceManager.GetString("Options_UseFileNameAsImageNameName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do Nothing.
        /// </summary>
        public static string PostAction_DoNothingValue {
            get {
                return ResourceManager.GetString("PostAction_DoNothingValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exit.
        /// </summary>
        public static string PostAction_ExitValue {
            get {
                return ResourceManager.GetString("PostAction_ExitValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hibernate.
        /// </summary>
        public static string PostAction_HibernateValue {
            get {
                return ResourceManager.GetString("PostAction_HibernateValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Log Off.
        /// </summary>
        public static string PostAction_LogOffValue {
            get {
                return ResourceManager.GetString("PostAction_LogOffValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reboot.
        /// </summary>
        public static string PostAction_RebootValue {
            get {
                return ResourceManager.GetString("PostAction_RebootValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shutdown.
        /// </summary>
        public static string PostAction_ShutdownValue {
            get {
                return ResourceManager.GetString("PostAction_ShutdownValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sleep.
        /// </summary>
        public static string PostAction_SleepValue {
            get {
                return ResourceManager.GetString("PostAction_SleepValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Above Normal.
        /// </summary>
        public static string ProcessPriorityClass_AboveNormalValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_AboveNormalValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auto.
        /// </summary>
        public static string ProcessPriorityClass_AutoValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_AutoValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Below Normal.
        /// </summary>
        public static string ProcessPriorityClass_BelowNormalValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_BelowNormalValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to High.
        /// </summary>
        public static string ProcessPriorityClass_HighValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_HighValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Idle.
        /// </summary>
        public static string ProcessPriorityClass_IdleValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_IdleValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Normal.
        /// </summary>
        public static string ProcessPriorityClass_NormalValue {
            get {
                return ResourceManager.GetString("ProcessPriorityClass_NormalValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select Executable File.
        /// </summary>
        public static string SelectExecutableFile {
            get {
                return ResourceManager.GetString("SelectExecutableFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select XPS File.
        /// </summary>
        public static string SelectXpsFile {
            get {
                return ResourceManager.GetString("SelectXpsFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify folder to store images. If not set new folder named as document will be created in folder where document is..
        /// </summary>
        public static string SpecifyImagesFolder {
            get {
                return ResourceManager.GetString("SpecifyImagesFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CCITT3.
        /// </summary>
        public static string TiffCompressOption_Ccitt3Value {
            get {
                return ResourceManager.GetString("TiffCompressOption_Ccitt3Value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CCITT4.
        /// </summary>
        public static string TiffCompressOption_Ccitt4Value {
            get {
                return ResourceManager.GetString("TiffCompressOption_Ccitt4Value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LZW.
        /// </summary>
        public static string TiffCompressOption_LzwValue {
            get {
                return ResourceManager.GetString("TiffCompressOption_LzwValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to None.
        /// </summary>
        public static string TiffCompressOption_NoneValue {
            get {
                return ResourceManager.GetString("TiffCompressOption_NoneValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RLE.
        /// </summary>
        public static string TiffCompressOption_RleValue {
            get {
                return ResourceManager.GetString("TiffCompressOption_RleValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ZIP.
        /// </summary>
        public static string TiffCompressOption_ZipValue {
            get {
                return ResourceManager.GetString("TiffCompressOption_ZipValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value is not valid integer number.
        /// </summary>
        public static string ValueIsNotValidIntegerNumber {
            get {
                return ResourceManager.GetString("ValueIsNotValidIntegerNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Yes.
        /// </summary>
        public static string Yes {
            get {
                return ResourceManager.GetString("Yes", resourceCulture);
            }
        }
    }
}

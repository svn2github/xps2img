using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("XPS to Images Converter"
#if XPS2IMG_UI
    + " Frontend"
#elif XPS2IMG_SHARED
    + " Shared Code"
#endif
)]
[assembly: AssemblyDescription("XPS to Images Converter"
#if XPS2IMG_UI
    + " Frontend"
#elif XPS2IMG_SHARED
    + " Shared Code"
#endif
)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://xps2img.sf.net")]
[assembly: AssemblyProduct("Xps2Img")]
[assembly: AssemblyCopyright("Copyright © 2010-2013, Ivan Ivon")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly : ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly : Guid("a9a56726-7a2e-4e55-ae35-4e87c8673067")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly : AssemblyVersion    ("3.25.0.0")]
[assembly : AssemblyFileVersion("3.25.0.0")]

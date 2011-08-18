using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Xps2ImgUI.Utils
{
    public static class AssemblyInfo
    {
        public static string Title
        {
            get { return GetCustomAttribute<AssemblyTitleAttribute>().Title; } 
        }

        public static string Description
        {
            get { return GetCustomAttribute<AssemblyDescriptionAttribute>().Description; } 
        }

        public static string Company
        {
            get { return GetCustomAttribute<AssemblyCompanyAttribute>().Company; } 
        }

        public static string Product
        {
            get { return GetCustomAttribute<AssemblyProductAttribute>().Product; }
        }

        public static string Copyright
        {
            get { return GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright; }
        }

        public static string Trademark
        {
            get { return GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark; }
        }

        public static string Guid
        {
            get { return GetCustomAttribute<GuidAttribute>().Value; }
        }

        public static string AssemblyVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public static string FileVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static string OriginalFilename
        {
            get
            {
                return VersionInfo.OriginalFilename;
            }
        }

        public static string FileName
        {
            get
            {
                return VersionInfo.FileName;
            }
        }

        private static T GetCustomAttribute<T>() where T : Attribute
        {
            var customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            return (T)(customAttributes.Length > 0 ? customAttributes[0] : Activator.CreateInstance(typeof(T), String.Empty));
        }

        private static FileVersionInfo VersionInfo
        {
            get
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}

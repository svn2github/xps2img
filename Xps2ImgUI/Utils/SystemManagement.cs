using System.Management;

namespace Xps2ImgUI.Utils
{
    public enum ShutdownType
    {
        LogOff          = 0,
        Shutdown        = 1,
        Reboot          = 2,
        PowerOff        = 8,
        ForcedLogOff    = 4,
        ForcedShutdown  = 5,
        ForcedReboot    = 6,
        ForcedPowerOff  = 12
    };

    public static class SystemManagement
    {
        public static void Shutdown(ShutdownType shutdownType)
        {
            using (var operatingSystem = new ManagementClass("Win32_OperatingSystem"))
            {
                operatingSystem.Get();

                operatingSystem.Scope.Options.EnablePrivileges = true;

                var args = operatingSystem.GetMethodParameters("Win32Shutdown");

                args["Flags"] = shutdownType;
                args["Reserved"] = "0";

                foreach (ManagementObject managementObject in operatingSystem.GetInstances())
                {
                    managementObject.InvokeMethod("Win32Shutdown", args, null);
                }
            }
        }
    }
}

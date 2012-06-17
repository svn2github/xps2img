using System;
using System.Management;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils
{
    [Flags]
    public enum ShutdownType
    {
        Forced          = 4,
        LogOff          = 0,
        Shutdown        = 1,
        Reboot          = 2,
        Hibernate       = 8,
        Suspend         = 16,
        ForcedLogOff    = LogOff    | Forced,
        ForcedShutdown  = Shutdown  | Forced,
        ForcedReboot    = Reboot    | Forced,
        ForcedHibernate = Hibernate | Forced,
        ForcedSuspend   = Suspend   | Forced
    };

    public static partial class SystemManagement
    {
        public static bool CanHibernate
        {
            get { return GetPwrCapabilities().HiberFilePresent; }
        }

        public static bool CanSleep
        {
            get { return GetPwrCapabilities().SleepButtonPresent; }
        }

        public static bool CanShutdown
        {
            get { return GetPwrCapabilities().PowerButtonPresent; }
        }

        public static void Shutdown(ShutdownType shutdownType, bool disableWakeEvent = false)
        {
            var isForced = ((int)shutdownType & (int)ShutdownType.Forced) != 0;

            if ((ShutdownType.Hibernate & shutdownType) != 0)
            {
                Application.SetSuspendState(PowerState.Hibernate, isForced, disableWakeEvent);
                return;		
            }
            
            if ((ShutdownType.Suspend & shutdownType) != 0)
            {
                Application.SetSuspendState(PowerState.Suspend, isForced, disableWakeEvent);
                return;		
            }

            using (var operatingSystem = new ManagementClass("Win32_OperatingSystem"))
            {
                operatingSystem.Get();

                operatingSystem.Scope.Options.EnablePrivileges = true;

                var args = operatingSystem.GetMethodParameters("Win32Shutdown");

                args["Flags"] = shutdownType;
                args["Reserved"] = 0;

                foreach (ManagementObject managementObject in operatingSystem.GetInstances())
                {
                    managementObject.InvokeMethod("Win32Shutdown", args, null);
                }
            }
        }
    }
}

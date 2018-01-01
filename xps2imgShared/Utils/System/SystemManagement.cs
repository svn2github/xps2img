using System;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Xps2Img.Shared.Utils.System
{
    [Flags]
    public enum ShutdownType
    {
        Forced          = 4,
        LogOff          = 0,
        Shutdown        = 1,
        Reboot          = 2,
        Hibernate       = 8,
        Sleep           = 16,
        ForcedLogOff    = LogOff    | Forced,
        ForcedShutdown  = Shutdown  | Forced,
        ForcedReboot    = Reboot    | Forced,
        ForcedHibernate = Hibernate | Forced,
        ForcedSleep     = Sleep     | Forced,
        Exit            = 32
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

        public static bool CanLogOff
        {
            get
            {
                using (var subKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                {
                    var value = subKey != null ? subKey.GetValue("NoLogOff") ?? 0 : 0;
                    return value is byte[] ? ((byte[]) value).All(b => b == 0) : value is int && (int)value == 0;
                }
            }
        }

        public static void Shutdown(ShutdownType shutdownType, bool disableWakeEvent = false)
        {
            var isForced = ((int)shutdownType & (int)ShutdownType.Forced) != 0;

            if ((ShutdownType.Hibernate & shutdownType) != 0)
            {
                Application.SetSuspendState(PowerState.Hibernate, isForced, disableWakeEvent);
                return;
            }
            
            if ((ShutdownType.Sleep & shutdownType) != 0)
            {
                Application.SetSuspendState(PowerState.Suspend, isForced, disableWakeEvent);
                return;
            }

            if (shutdownType == ShutdownType.Exit)
            {
                Application.Exit();
                return;
            }

            using (var operatingSystem = new ManagementClass("Win32_OperatingSystem"))
            {
                operatingSystem.Get();

                operatingSystem.Scope.Options.EnablePrivileges = true;

                var args = operatingSystem.GetMethodParameters("Win32Shutdown");

                args["Flags"] = shutdownType;
                args["Reserved"] = 0;

                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ManagementObject managementObject in operatingSystem.GetInstances())
                {
                    managementObject.InvokeMethod("Win32Shutdown", args, null);
                }
            }
        }
    }
}

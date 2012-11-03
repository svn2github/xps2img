using System.Runtime.InteropServices;
using System.Security;

namespace Xps2ImgUI.Utils
{
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable InconsistentNaming
    [SuppressUnmanagedCodeSecurity]
    public static partial class SystemManagement
    {
        [DllImport("powrprof.dll")]
        static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilites);

        private static SYSTEM_POWER_CAPABILITIES GetPwrCapabilities()
        {
            SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;
            GetPwrCapabilities(out systemPowerCapabilites);
            return systemPowerCapabilites;
        }

        private enum SYSTEM_POWER_STATE
        {
            PowerSystemUnspecified = 0,
            PowerSystemWorking = 1,
            PowerSystemSleeping1 = 2,
            PowerSystemSleeping2 = 3,
            PowerSystemSleeping3 = 4,
            PowerSystemHibernate = 5,
            PowerSystemShutdown = 6,
            PowerSystemMaximum = 7
        }

        private struct BATTERY_REPORTING_SCALE
        {
            public ulong Granularity;
            public ulong Capacity;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SYSTEM_POWER_CAPABILITIES
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool PowerButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SleepButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool LidPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS1;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS2;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS4;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS5;
            [MarshalAs(UnmanagedType.U1)]
            public bool HiberFilePresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool FullWake;
            [MarshalAs(UnmanagedType.U1)]
            public bool VideoDimPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ApmPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool UpsPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ThermalControl;
            [MarshalAs(UnmanagedType.U1)]
            public bool ProcessorThrottle;
            public byte ProcessorMinThrottle;
            public byte ProcessorMaxThrottle;
            [MarshalAs(UnmanagedType.U1)]
            public bool FastSystemS4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            private byte[] spare2;
            [MarshalAs(UnmanagedType.U1)]
            public bool DiskSpinDown;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private byte[] spare3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemBatteriesPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool BatteriesAreShortTerm;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public BATTERY_REPORTING_SCALE[] BatteryScale;
            public SYSTEM_POWER_STATE AcOnLineWake;
            public SYSTEM_POWER_STATE SoftLidWake;
            public SYSTEM_POWER_STATE RtcWake;
            public SYSTEM_POWER_STATE MinDeviceWakeState;
            public SYSTEM_POWER_STATE DefaultLowLatencyWake;
        }
    }
}

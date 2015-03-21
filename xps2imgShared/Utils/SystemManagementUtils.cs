using System;

using Xps2Img.Shared.Enums;
using Xps2Img.Shared.Utils.System;

namespace Xps2Img.Shared.Utils
{
    public static class SystemManagementUtils
    {
        private static ShutdownType PostActionToShutdownType(PostAction shutdownType)
        {
            switch (shutdownType)
            {
                case PostAction.Exit:      return ShutdownType.Exit;
                case PostAction.Hibernate: return ShutdownType.ForcedHibernate;
                case PostAction.LogOff:    return ShutdownType.ForcedLogOff;
                case PostAction.Reboot:    return ShutdownType.ForcedReboot;
                case PostAction.Shutdown:  return ShutdownType.ForcedShutdown;
                case PostAction.Sleep:     return ShutdownType.ForcedSleep;
            }
            throw new InvalidOperationException();
        }

        public static void Shutdown(PostAction shutdownType)
        {
            if (shutdownType == PostAction.DoNothing)
            {
                return;
            }
            #if DEBUG
            global::System.Diagnostics.Debug.WriteLine("SHUTDOWN TYPE: " + shutdownType);
            if (PostActionToShutdownType(shutdownType) == ShutdownType.Exit)
            #endif
            SystemManagement.Shutdown(PostActionToShutdownType(shutdownType));
        }
    }
}

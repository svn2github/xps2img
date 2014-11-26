using Xps2Img.Shared.Utils.System;

namespace Xps2Img.Shared.TypeConverters
{
    public enum PostAction
    { 
        DoNothing,
        Shutdown,
        Reboot,
        Sleep,
        Hibernate,
        LogOff,
        Exit
    };

    public class PostActionTypeConverter : FilterableEnumConverter<PostAction>
    {
        public static PostAction ChooseAction(PostAction currentAction, PostAction userAction)
        {
            return currentAction != PostAction.DoNothing ? currentAction : userAction;
        }

        protected override bool IsValueVisible(PostAction value)
        {
            switch (value)
            {
                case PostAction.Shutdown:  return SystemManagement.CanShutdown;
                case PostAction.Sleep:     return SystemManagement.CanSleep;
                case PostAction.Hibernate: return SystemManagement.CanHibernate;
                case PostAction.LogOff:    return SystemManagement.CanLogOff;
            }

            return true;
        }
    }
}

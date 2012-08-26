using System.Collections.Generic;
using System.Linq;

using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Converters
{
    public class PostActionConverter : StandardValuesTypeConverter
    {
        public const string Default   = "Do Nothing";
        public const string Shutdown  = "Shutdown";
        public const string Reboot    = "Reboot";
        public const string Sleep     = "Sleep";
        public const string Hibernate = "Hibernate";
        public const string LogOff    = "Log Off";
        public const string Exit      = "Exit";

        private static IEnumerable<string> Actions
        {
            get
            {
                yield return Default;
                if(SystemManagement.CanShutdown)    yield return Shutdown;
                yield return Reboot;
                if(SystemManagement.CanSleep)       yield return Sleep;
                if(SystemManagement.CanHibernate)   yield return Hibernate;
                if (SystemManagement.CanLogOff)     yield return LogOff;
                yield return Exit;
            }
        }

        public static string ChooseAction(string currentAction, string userAction)
        {
            return currentAction != Default ? currentAction : userAction;
        }

        public override string[] Values
        {
            get { return Actions.ToArray(); }
        }
    }
}

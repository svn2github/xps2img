using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CommandLine.GetOpt;
using CommandLine.Interfaces;
using CommandLine.Strings;
using CommandLine.Utils;

using Gnu.Getopt;

namespace CommandLine
{
    public static partial class Parser
    {
        private static IList<LongOptEx> GetOptionsList(Type optionsObjectType)
        {
            var optionsObject = Activator.CreateInstance(optionsObjectType);
            return GetOptionsList(optionsObject);
        }

        private static IList<LongOptEx> GetOptionsList(object optionsObject)
        {
            return GetOptionsList(optionsObject, false);
        }

        private static IList<LongOptEx> GetOptionsList(object optionsObject, bool ignoreDefault)
        {
            const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var result = new List<LongOptEx>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyInfo in optionsObject.GetType().GetProperties(bindingFlags))
            {
                var optionAttribute = (OptionAttribute)propertyInfo.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault();

                if (optionAttribute != null && (optionAttribute.Flags & OptionFlags.Ignore) == 0)
                {
                    result.Add(optionAttribute.Create(optionsObject, propertyInfo, ignoreDefault));
                }
            }

            return result;
        }

        private static readonly string[] HelpOpt = { "-h", "--help" };
        
        public static bool IsUsageRequested(string[] args)
        {
            return !args.Any() || (args.Length == 1 && HelpOpt.Contains(args.First()));
        }

        private static StringsSource _stringsSource;

        public static void RegisterStringsSource<TStringsSource>()
        {
            _stringsSource = new StringsSource(typeof(TStringsSource));
        }

        private static IStringsSourceKeyProvider _stringsSourceKeyProvider = DefaultStringsSourceKeyProvider.Instance;

        public static IStringsSourceKeyProvider StringsSourceKeyProvider
        {
            get { return _stringsSourceKeyProvider; }
            set { _stringsSourceKeyProvider = value ?? DefaultStringsSourceKeyProvider.Instance; }
        }

        private static string GetDescription<T>(string id)
        {
            var key = StringsSourceKeyProvider.FormatKey(typeof(T), id, "Description");
            return _stringsSource != null ? _stringsSource.GetString(key) : key;
        }

        public static T Parse<T>(string cmdline) where T : class
        {
            return Parse<T>(cmdline, ApplicationName, false);
        }

        public static T Parse<T>(string cmdline, string applicationName) where T : class
        {
            return Parse<T>(cmdline, applicationName, false);
        }

        public static T Parse<T>(string cmdline, bool ignoreErrors) where T : class
        {
            return Parse<T>(cmdline, ApplicationName, ignoreErrors);
        }

        public static T Parse<T>(string cmdline, string applicationName, bool ignoreErrors) where T : class
        {
            return Parse<T>(CommandLineToArgv(cmdline), applicationName, ignoreErrors);
        }

        public static T Parse<T>(string[] args) where T : class
        {
            return Parse<T>(args, ApplicationName);
        }

        public static T Parse<T>(string[] args, string applicationName) where T : class
        {
            return Parse<T>(args, applicationName, false);
        }

        public static T Parse<T>(string[] args, bool ignoreErrors) where T : class
        {
            return Parse<T>(args, ApplicationName, ignoreErrors);
        }

        public static T Parse<T>(string[] args, string applicationName, bool ignoreErrors) where T : class
        {
            var optionsObjectType = typeof(T);

            IntegrityCheck.Perform(optionsObjectType, GetOptionsList(optionsObjectType));

            var optionsObject = (T)Activator.CreateInstance(optionsObjectType);
            var longOpts = GetOptionsList(optionsObject).ToArray();

            // ReSharper disable once CoVariantArrayConversion
            var getopt = new Getopt(applicationName, args, longOpts.BuildOptString(), longOpts, false);

            var isValid = true;

            Action<LongOptEx, string> displayError = (longOptEx, message) =>
            {
                if (!ignoreErrors)
                {
                    Console.Error.WriteLine("{0}: option '{1}' {2}", ApplicationName, longOptEx.OptionNameToString(), message);
                    isValid = false;
                }
            };

            int opt;

            // Named args.
            while ((opt = getopt.getopt()) != -1)
            {
                if (!ignoreErrors && opt == '?')
                {
                    return null;
                }

                var closureOpt = opt;
                Array.ForEach(longOpts, o => { if (o.Val == closureOpt) o.SetPropertyValue(getopt.Optarg, displayError); });
            }

            var unnamedArgsLen = args.Length - getopt.Optind;

            // Unnamed args.
            var unnamedArgs = new string[unnamedArgsLen];

            Array.Copy(args, getopt.Optind, unnamedArgs, 0, unnamedArgsLen);

            var currentOption = 0;
            foreach (var longOptEx in longOpts)
            {
                if (!longOptEx.IsUnnamed)
                {
                    break;
                }

                var hasUnnamedOption = currentOption < unnamedArgs.Length;

                if (!ignoreErrors && longOptEx.IsRequired && !hasUnnamedOption)
                {
                    displayError(longOptEx, String.Format(Resources.Strings.Message_OptNotSpecified, ApplicationName, longOptEx.DisplayName));
                    return null;
                }

                if (hasUnnamedOption)
                {
                    longOptEx.SetPropertyValue(unnamedArgs[currentOption++], displayError);
                }
            }

            return isValid ? optionsObject : null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

using CommandLine;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.TypeConverters;
using Xps2Img.Shared.Utils;
using Xps2ImgUI.Model;

namespace Xps2ImgUI.Settings
{
    public static class SettingsManager
    {
        public static Xps2ImgModel LoadSettings()
        {
            var openFileDialog = new OpenFileDialog { Title = Resources.Strings.LoadSettingsTitle };
            InitFileDialog(openFileDialog);

            return openFileDialog.ShowDialog() == DialogResult.OK
                        ? LoadSettings(openFileDialog.FileName)
                        : null;
        }

        public static Xps2ImgModel LoadSettings(string file)
        {
            var commandLine = String.Join(StringUtils.SpaceString, File.ReadAllLines(file));
            return new Xps2ImgModel(Parser.Parse<UIOptions>(commandLine, true));
        }

        private static string ConvertToStringWith<T>(object value, string prefix = null, bool convert = true) where T : TypeConverter, new()
        {
            return convert ? prefix + (new T()).ConvertToString(value) : null;
        }

        private static IEnumerable<string> GetSettingsFileNameParts(Xps2ImgModel xps2ImgModel)
        {
            var srcFile = Path.GetFileNameWithoutExtension((xps2ImgModel.SrcFile ?? String.Empty).Trim());
            if (String.IsNullOrEmpty(srcFile))
            {
                yield break;
            }

            var optionsObject = xps2ImgModel.OptionsObject;
            var requiredSize  = optionsObject.RequiredSize;
            var dpi           = optionsObject.Dpi;

            yield return srcFile;
            yield return ConvertToStringWith<OptionsEnumConverter<Xps2ImgLib.ImageType>>(optionsObject.FileType);
            yield return optionsObject.Pages == null ? Resources.Strings.SettingsManager_NamePartAllPages : Resources.Strings.SettingsManager_NamePartSomePages;
            yield return ConvertToStringWith<CheckedDpiTypeConverter>(dpi, Resources.Strings.SettingsManager_NamePartDpi, dpi.HasValue && !requiredSize.HasValue && dpi != Options.Defaults.DpiValue);
            yield return ConvertToStringWith<RequiredSizeTypeConverter>(requiredSize, Resources.Strings.SettingsManager_NamePartSize, requiredSize.HasValue);
            yield return DateTime.Now.ToString(Resources.Strings.SettingsManager_NamePartTimestamp);
        }

        private static string GetSettingsFileName(Xps2ImgModel xps2ImgModel)
        {
            var parts = GetSettingsFileNameParts(xps2ImgModel).Where(p => p != null).ToArray();
            return parts.Any() ? String.Join(Resources.Strings.SettingsManager_NamePartSeparator, parts) : null;
        }

        public static void SaveSettings(Xps2ImgModel xps2ImgModel)
        {
            var saveFileDialog = new SaveFileDialog { Title = Resources.Strings.SaveSettingsTitle, AddExtension = true };
            InitFileDialog(saveFileDialog);

            saveFileDialog.FileName = GetSettingsFileName(xps2ImgModel);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, xps2ImgModel.FormatCommandLine(Options.ExcludedOnSave, true));
            }
        }

        public static bool IsSettingFile(string file)
        {
            return (Path.GetExtension(file) ?? String.Empty).ToLowerInvariant() == Resources.Strings.XPS2ImgFileExtension;
        }

        public static void SerializeSettings(ISettings settings)
        {
            try
            {
                var readBytes = new byte[0];

                var settingFile = EnsureSettingsFile();
                try
                {
                    File.SetAttributes(settingFile, File.GetAttributes(settingFile) & ~FileAttributes.ReadOnly);
                    readBytes = File.ReadAllBytes(settingFile);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                }

                using (var memoryStream = new MemoryStream())
                {
                    var serializer = new XmlSerializer(settings.GetSettingsType());
                    serializer.Serialize(memoryStream, settings.GetSettings());

                    var writtenBytes = memoryStream.ToArray();

                    if (writtenBytes.SequenceEqual(readBytes))
                    {
                        return;
                    }

                    using (var stream = new FileStream(settingFile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        stream.SetLength(0);
                        stream.Write(writtenBytes, 0, writtenBytes.Length);
                    }
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public static void DeserializeSettings(ISettings settings)
        {
            try
            {
                using (var stream = new FileStream(EnsureSettingsFile(), FileMode.Open, FileAccess.Read))
                {
                    var serializer = new XmlSerializer(settings.GetSettingsType());
                    settings.SetSettings(serializer.Deserialize(stream));
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public static bool IsPortable { get; private set; }

        static SettingsManager()
        {
            IsPortable = File.Exists(Application.ExecutablePath + PortableSuffix);

            var rootFolder = IsPortable
                                ? Path.GetDirectoryName(Application.ExecutablePath)
                                : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            Debug.Assert(rootFolder != null);

            DataFolder = IsPortable
                            ? rootFolder
                            : Path.Combine(rootFolder, Resources.Strings.WindowTitle);
        }

        private static string EnsureDataFolder()
        {
            Directory.CreateDirectory(DataFolder);
            return DataFolder;
        }

        private static string EnsureSettingsFolder()
        {
            var settingsFolder = Path.Combine(EnsureDataFolder(), SettingsFolder);

            Directory.CreateDirectory(settingsFolder);

            return settingsFolder;
        }

        private static string EnsureSettingsFile()
        {
            return Path.Combine(EnsureDataFolder(), SettingsFileName);
        }

        private static void InitFileDialog(FileDialog fileDialog)
        {
            fileDialog.Filter = Resources.Strings.FilterXPS2ImgFiles + Xps2Img.Shared.Resources.Strings.FilterAllFiles;
            fileDialog.InitialDirectory = EnsureSettingsFolder();
            fileDialog.RestoreDirectory = true;
        }

        private const string SettingsFolder = "Settings";
        private const string SettingsFileName = Program.ProductName + "UI.settings";

        private const string PortableSuffix = ".portable";

        private static readonly string DataFolder;       
    }
}

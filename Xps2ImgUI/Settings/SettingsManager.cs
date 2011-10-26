using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using CommandLine;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;

namespace Xps2ImgUI.Settings
{
    public static class SettingsManager
    {
        public static Xps2ImgModel LoadSettings()
        {
            if (_loadDialog == null)
            {
                InitFileDialog(_loadDialog = new OpenFileDialog());
            }

            return _loadDialog.ShowDialog() == DialogResult.OK
                        ? LoadSettings(_loadDialog.FileName)
                        : null;
        }

        public static Xps2ImgModel LoadSettings(string file)
        {
            var commandLine = String.Join("\x20", File.ReadAllLines(file));
            return new Xps2ImgModel(Parser.Parse<Options>(commandLine, true));
        }

        public static void SaveSettings(Xps2ImgModel xps2ImgModel)
        {
            if (_saveDialog == null)
            {
                InitFileDialog(_saveDialog = new SaveFileDialog());
            }

            #if DEBUG
            _saveDialog.FileName = DateTime.Now.ToString("x2i-yyyyMMdd-HHmmss");
            #endif

            if (_saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(_saveDialog.FileName, xps2ImgModel.FormatCommandLine(Options.ExcludedOnSave));
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
                var settingFile = EnsureSettingsFile();
                try
                {
                    File.SetAttributes(settingFile, File.GetAttributes(settingFile) & ~FileAttributes.ReadOnly);
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
                using (var stream = new FileStream(settingFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.SetLength(0);
                    var serializer = new XmlSerializer(settings.GetSettingsType());
                    serializer.Serialize(stream, settings.GetSettings());
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
            fileDialog.Filter = Resources.Strings.XPS2ImgFilesFilter + Utils.Filter.AllFiles;
            fileDialog.InitialDirectory = EnsureSettingsFolder();
            fileDialog.RestoreDirectory = true;
        }

        private const string SettingsFolder = "Settings";
        private const string SettingsFileName = "xps2imgUI.settings";

        private const string PortableSuffix = ".portable";

        private static readonly string DataFolder;
        
        private static OpenFileDialog _loadDialog;
        private static SaveFileDialog _saveDialog;
    }
}

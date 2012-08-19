using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

using CommandLine;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;
using Xps2ImgUI.Utils;

namespace Xps2ImgUI.Settings
{
    public static class SettingsManager
    {
        public static Xps2ImgModel LoadSettings()
        {
            if (_loadDialog == null)
            {
                InitFileDialog(_loadDialog = new OpenFileDialog { Title = Resources.Strings.LoadSettingsTitle });
            }

            return _loadDialog.ShowDialog() == DialogResult.OK
                        ? LoadSettings(_loadDialog.FileName)
                        : null;
        }

        public static Xps2ImgModel LoadSettings(string file)
        {
            var commandLine = String.Join(StringUtils.SpaceString, File.ReadAllLines(file));
            return new Xps2ImgModel(Parser.Parse<Options>(commandLine, true));
        }

        public static void SaveSettings(Xps2ImgModel xps2ImgModel)
        {
            if (_saveDialog == null)
            {
                InitFileDialog(_saveDialog = new SaveFileDialog { Title = Resources.Strings.SaveSettingsTitle, AddExtension = true });
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
            fileDialog.Filter = Resources.Strings.FilterXPS2ImgFiles + Resources.Strings.FilterAllFiles;
            fileDialog.InitialDirectory = EnsureSettingsFolder();
            fileDialog.RestoreDirectory = true;
        }

        private const string SettingsFolder = "Settings";
        private const string SettingsFileName = Program.ProductName + "UI.settings";

        private const string PortableSuffix = ".portable";

        private static readonly string DataFolder;
        
        private static OpenFileDialog _loadDialog;
        private static SaveFileDialog _saveDialog;
    }
}

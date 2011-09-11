using System;
using System.IO;
using System.Windows.Forms;

using CommandLine;

using Xps2Img.CommandLine;

namespace Xps2ImgUI.Model
{
    public static class SettingsManager
    {
        private const string optionsSeparator   = "\x20";
        private const string autoFileName       = "x2i-yyyyMMdd-HHmmss";

        private static OpenFileDialog _loadDialog;
        private static SaveFileDialog _saveDialog;

        public static Xps2ImgModel LoadSettings()
        {
            if (_loadDialog == null)
            {
                InitFileDialog(_loadDialog = new OpenFileDialog());
            }

            if (_loadDialog.ShowDialog() == DialogResult.OK)
            {
                var commandLine = String.Join(optionsSeparator, File.ReadAllLines(_loadDialog.FileName));
                return new Xps2ImgModel(Parser.Parse<Options>(commandLine, true));
            }

            return null;
        }

        public static void SaveSettings(Xps2ImgModel xps2ImgModel)
        {
            if (_saveDialog == null)
            {
                InitFileDialog(_saveDialog = new SaveFileDialog());
            }

            _saveDialog.FileName = DateTime.Now.ToString(autoFileName);

            if (_saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(_saveDialog.FileName, xps2ImgModel.FormatCommandLine());
            }
        }

        private static void InitFileDialog(FileDialog fileDialog)
        {
            fileDialog.Filter = Resources.Strings.XPS2ImgFilesFilter + Utils.Filter.AllFiles;
            fileDialog.InitialDirectory = EnsureApplicationDataFolder();
            fileDialog.RestoreDirectory = true;
        }

        private static string EnsureApplicationDataFolder()
        {
            var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(rootFolder, Resources.Strings.WindowTitle);

            Directory.CreateDirectory(appFolder);

            return appFolder;
        }
    }
}

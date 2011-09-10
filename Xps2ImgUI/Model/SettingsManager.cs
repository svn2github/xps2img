using System;
using System.IO;
using System.Windows.Forms;

using CommandLine;

using Xps2Img.CommandLine;

namespace Xps2ImgUI.Model
{
    public static class SettingsManager
    {
        private const string optionsSeparator = "\x20";
        private const string autoFileName = "yyyyMMdd-HHmmss";

        public static Xps2ImgModel LoadSettings()
        {
            var initialDirectory = EnsureApplicationDataFolder();
            using (var dialog = new OpenFileDialog { Filter = Resources.Strings.XPS2ImgFilesFilter + Utils.Filter.AllFiles, InitialDirectory = initialDirectory })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var commandLine = String.Join(optionsSeparator, File.ReadAllLines(dialog.FileName));
                    return new Xps2ImgModel(Parser.Parse<Options>(commandLine.Split(optionsSeparator[0]), Parser.ApplicationName, true));
                }
            }
            return null;
        }

        public static void SaveSettings(Xps2ImgModel xps2ImgModel)
        {
            var initialDirectory = EnsureApplicationDataFolder();
            var fileName = DateTime.Now.ToString(autoFileName);
            using (var dialog = new SaveFileDialog { Filter = Resources.Strings.XPS2ImgFilesFilter + Utils.Filter.AllFiles, InitialDirectory = initialDirectory, FileName = fileName })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, xps2ImgModel.FormatCommandLine());
                }
            }
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

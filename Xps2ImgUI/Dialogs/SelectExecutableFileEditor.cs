﻿using System;

namespace Xps2ImgUI.Dialogs
{
    public class SelectExecutableFileEditor : SelectFileEditor
    {
        public SelectExecutableFileEditor() :
            base(Resources.Strings.FilterExeFiles + Resources.Strings.FilterAllFiles)
        {
            Title = Resources.Strings.SelectExecutableFile;
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }
    }
}

﻿using System.ComponentModel;
using System.Drawing.Design;

namespace Xps2Img.Shared.TypeEditors
{
    public class OrdinalUITypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
}

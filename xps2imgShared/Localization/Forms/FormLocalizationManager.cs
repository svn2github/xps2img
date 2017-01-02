using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xps2Img.Shared.Localization.Forms
{
    public static class FormLocalizationManager
    {
        private static readonly List<IFormLocalization> FormLocalizations = new List<IFormLocalization>();

        static FormLocalizationManager()
        {
            LocalizationManager.UICultureChanged += OnUICultureChanged;
        }

        public static void EnableFormLocalization(this IFormLocalization formLocalization)
        {
            Debug.Assert(!FormLocalizations.Contains(formLocalization));

            FormLocalizations.Add(formLocalization);

            formLocalization.Closed += OnClosed;

            formLocalization.UICultureChanged();
        }

        private static void OnClosed(object sender, EventArgs e)
        {
            var formLocalization = (IFormLocalization)sender;

            FormLocalizations.Remove(formLocalization);

            formLocalization.Closed -= OnClosed;
        }

        private static void OnUICultureChanged(object sender, EventArgs eventArgs)
        {
            foreach (var formLocalization in FormLocalizations)
            {
                formLocalization.UICultureChanged();
            }
        }
    }
}

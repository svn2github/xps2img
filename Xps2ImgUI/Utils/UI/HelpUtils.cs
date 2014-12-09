using System;
using System.Linq;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static partial class HelpUtils
    {
        private static Form _helpForm;

        private static Form HelpForm
        {
            get { return _helpForm ?? (_helpForm = new Form()); }
        }

        public static void ShowHelpTableOfContents()
        {
            Help.ShowHelp(HelpForm, HelpFile, HelpNavigator.TableOfContents);
        }

        public static void ShowHelpTopicId(string topicId)
        {
            Help.ShowHelp(HelpForm, HelpFile, HelpNavigator.TopicId, topicId);
        }

        private static bool ShowPropertyHelp(string text, string fallbackTopicId = null)
        {
            string topicId;

            if (String.IsNullOrEmpty(text))
            {
                return false;
            }

            if (!PropertyToTopicMap.TryGetValue(text, out topicId))
            {
                topicId = CategoryToTopicMap.FirstOrDefault(kvp => kvp.Key == text).Value ?? fallbackTopicId;

                if (topicId == null)
                {
                    return false;
                }
            }

            ShowHelpTopicId(topicId);

            return true;
        }

        public static bool ShowPropertyHelp(PropertyGrid propertyGrid, string fallbackTopicId = null)
        {
            if (!propertyGrid.Focused)
            {
                return false;
            }

            var gridItem = propertyGrid.SelectedGridItem;
            return gridItem != null && ShowPropertyHelp(gridItem.IsCategory() ? propertyGrid.GetCategoryName(gridItem) : gridItem.PropertyDescriptor.Name, fallbackTopicId);
        }
    }
}

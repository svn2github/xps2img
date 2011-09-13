using System;
using System.Linq;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static class PropertyGridUtils
    {
        // http://www.vb-helper.com/howto_net_select_propertygrid_item2.html
        private static GridItem FindItem(PropertyGrid propertyGrid, Func<GridItem, bool> findGridItem)
        {
            var node = propertyGrid.SelectedGridItem;
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return FindNode(node, findGridItem);
        }

        private static GridItem FindNode(GridItem node, Func<GridItem, bool> findGridItem)
        {
            return findGridItem(node) ?
                    node :
                    node.GridItems
                      .Cast<GridItem>()
                      .Select(gridItem => FindNode(gridItem, findGridItem))
                      .FirstOrDefault(gridItem => gridItem != null);
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string itemLabel)
        {
            SelectGridItem(propertyGrid, itemLabel, null);
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string itemLabel, Func<GridItem, bool> searchFunc)
        {
            SelectGridItem(propertyGrid, itemLabel, searchFunc, false);
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string itemLabel, Func<GridItem, bool> searchFunc, bool focus)
        {
            if (searchFunc == null)
            {
                searchFunc = gi => gi.Label == itemLabel;
            }

            var gridItem = FindItem(propertyGrid, searchFunc);

            if (gridItem == null)
            {
                return;
            }

            propertyGrid.SelectedGridItem = gridItem;

            if (focus)
            {
                propertyGrid.Focus();
                propertyGrid.Select();
                propertyGrid.SelectedGridItem.Select();
                SendKeys.SendWait("{TAB}");
            }
        }
    }
}

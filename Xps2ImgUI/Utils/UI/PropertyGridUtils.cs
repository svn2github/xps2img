using System;
using System.Linq;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static class PropertyGridUtils
    {
        // http://www.vb-helper.com/howto_net_select_propertygrid_item2.html

        private static GridItem GetParentNode(PropertyGrid propertyGrid)
        {
            var node = propertyGrid.SelectedGridItem;
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }

        private static GridItem FindItem(PropertyGrid propertyGrid, Func<GridItem, bool> findGridItem)
        {
            return FindNode(GetParentNode(propertyGrid), findGridItem);
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

        public static void SelectFirstGridItem(this PropertyGrid propertyGrid)
        {
            Func<GridItem, bool> isEmpty = n => n == null || n.GridItems.Count == 0;

            var node = GetParentNode(propertyGrid);
            if (isEmpty(node))
            {
                return;
            }

            node = node.GridItems[0];
            if (isEmpty(node))
            {
                return;
            }

            propertyGrid.SelectedGridItem = node.GridItems[0];
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string itemLabel, Func<GridItem, bool> searchFunc = null, bool focus = false)
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

            if (gridItem.Parent != null)
            {
                gridItem.Parent.Expanded = true;
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

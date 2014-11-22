using System;
using System.Linq;
using System.Windows.Forms;

namespace Xps2ImgUI.Utils.UI
{
    public static class PropertyGridUtils
    {
        // http://www.vb-helper.com/howto_net_select_propertygrid_item2.html

        public static GridItem GetParentGridItem(this PropertyGrid propertyGrid)
        {
            var gridItem = propertyGrid.SelectedGridItem;
            while (gridItem.Parent != null)
            {
                gridItem = gridItem.Parent;
            }
            return gridItem;
        }

        public static GridItem FindGridItem(this PropertyGrid propertyGrid, Func<GridItem, bool> findGridItem)
        {
            return FindGridItem(GetParentGridItem(propertyGrid), findGridItem);
        }

        public static GridItem FindGridItem(this GridItem gridItem, Func<GridItem, bool> findGridItem)
        {
            return findGridItem(gridItem) ?
                    gridItem :
                    gridItem.GridItems
                      .Cast<GridItem>()
                      .Select(g => FindGridItem(g, findGridItem))
                      .FirstOrDefault(g => g != null);
        }

        public static void SelectFirstGridItem(this PropertyGrid propertyGrid)
        {
            Func<GridItem, bool> isEmpty = n => n == null || n.GridItems.Count == 0;

            var gridItem = GetParentGridItem(propertyGrid);
            if (isEmpty(gridItem))
            {
                return;
            }

            gridItem = gridItem.GridItems[0];
            if (isEmpty(gridItem))
            {
                return;
            }

            propertyGrid.SelectedGridItem = gridItem.GridItems[0];
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string gridItemLabel, Func<GridItem, bool> searchFunc = null, bool focus = false)
        {
            if (searchFunc == null)
            {
                searchFunc = gi => gi.Label == gridItemLabel;
            }

            var gridItem = FindGridItem(propertyGrid, searchFunc);

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

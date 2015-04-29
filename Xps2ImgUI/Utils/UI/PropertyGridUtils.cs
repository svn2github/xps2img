using System;
using System.ComponentModel;
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

        public static void SelectFirstGridItem(this PropertyGrid propertyGrid, bool focus = true, bool focusEdit = true)
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

            gridItem.Expanded = true;

            propertyGrid.SelectedGridItem = gridItem.GridItems[0];

            propertyGrid.FocusSelectedGridItem(focus, focusEdit);
        }

        public static void SelectGridItem(this PropertyGrid propertyGrid, string labelOrPropertyName, bool searchByPropertyName = false, Func<GridItem, bool> searchFunc = null, bool focus = false, bool focusEdit = true)
        {
            if (searchFunc == null)
            {
                searchFunc = gi => searchByPropertyName
                                    ? gi.PropertyDescriptor != null && gi.PropertyDescriptor.Name == labelOrPropertyName
                                    : gi.Label == labelOrPropertyName;
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

            propertyGrid.FocusSelectedGridItem(focus, focusEdit);
        }

        public static void FocusSelectedGridItem(this PropertyGrid propertyGrid, bool focus = true, bool focusEdit = true)
        {
            if (!focus)
            {
                return;
            }

            propertyGrid.Focus();
            propertyGrid.Select();

            if (focusEdit)
            {
                propertyGrid.BeginInvoke(new Action(() =>
                {
                    Application.DoEvents();
                    SendKeys.Send("{TAB}");
                }));
            }
        }

        private static GridItem GetGridItem(PropertyGrid propertyGrid, GridItem gridItem)
        {
            return gridItem ?? (propertyGrid != null ? propertyGrid.SelectedGridItem : null);
        }

        private static bool TestItemType(this PropertyGrid propertyGrid, GridItem gridItem, GridItemType gridItemType)
        {
            return GetGridItem(propertyGrid, gridItem).GridItemType == gridItemType;
        }

        public static bool IsCategory(this GridItem gridItem)
        {
            return TestItemType(null, gridItem, GridItemType.Category);
        }

        public static bool HasPropertyDescriptor(this PropertyGrid propertyGrid, GridItem gridItem = null)
        {
            return GetGridItem(propertyGrid, gridItem).PropertyDescriptor != null;
        }

        public static bool HasPropertyDescriptor(this GridItem gridItem)
        {
            return HasPropertyDescriptor(null, gridItem);
        }

        public static string GetCategoryName(this PropertyGrid propertyGrid, GridItem categoryGridItem = null)
        {
            var gridItem = GetGridItem(propertyGrid, categoryGridItem).FindGridItem(g => !g.IsCategory());
            var categoryAttribute = gridItem.PropertyDescriptor == null ? null : gridItem.PropertyDescriptor.Attributes.OfType<CategoryAttribute>().FirstOrDefault();
            return categoryAttribute != null ? categoryAttribute.Category : null;
        }
    }
}

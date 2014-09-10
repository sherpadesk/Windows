using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk.Extensions
{
    public static class UIElementExtensions
    {
        public static Grid ParentGrid(this FrameworkElement element)
        {
            if (element.Parent == null)
                return null;
            var grid = element.Parent as Grid;
            if (grid != null)
                return grid;
            return element.ParentGrid();
        }

        public static void MainPage(this FrameworkElement element, Action<MainPage> found)
        {
            if (element == null)
                return;
            var page = element as MainPage;
            if (page != null)
                found(page);
            else
                (element.Parent as FrameworkElement).MainPage(found);
        }

    }
}

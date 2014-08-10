using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk.Common
{
    public static class UIElementExtensions
    {
        public static Grid ParentGrid(this FrameworkElement element)
        {
            if (element.Parent == null)
                return null;
            if (element.Parent is Grid)
                return (Grid)element.Parent;
            else
                return element.ParentGrid();
        }

        public static void MainPage(this FrameworkElement element, Action<MainPage> found)
        {
            if (element == null)
                return;
            if (element is MainPage)
                found((MainPage)element);
            else
                (element.Parent as FrameworkElement).MainPage(found);
        }

    }
}

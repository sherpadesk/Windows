﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        public Timesheet()
        {
            this.InitializeComponent();
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

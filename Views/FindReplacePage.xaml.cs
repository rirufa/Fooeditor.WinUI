﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace FooEditor.WinUI.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class FindReplacePage : Page
    {
        public FindReplacePage()
        {
            this.InitializeComponent();
            this.Loaded += FindReplacePage_Loaded;
        }

        private void FindReplacePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.FindPartternInputUI.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
        }
    }
}

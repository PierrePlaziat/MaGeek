﻿using System;
using System.Windows;
using MageekDesktopClient.UI.ViewModels.Windows;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class MainWindow : BaseWindow
    {

        public MainWindow(MainWindowViewModel vm)
        {
            Application.Current.MainWindow = this;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
        }

    }

}

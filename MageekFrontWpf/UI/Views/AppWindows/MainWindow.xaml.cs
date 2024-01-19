using System;
using System.Windows;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

namespace MaGeek
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

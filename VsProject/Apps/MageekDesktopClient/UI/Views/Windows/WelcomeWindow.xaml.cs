using MageekDesktopClient.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class WelcomeWindow : BaseWindow
    {
        private WelcomeWindowViewModel vm;

        public WelcomeWindow(WelcomeWindowViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            PasswordBox.Focus();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            vm.InputPass = ((PasswordBox)sender).Password;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                vm.Connect().ConfigureAwait(false);
            }
        }

    }

}

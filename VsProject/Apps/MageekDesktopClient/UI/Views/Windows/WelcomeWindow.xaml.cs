using MageekDesktopClient.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;
using System;
using System.Windows.Input;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class WelcomeWindow : BaseWindow
    {

        private readonly WelcomeWindowViewModel vm;

        public WelcomeWindow(WelcomeWindowViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //vm.Init().ConfigureAwait(false);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

    }

}

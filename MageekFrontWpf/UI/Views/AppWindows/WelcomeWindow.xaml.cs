using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using System;
using System.Windows.Input;

namespace MageekFrontWpf.UI.Views.AppWindows
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

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            vm.Init().ConfigureAwait(false);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

    }

}

using MaGeek.UI;
using MageekFrontWpf.ViewModels;
using System;
using System.Windows.Input;

namespace MaGeek
{

    public partial class WelcomeWindow : BaseWindow
    {

        private readonly WelcomeViewModel vm;

        public WelcomeWindow(WelcomeViewModel vm)
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

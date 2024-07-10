using MageekDesktop.UI.ViewModels;
using PlaziatWpf.Mvvm;
using System;
using PlaziatWpf.Mvvm;

namespace MageekDesktop.UI.Views
{

    public partial class TopMenu : BaseUserControl
    {

        public TopMenu() {}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = ServiceHelper.GetService<TopMenuViewModel>();
            InitializeComponent();
        }

    }

}

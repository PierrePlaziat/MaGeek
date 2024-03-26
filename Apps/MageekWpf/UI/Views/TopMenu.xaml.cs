using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels;
using System;

namespace MageekFrontWpf.UI.Views
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

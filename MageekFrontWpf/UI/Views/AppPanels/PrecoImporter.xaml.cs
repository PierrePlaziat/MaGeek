using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using System;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class PrecoList : BaseUserControl
    {

        PrecoListViewModel vm;

        public PrecoList(PrecoListViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            vm.Init().ConfigureAwait(false);
        }

    }

}

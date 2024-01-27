using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using System;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class ImporterUi : BaseUserControl
    {

        public ImporterUi(){}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = ServiceHelper.GetService<ImporterUiViewModel>();
            InitializeComponent();
        }

    }

}

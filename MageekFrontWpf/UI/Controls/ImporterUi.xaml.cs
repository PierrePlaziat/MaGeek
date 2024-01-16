using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System;

namespace MaGeek.UI.Menus
{

    public partial class ImporterUi : BaseUserControl
    {

        public ImporterUi() {}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = ServiceHelper.GetService<ImporterUiViewModel>();
            InitializeComponent();
        }

    }
}

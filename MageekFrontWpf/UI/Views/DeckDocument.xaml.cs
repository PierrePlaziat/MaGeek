using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System;

namespace MageekFrontWpf.UI.Views
{

    public partial class DeckDocument : BaseUserControl
    {

        public DeckDocument() {}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = ServiceHelper.GetService<DeckDocumentViewModel>();
            InitializeComponent();
        }

    }

}

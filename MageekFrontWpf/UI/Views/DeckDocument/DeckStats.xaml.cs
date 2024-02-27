using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckStats : BaseUserControl
    {

        private DeckDocumentViewModel vm;

        public DeckStats() {}

        public void SetDataContext(DeckDocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

    }

}

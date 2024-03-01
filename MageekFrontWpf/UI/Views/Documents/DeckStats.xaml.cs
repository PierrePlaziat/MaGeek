using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckStats : BaseUserControl
    {

        private DocumentViewModel vm;

        public DeckStats() {}

        public void SetDataContext(DocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

    }

}

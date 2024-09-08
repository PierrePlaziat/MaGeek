using MageekDesktopClient.UI.ViewModels;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppPanels
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

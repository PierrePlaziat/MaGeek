using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using System.Windows;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckTable : BaseUserControl
    {

        public DeckTable()
        {
        }
        
        public DeckTable(DeckTableViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}

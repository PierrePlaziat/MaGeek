using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

namespace MaGeek.UI
{

    public partial class DeckList : BaseUserControl
    {

        public DeckList(DeckListViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }

}

using CommunityToolkit.Mvvm.ComponentModel;

namespace MageekMaui.ViewModels
{

    public partial class DeckViewModel : ViewModel
    {

        private MageekClient client;

        [ObservableProperty]
        List<string> deckList;

        public DeckViewModel(MageekClient client)
        {
            this.client = client;
        }

    }
}
using CommunityToolkit.Mvvm.ComponentModel;

namespace MageekMaui.ViewModels
{

    public partial class DeckViewModel : ViewModel
    {

        private IMageekClient client;

        [ObservableProperty]
        List<string> deckList;

        public DeckViewModel(IMageekClient client)
        {
            this.client = client;
        }

    }
}
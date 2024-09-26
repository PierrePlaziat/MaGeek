using CommunityToolkit.Mvvm.ComponentModel;
using MageekClient.Services;
using MageekCore.Services;

namespace MageekMobile.ViewModels
{

    public partial class DeckViewModel : ViewModel
    {

        private IMageekService client;

        [ObservableProperty]
        List<string> deckList;

        public DeckViewModel(IMageekService client)
        {
            this.client = client;
        }

    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Data.Collection.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.UI.ViewModels
{
    
    //events.UpdateDeckEvent += async () => { await Reload(); };
    //events.UpdateDeckListEvent += async () => { await Reload(); };
    public partial class DeckListViewModel : BaseViewModel
    {

        private WindowsManager win;
        private SettingService config;
        private DialogService dialog;

        public DeckListViewModel(
            SettingService config,
            DialogService dialog,
            WindowsManager win
        ){
            this.win = win;
            this.config = config;
            this.dialog = dialog;
        }

        [ObservableProperty] private IEnumerable<Deck> decks;
        [ObservableProperty] private string filterString = "";
        [ObservableProperty] private bool isLoading = false;

        protected override void OnActivated()
        {
            base.OnActivated();
            Reload().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task Reload()
        {
            IsLoading = true;
            Decks = FilterDeck(await MageekService.MageekService.GetDecks());
            IsLoading = false;
        }

        [RelayCommand]
        private async Task AddDeck()
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await MageekService.MageekService.CreateDeck_Empty(title, "");
            await Reload();
        }

        [RelayCommand]
        private async Task RenameDeck(string deckId)
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await MageekService.MageekService.RenameDeck(deckId, title);
            await Reload();
        }

        [RelayCommand]
        private async Task DuplicateDeck(string deckId)
        {
            await MageekService.MageekService.DuplicateDeck(deckId);
            await Reload();
        }

        [RelayCommand]
        private async Task DeleteDeck(string deckId)
        {
            await MageekService.MageekService.DeleteDeck(deckId);
            await Reload();
        }

        [RelayCommand]
        private async Task EstimateDeckPrice(string deckId)
        {
            var totalPrice = await MageekService.MageekService.EstimateDeckPrice(deckId, config.Settings[AppSetting.Currency]);
            MessageBox.Show("Estimation : " + totalPrice.Item1 + " €" + "\n" +
                            "Missing : " + totalPrice.Item2);
        }

        [RelayCommand]
        private async Task GetAsTxtList(string deckId)
        {
            string txt = await MageekService.MageekService.DeckToTxt(deckId);
            if (!string.IsNullOrEmpty(txt))
            {
                Clipboard.SetText(txt);
                win.Notif("Deck:", "Copied to clipboard.");
            }
        }

        private IEnumerable<Deck> FilterDeck(IEnumerable<Deck> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable.Where(x => x.Title.ToLower().Contains(FilterString.ToLower()))
                             .OrderBy(x => x.Title);
        }

    }

}

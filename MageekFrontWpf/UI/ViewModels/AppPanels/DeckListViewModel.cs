using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekCore;
using MageekCore.Data.Collection.Entities;
using MageekCore.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{

    public partial class DeckListViewModel : BaseViewModel, 
        IRecipient<UpdateDeckListMessage>, 
        IRecipient<UpdateDeckMessage>
    {

        private WindowsService wins;
        private MageekService mageek;
        private SettingService config;
        private DialogService dialog;

        public DeckListViewModel(
            WindowsService wins,
            SettingService config,
            DialogService dialog, 
            MageekService mageek
        ){
            this.wins = wins;
            this.mageek = mageek;
            this.config = config;
            this.dialog = dialog;
            WeakReferenceMessenger.Default.RegisterAll(this);
            Reload().ConfigureAwait(false);
        }

        [ObservableProperty] private IEnumerable<Deck> decks;
        [ObservableProperty] private string filterString = "";
        [ObservableProperty] private bool isLoading = false;

        public void Receive(UpdateDeckListMessage message)
        {
            Reload().ConfigureAwait(false);
        }

        public void Receive(UpdateDeckMessage message)
        {
            Reload().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task Reload()
        {
            Logger.Log("Reload");
            IsLoading = true;
            Decks = FilterDeck(await mageek.GetDecks());
            IsLoading = false;
        }

        [RelayCommand]
        public async Task SelectDeck(string deckId)
        {
            wins.OpenDoc(await mageek.GetDeck(deckId),null);
        }

        [RelayCommand]
        public async Task AddDeck()
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await mageek.CreateDeck_Empty(title, "");
            await Reload();
        }

        [RelayCommand]
        public async Task RenameDeck(string deckId)
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await mageek.RenameDeck(deckId, title);
            await Reload();
        }

        [RelayCommand]
        public async Task DuplicateDeck(string deckId)
        {
            await mageek.DuplicateDeck(deckId);
            await Reload();
        }

        [RelayCommand]
        public async Task DeleteDeck(string deckId)
        {
            await mageek.DeleteDeck(deckId);
            await Reload();
        }

        [RelayCommand]
        public async Task EstimateDeckPrice(string deckId)
        {
            var totalPrice = await mageek.EstimateDeckPrice(deckId, config.Settings[AppSetting.Currency]);
            MessageBox.Show("Estimation : " + totalPrice.Item1 + " €" + "\n" +
                            "Missing : " + totalPrice.Item2);
        }

        [RelayCommand]
        public async Task GetAsTxtList(string deckId)
        {
            string txt = await mageek.DeckToTxt(deckId);
            if (!string.IsNullOrEmpty(txt))
            {
                Clipboard.SetText(txt);
                dialog.Notif("Deck:", "Copied to clipboard.");
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

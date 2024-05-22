using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data.Collection.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using MageekFrontWpf.Framework.AppValues;
using PlaziatTools;
using MageekCore.Services;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{

    public partial class DeckListViewModel : ObservableViewModel, 
        IRecipient<UpdateDeckListMessage>,
        IRecipient<LaunchAppMessage>
    {

        private IMageekService mageek;
        private WindowsService wins;
        private SettingService config;
        private DialogService dialog;

        public DeckListViewModel(
            IMageekService mageek,
            WindowsService wins,
            SettingService config,
            DialogService dialog
        ){
            this.wins = wins;
            this.mageek = mageek;
            this.config = config;
            this.dialog = dialog;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] private IEnumerable<Deck> decks;
        [ObservableProperty] private string filterString = "";
        [ObservableProperty] private bool isLoading = false;

        public void Receive(LaunchAppMessage message)
        {
            Reload().ConfigureAwait(false);
        }

        public void Receive(UpdateDeckListMessage message)
        {
            Reload().ConfigureAwait(false);
        }

        [RelayCommand]
        public async Task Reload()
        {
            Logger.Log("Reload");
            IsLoading = true;
            
            Decks = FilterDeck(await mageek.Decks_All());
            OnPropertyChanged(nameof(Decks));
            IsLoading = false;
        }

        [RelayCommand]
        public async Task SelectDeck(string deckId)
        {
            DocumentArguments doc = new DocumentArguments(deck : await mageek.Decks_Get(deckId));
            wins.OpenDoc(doc);
        }

        [RelayCommand]
        public async Task AddDeck()
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await mageek.Decks_Create(title, "");
            await Reload();
        }

        [RelayCommand]
        public async Task RenameDeck(string deckId)
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await mageek.Decks_Rename(deckId, title);
            await Reload();
        }

        [RelayCommand]
        public async Task DuplicateDeck(string deckId)
        {
            await mageek.Decks_Duplicate(deckId);
            await Reload();
        }

        [RelayCommand]
        public async Task DeleteDeck(string deckId)
        {
            if (deckId == null) return;
            await mageek.Decks_Delete(deckId);
            await Reload();
        }

        [RelayCommand]
        public async Task EstimateDeckPrice(string deckId)
        {
            //var totalPrice = await mageek.EstimateDeckPrice(deckId, config.Settings[Setting.Currency]);
            //MessageBox.Show("Estimation : " + totalPrice.Item1 + " €" + "\n" +
            //                "Missing : " + totalPrice.Item2);
        }

        [RelayCommand]
        public async Task GetAsTxtList(string deckId)
        {
            string txt = await mageek.CardLists_FromDeck(deckId);
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

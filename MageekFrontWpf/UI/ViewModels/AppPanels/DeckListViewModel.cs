using CommunityToolkit.Mvvm.ComponentModel;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Data.Collection.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class DeckListViewModel : BaseViewModel
    {

        private AppEvents events;
        private AppState state;
        private SettingService config;
        private DialogService dialog;

        public DeckListViewModel(
            AppEvents events,
            AppState state,
            SettingService config,
            DialogService dialog
        )
        {
            this.events = events;
            this.state = state;
            this.config = config;
            this.dialog = dialog;
            events.UpdateDeckEvent += async () => { await Reload(); };
            events.UpdateDeckListEvent += async () => { await Reload(); };
            Reload().ConfigureAwait(false);
        }

        [ObservableProperty] private IEnumerable<Deck> decks;
        [ObservableProperty] private string filterString = "";
        [ObservableProperty] private bool isLoading = false;

        private async Task Reload()
        {
            IsLoading = true;
            await Task.Run(async () =>
            {
                Decks = FilterDeckEnumerator(await MageekService.MageekService.GetDecks());
            });
            IsLoading = false;
        }

        private IEnumerable<Deck> FilterDeckEnumerator(IEnumerable<Deck> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable.Where(x => x.Title.ToLower().Contains(FilterString.ToLower()))
                             .OrderBy(x => x.Title);
        }

        //private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var deck = decklistbox.SelectedItem as Deck;
        //    if (deck != null) events.RaiseDeckSelect(deck);
        //}

        //private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (decklistbox.SelectedItem is Deck deck) events.RaiseDeckSelect(deck.DeckId);
        //}

        private async void AddDeck(object sender, RoutedEventArgs e)
        {
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await MageekService.MageekService.CreateDeck_Empty(title, "");
            await Reload();
        }

        private async void RenameDeck(object sender, RoutedEventArgs e)
        {
            if (state.SelectedDeck == null) return;
            string title = dialog.GetInpurFromUser("What title?", "New title");
            await MageekService.MageekService.RenameDeck(state.SelectedDeck.DeckId, title);
            await Reload();
        }

        //private async void DuplicateDeck(object sender, RoutedEventArgs e)
        //{
        //    if (decklistbox.SelectedIndex == -1) return;
        //    await MageekService.MageekService.DuplicateDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        //    await Reload();
        //}

        //private async void DeleteDeck(object sender, RoutedEventArgs e)
        //{
        //    if (decklistbox.SelectedIndex == -1) return;
        //    var v = decklistbox.SelectedItems;
        //    List<Deck> v2 = new();
        //    foreach (var vv in v) v2.Add((Deck)vv);
        //    await MageekService.MageekService.DeleteDecks(v2);
        //    await Reload();
        //}

        private async void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (state.SelectedDeck == null) return;
            var totalPrice = await MageekService.MageekService.EstimateDeckPrice(state.SelectedDeck.DeckId, config.Settings[Settings.Currency]);

            MessageBox.Show("Estimation : " + totalPrice.Item1 + " €" + "\n" +
                            "Missing : " + totalPrice.Item2);
        }

        //private async void GetAsTxtList(object sender, RoutedEventArgs e)
        //{
        //    string txt = await MageekService.MageekService.DeckToTxt(((Deck)decklistbox.SelectedItem).DeckId);
        //    //new TxtImporter().Show();
        //    // TODO load content
        //}
    }
}

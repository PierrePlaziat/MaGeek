using MaGeek;
using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Data.Collection.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.ViewModels
{
    public class DeckListViewModel : BaseViewModel
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
        ){
            this.events = events;
            this.state = state;
            this.config = config;
            this.dialog = dialog;
            ConfigureEvents();
            DelayLoad().ConfigureAwait(false);
        }

        #region Attributes

        public IEnumerable<Deck> Decks { get; private set; }

        #region Filter

        private string filterString = "";
        public string FilterString
        {
            get { return filterString; }
            set
            {
                filterString = value;
                OnPropertyChanged(nameof(FilterString));
                Reload().ConfigureAwait(false);
            }
        }

        private IEnumerable<Deck> FilterDeckEnumerator(IEnumerable<Deck> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable.Where(x => x.Title.ToLower().Contains(FilterString.ToLower()))
                             .OrderBy(x => x.Title);
        }

        #endregion

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;

        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive
        {
            get
            {
                if (Decks == null) return Visibility.Visible;
                return Decks.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #endregion

        #region CTOR

        private void ConfigureEvents()
        {
            events.UpdateDeckEvent += async () => { await Reload(); };
            events.UpdateDeckListEvent += async () => { await Reload(); };
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            events.RaiseUpdateDeckList();
        }

        #endregion

        #region Methods

        private async Task Reload()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                Decks = FilterDeckEnumerator(await MageekService.MageekService.GetDecks());
                OnPropertyChanged(nameof(Decks));
                IsLoading = Visibility.Collapsed;
            });
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

        #endregion

        //private async void GetAsTxtList(object sender, RoutedEventArgs e)
        //{
        //    string txt = await MageekService.MageekService.DeckToTxt(((Deck)decklistbox.SelectedItem).DeckId);
        //    //new TxtImporter().Show();
        //    // TODO load content
        //}
    }
}

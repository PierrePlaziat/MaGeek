using System.Windows;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MtgSqliveSdk;
using MageekSdk.Collection.Entities;
using MaGeek.Framework.Utils;

namespace MaGeek.UI
{

    public partial class DeckList : TemplatedUserControl
    {

        #region Attributes

        public IEnumerable<Deck> Decks { get; private set; }

        #region Filter

        private string filterString = "";
        public string FilterString
        {
            get { return filterString; }
            set { 
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

        public DeckList()
        {
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
            DelayLoad().ConfigureAwait(false);
        }

        private void ConfigureEvents()
        {
            App.Events.UpdateDeckEvent += async () => { await Reload(); };
            App.Events.UpdateDeckListEvent += async () => { await Reload(); };
        }

        private static async Task DelayLoad()
        {
            await Task.Delay(1);
            App.Events.RaiseUpdateDeckList();
        }

        #endregion

        #region Methods

        private async Task Reload()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                Decks = FilterDeckEnumerator(await Mageek.GetDecks());
                OnPropertyChanged(nameof(Decks));
                IsLoading = Visibility.Collapsed;
            });
        }

        //private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var deck = decklistbox.SelectedItem as Deck;
        //    if (deck != null) App.Events.RaiseDeckSelect(deck);
        //}

        private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (decklistbox.SelectedItem is Deck deck) App.Events.RaiseDeckSelect(deck.DeckId);
        }

        private async void AddDeck(object sender, RoutedEventArgs e)
        {
            string title = Log.GetInpurFromUser("What title?","New title");
            await Mageek.CreateDeck_Empty(title,"");
        }
        
        private async void RenameDeck(object sender, RoutedEventArgs e)
        {
            string title = Log.GetInpurFromUser("What title?", "New title");
            await Mageek.RenameDeck(App.State.SelectedDeck.DeckId, title);
        }

        private async void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            await Mageek.DuplicateDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private async void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            var v = decklistbox.SelectedItems;
            List<Deck> v2 = new();
            foreach (var vv in v) v2.Add((Deck)vv);
            await Mageek.DeleteDecks(v2);
        }

        private async void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            var totalPrice = await Mageek.EstimateDeckPrice(App.State.SelectedDeck.DeckId, App.Config.Settings[Setting.Currency]);

            MessageBox.Show("Estimation : " + totalPrice.Item1 + " €"+"\n"+
                            "Missing : " + totalPrice.Item2);
        }

        #endregion

        private async void GetAsTxtList(object sender, RoutedEventArgs e)
        {
            string txt = await Mageek.DeckToTxt(((Deck)decklistbox.SelectedItem).DeckId);
            new TxtImporter(txt).Show();
        }
    }

}

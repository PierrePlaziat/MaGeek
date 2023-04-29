using System.Windows;
using System.Windows.Controls;
using MaGeek.AppData.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MaGeek.AppBusiness;

namespace MaGeek.UI
{

    public partial class DeckList : TemplatedUserControl
    {

        #region Attributes

        public IEnumerable<MagicDeck> Decks { get; private set; }

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

        private IEnumerable<MagicDeck> FilterDeckEnumerator(IEnumerable<MagicDeck> enumerable)
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

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            App.Events.RaiseUpdateDeckList();
        }

        #endregion

        #region Methods

        private async Task Reload()
        {
            IsLoading = Visibility.Visible;
            Decks = FilterDeckEnumerator(await MageekUtils.GetDecks());
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(Decks));
                IsLoading = Visibility.Collapsed;
            });
        }

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.Events.RaiseDeckSelect(deck);
        }

        private async void AddDeck(object sender, RoutedEventArgs e)
        {
            await MageekUtils.AddEmptyDeck();
        }
        
        private async void RenameDeck(object sender, RoutedEventArgs e)
        {
            await MageekUtils.RenameDeck(App.State.SelectedDeck);
        }

        private async void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            await MageekUtils.DuplicateDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private async void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            foreach(MagicDeck d in decklistbox.SelectedItems)
                await MageekUtils.DeleteDeck(d);
        }

        private async void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            float totalPrice = await MageekUtils.EstimateDeckPrice(App.State.SelectedDeck);
            MessageBox.Show("Estimation : " + totalPrice + " €");
        }

        #endregion

    }

}

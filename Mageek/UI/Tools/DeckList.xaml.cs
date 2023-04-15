using System.Windows;
using System.Windows.Controls;
using MaGeek.AppData.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
                AsyncReload();
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

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            App.Events.RaiseUpdateDeckList();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.UpdateDeckEvent += Events_UpdateDeckEvent; ;
            App.Events.UpdateDeckListEvent += Events_UpdateDeckListEvent;
        }

        private void Events_UpdateDeckEvent()
        {
            AsyncReload();
        }

        private void Events_UpdateDeckListEvent()
        {
            AsyncReload();
        }

        #endregion

        #region Async Reload

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(() => { Decks = GetDecks(); });
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(Decks));
                IsLoading = Visibility.Collapsed;
            });
        }

        #endregion

        #region Data Retrieve

        private IEnumerable<MagicDeck> GetDecks()
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                return FilterDeckEnumerator(
                    DB.decks
                    .Include(deck => deck.CardRelations)
                        .ThenInclude(cardrel => cardrel.Card)
                            .ThenInclude(card => card.Card)
                    .Include(deck => deck.CardRelations)
                        .ThenInclude(cardrel => cardrel.Card)
                            .ThenInclude(card => card.DeckRelations)
                    .ToArray()
                ); 
            }
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.Events.RaiseDeckSelect(deck);
        }

        private async void AddDeck(object sender, RoutedEventArgs e)
        {
            await App.Biz.Utils.AddDeck();
        }
        
        private async void RenameDeck(object sender, RoutedEventArgs e)
        {
            await App.Biz.Utils.RenameDeck(App.State.SelectedDeck);
        }

        private async void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            await App.Biz.Utils.DuplicateDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private async void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex == -1) return;
            await App.Biz.Utils.DeleteDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private async void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            float totalPrice = await MageekUtils.EstimateDeckPrice(App.State.SelectedDeck);
            MessageBox.Show("Estimation : " + totalPrice + " €");
        }
    }

}

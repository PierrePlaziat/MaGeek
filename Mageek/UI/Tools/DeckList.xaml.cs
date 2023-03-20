using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using MaGeek.AppData.Entities;
using System;
using System.Threading.Tasks;
using MaGeek.AppBusiness;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace MaGeek.UI
{

    public partial class DeckList : TemplatedUserControl
    {

        #region Attributes

        private MageekDbContext db;
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
            set { isLoading = value; }
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
            db = App.Biz.DB.GetNewContext();
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
            App.Events.RaiseUpdateDeckList();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.UpdateDeckEvent += Events_UpdateDeckEvent; ;
            App.Events.UpdateDeckListEvent += Events_UpdateDeckListEvent; ;
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
            // Show Busy feedback
            Application.Current.Dispatcher.Invoke(new Action(() => {
                IsLoading = Visibility.Visible;
                OnPropertyChanged(nameof(IsLoading));
            }));
            // Async
            await Task.Run(async () =>
            {
                Decks = GetDecks();
                OnPropertyChanged(nameof(Decks));
            });
            // Hide Busy feedback
            Application.Current.Dispatcher.Invoke(new Action(() => {
                IsLoading = Visibility.Collapsed;
                OnPropertyChanged(nameof(IsLoading));
            }));
        }

        #endregion

        #region Data Retrieve

        private IEnumerable<MagicDeck> GetDecks()
        {
            return FilterDeckEnumerator(db.decks);
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.Events.RaiseDeckSelect(deck);
        }

        private void AddDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.AddDeck();
        }
        
        private void RenameDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.RenameDeck(App.State.SelectedDeck);
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.DuplicateDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.DeleteDeck(Decks.ToArray()[decklistbox.SelectedIndex]);
        }

        private void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            float totalPrice = App.Biz.Utils.EstimateDeckPrice(App.State.SelectedDeck);
            MessageBox.Show("Estimation : " + totalPrice + " €");
        }
    }

}

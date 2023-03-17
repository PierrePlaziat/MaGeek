using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using MaGeek.AppData.Entities;
using System;
using System.Threading.Tasks;
using MaGeek.AppBusiness;
using System.Linq;

namespace MaGeek.UI
{

    public partial class DeckList : TemplatedUserControl
    {

        #region Attributes
        
        public ObservableCollection<MagicDeck> Decks {
            get
            {
                return new ObservableCollection<MagicDeck>(
                    db.decks
                    .Where(x => x.Title.ToLower().Contains(FilterString.ToLower()))
                    .OrderBy(x => x.Title)
                );
            }
        }

        private string filterString = "";
        public string FilterString
        {
            get { return filterString; }
            set { 
                filterString = value;
                DoAsyncRefresh().ConfigureAwait(false);
            } 
        }

        #endregion

        #region CTOR

        private MageekDbContext db;
        public DeckList()
        {
            db = App.Biz.DB.GetNewContext();
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.UpdateDeckEvent += () => { DoAsyncRefresh().ConfigureAwait(false); };
            App.Events.UpdateDeckListEvent += () => { DoAsyncRefresh().ConfigureAwait(false); };
        }

        #endregion

        private async Task DoAsyncRefresh()
        {
            // Async
            await Task.Run(async () =>
            {
                OnPropertyChanged(nameof(Decks));
            });
            // Resync
            Application.Current.Dispatcher.Invoke(new Action(() => {
                decklistbox.ItemsSource = null;
                decklistbox.ItemsSource = Decks;
            }));
        }

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
            App.Biz.Utils.DuplicateDeck(Decks[decklistbox.SelectedIndex]);
        }

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.DeleteDeck(Decks[decklistbox.SelectedIndex]);
        }

        private void EstimateDeckPrice(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            float totalPrice = App.Biz.Utils.EstimateDeckPrice(App.State.SelectedDeck);
            MessageBox.Show("Estimation : " + totalPrice + " €");
        }
    }

}

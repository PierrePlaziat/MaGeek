using MaGeek.AppBusiness;
using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckList : TemplatedUserControl
    {

        #region Attributes

        public ObservableCollection<MagicDeck> Decks { get { return new ObservableCollection<MagicDeck>( db.decks.Where(x=>x.Title.ToLower().Contains(FilterString.ToLower())).OrderBy(x=>x.Title)); } }

        private string filterString = "";
        public string FilterString
        {
            get { return filterString; }
            set { 
                filterString = value;
                OnPropertyChanged();
                OnPropertyChanged("Decks");
            } 
        }

        #endregion

        #region CTOR

        MageekDbContext db;

        public DeckList()
        {
            db = App.Biz.DB.GetNewContext();
            DataContext = this;
            InitializeComponent();
            App.Events.UpdateDeckEvent += () => { forceRefresh(); };
            App.Events.UpdateDeckListEvent += () => { forceRefresh(); };
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.Events.RaiseDeckSelect(deck);
        }
        
        internal void forceRefresh()
        {
            decklistbox.ItemsSource = null;
            decklistbox.ItemsSource = Decks;
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

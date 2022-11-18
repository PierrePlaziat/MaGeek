using MaGeek.Data.Entities;
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

        public ObservableCollection<MagicDeck> Decks { get { return new ObservableCollection<MagicDeck>( App.CARDS.AllDecks.Where(x=>x.Title.ToLower().Contains(FilterString.ToLower()))); } }

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

        public DeckList()
        {
            DataContext = this;
            InitializeComponent();
            App.STATE.UpdateDeckEvent += () => { forceRefresh(); };
            App.STATE.UpdateDeckListEvent += () => { forceRefresh(); };
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.STATE.RaiseDeckSelect(deck);
        }
        
        internal void forceRefresh()
        {
            decklistbox.ItemsSource = null;
            decklistbox.ItemsSource = Decks;
        }

        private void AddDeck(object sender, RoutedEventArgs e)
        {
            App.CARDS.Utils.AddDeck();
        }
        
        private void RenameDeck(object sender, RoutedEventArgs e)
        {
            if (App.STATE.SelectedDeck == null) return;
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \""+App.STATE.SelectedDeck.Title+"\"", App.STATE.SelectedDeck.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (App.DB.decks.Where(x => x.Title == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            App.STATE.SelectedDeck.Title = newTitle;
            App.DB.SafeSaveChanges();
            App.STATE.RaiseUpdateDeck();
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (App.STATE.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                var deckToCopy = Decks[decklistbox.SelectedIndex];
                var newDeck = new MagicDeck(deckToCopy);
                App.DB.decks.Add(newDeck);
                App.DB.SafeSaveChanges();
                App.STATE.RaiseUpdateDeckList();
            }
        }

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (App.STATE.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                if (MessageBoxHelper.AskUser("Are you sure to delete this deck?"))
                {
                    var deck = Decks[decklistbox.SelectedIndex];
                    App.DB.decks.Remove(deck);
                    App.DB.SafeSaveChanges();
                    App.STATE.RaiseUpdateDeckList();
                }
            }
        }

    }

}

using MaGeek.Data.Entities;
using MaGeek.Events;
using Plaziat.CommonWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckList : UserControl, INotifyPropertyChanged
    {

        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public ObservableCollection<MagicDeck> Decks { get { return App.cardManager.BinderDeck; } }

        #endregion

        #region CTOR

        public DeckList()
        {
            DataContext = this;
            InitializeComponent();
            App.state.RaiseDeckModif += HandleDeckModified;
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            forceRefresh();
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.state.SelectDeck(deck);
        }
        
        internal void forceRefresh()
        {
            decklistbox.ItemsSource = null;
            decklistbox.ItemsSource = Decks;
        }

        private void AddDeck(object sender, RoutedEventArgs e)
        {
            try
            {
                string deckTitle = MessageBoxHelper.UserInputString("Please enter a title for this new deck");
                if (deckTitle == null) return;
                if (App.database.decks.Where(x => x.Title == deckTitle).Any())
                {
                    MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                    return; 
                }
                MagicDeck deck = new MagicDeck(deckTitle);
                App.database.decks.Add(deck);
                App.database.SaveChanges();
                OnPropertyChanged("Decks");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }
        
        private void RenameDeck(object sender, RoutedEventArgs e)
        {
            if (App.state.SelectedDeck == null) return;
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \""+App.state.SelectedDeck.Title+"\"");
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (App.database.decks.Where(x => x.Title == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            App.state.SelectedDeck.Title = newTitle;
            App.database.SaveChanges();
            forceRefresh();
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (App.state.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                var deckToCopy = Decks[decklistbox.SelectedIndex];
                var newDeck = new MagicDeck(deckToCopy);
                Decks.Add(newDeck);
                App.database.SaveChanges();
            }
        }

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (App.state.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                if (MessageBoxHelper.AskUser("Are you sure to delete this deck?"))
                {
                    var deck = Decks[decklistbox.SelectedIndex];
                    Decks.Remove(deck);
                    App.database.SaveChanges();
                }
            }
        }
    }

}

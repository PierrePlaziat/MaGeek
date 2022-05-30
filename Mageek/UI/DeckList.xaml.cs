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

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Select Deck Event

        public delegate void CustomEventHandler(object sender, SelectDeckEventArgs args);
        public event CustomEventHandler RaiseCustomEvent;

        #endregion

        public ObservableCollection<MagicDeck> Decks { 
            get {
                return App.database.decksBind;
            }
        }

        internal void Refresh()
        {
            decklistbox.ItemsSource = null;
            decklistbox.ItemsSource = Decks;
        }

        public DeckList()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void ListView_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                var deck = Decks[decklistbox.SelectedIndex];
                if (deck != null)
                {
                    OnRaiseDeckEvent(new SelectDeckEventArgs(deck));
                }
            }
        }
        protected virtual void OnRaiseDeckEvent(SelectDeckEventArgs e)
        {
            CustomEventHandler raiseEvent = RaiseCustomEvent;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        private void AddDeck(object sender, RoutedEventArgs e)
        {
            try
            {
                string deckTitle = MessageBoxHelper.UserInputString("Please enter a title for this new deck");
                if (deckTitle == null) return;
                if (App.database.decks.Where(x => x.Name == deckTitle).Any())
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
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \""+App.state.CurrentDeck.Name+"\"");
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (App.database.decks.Where(x => x.Name == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            App.state.CurrentDeck.Name = newTitle;
            App.database.SaveChanges();
            Refresh();
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
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

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
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;

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

        public ObservableCollection<MagicDeck> Decks { get { return new ObservableCollection<MagicDeck>( App.CardManager.DeckListBinder.Where(x=>x.Title.ToLower().Contains(FilterString.ToLower()))); } }

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
            App.State.UpdateDeckEvent += () => { forceRefresh(); };
            App.State.UpdateDeckListEvent += () => { forceRefresh(); };
        }

        #endregion

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as MagicDeck;
            if (deck != null) App.State.RaiseDeckSelect(deck);
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
                string deckTitle = MessageBoxHelper.UserInputString("Please enter a title for this new deck","");
                if (deckTitle == null) return;
                if (App.Database.decks.Where(x => x.Title == deckTitle).Any())
                {
                    MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                    return; 
                }
                MagicDeck deck = new MagicDeck(deckTitle);
                App.Database.decks.Add(deck);
                App.Database.SaveChanges();
                App.State.RaiseUpdateDeckList();
                App.State.RaiseDeckSelect(deck);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }
        
        private void RenameDeck(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \""+App.State.SelectedDeck.Title+"\"", App.State.SelectedDeck.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (App.Database.decks.Where(x => x.Title == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            App.State.SelectedDeck.Title = newTitle;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                var deckToCopy = Decks[decklistbox.SelectedIndex];
                var newDeck = new MagicDeck(deckToCopy);
                App.Database.decks.Add(newDeck);
                App.Database.SaveChanges();
                App.State.RaiseUpdateDeckList();
            }
        }

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (App.State.SelectedDeck == null) return;
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                if (MessageBoxHelper.AskUser("Are you sure to delete this deck?"))
                {
                    var deck = Decks[decklistbox.SelectedIndex];
                    App.Database.decks.Remove(deck);
                    App.Database.SaveChanges();
                    App.State.RaiseUpdateDeckList();
                }
            }
        }
    }

}

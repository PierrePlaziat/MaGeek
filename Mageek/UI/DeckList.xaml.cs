using MaGeek.Data.Entities;
using MaGeek.Events;
using Plaziat.CommonWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private string filter;
        public string Filter { 
            get { return filter; } 
            set { filter = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MagicDeck> Decks { 
            get {
                return App.appContext.decksBind;
            }
        }

        internal void Refresh()
        {
            OnPropertyChanged("Decks");
        }

        public DeckList()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void AddDeck(object sender, RoutedEventArgs e)
        {
            try
            {
                string deckTitle = String.IsNullOrEmpty(Filter) ? "NewDeck" : Filter;
                MagicDeck deck = new MagicDeck(deckTitle);
                App.appContext.decks.Add(deck);
                App.appContext.SaveChanges();
                OnPropertyChanged("Decks"); 
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
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

        private void DeleteDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                if (MessageBoxHelper.AskUser("Are you sure to delete this deck?"))
                {
                    var deck = Decks[decklistbox.SelectedIndex];
                    Decks.Remove(deck);
                    App.appContext.SaveChanges();
                }
            }
        }

        private void DuplicateDeck(object sender, RoutedEventArgs e)
        {
            if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            {
                var deckToCopy = Decks[decklistbox.SelectedIndex];
                var newDeck = new MagicDeck(deckToCopy);
                Decks.Add(newDeck);
                App.appContext.SaveChanges();
            }
        }

    }

}

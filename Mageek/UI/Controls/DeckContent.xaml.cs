using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckContent : UserControl, INotifyPropertyChanged
    {

        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged("CurrentCommanders");
                OnPropertyChanged("CurrentNonCommanders");
                OnPropertyChanged("CurrentSide");
                OnPropertyChanged("Visible");
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentCommanders
        {
            get {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(x=>x.RelationType==1));
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentNonCommanders
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(x => x.RelationType == 0));
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentSide
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(x => x.RelationType == 2));
            }
        }

        public Visibility Visible { get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
        }

        void HandleDeckModif(object sender, DeckModifEventArgs e)
        {
            var v = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = v;
        }

        #region CTOR

        public DeckContent()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModif;
        }


        #endregion

        private void LessCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.cardManager.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

        private void MoreCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.cardManager.AddCardToDeck(c, CurrentDeck);
        }

        private void LVDeck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is CardDeckRelation cardRel) App.state.SelectCard(cardRel.Card.Card);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeck.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 1;
            App.state.ModifDeck();
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVCommandants.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 0;
            App.state.ModifDeck();
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeck.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 2;
            App.state.ModifDeck();
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeckSide.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 0;
            App.state.ModifDeck();
        }
    }

}

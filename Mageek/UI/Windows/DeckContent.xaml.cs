using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;

namespace MaGeek.UI
{

    public partial class DeckContent : UserControl, INotifyPropertyChanged, IXmlSerializable
    {

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
        }

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
                OnPropertyChanged("HasCommander");
                OnPropertyChanged("Visible");
            }
        }

        private string filterString = "";
        public string FilterString {
            get { return filterString; }
            set { 
                filterString = value; 
                OnPropertyChanged();
                OnPropertyChanged("CurrentCommanders");
                OnPropertyChanged("CurrentNonCommanders");
                OnPropertyChanged("CurrentSide");
                OnPropertyChanged("HasCommander");
                OnPropertyChanged("Visible");
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentCommanders
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 1
                    && x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                    && x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower()))
                );
            }
        }

        public Visibility HasCommander
        {
            get
            {
                if (CurrentCommanders==null) return Visibility.Collapsed;
                if (CurrentCommanders.Count<=0) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentNonCommanders
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 0
                    && x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                    && x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower()))
                );
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentSide
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 2
                    && x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                    && x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower()))
                );
            }
        }

        public Visibility Visible { get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
        }

        void HandleDeckModif()
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
            App.State.SelectDeckEvent += HandleDeckSelected;
            App.State.UpdateDeckEvent += HandleDeckModif;
        }


        #endregion

        private void LessCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.CardManager.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

        private void MoreCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.CardManager.AddCardToDeck(c, CurrentDeck,1);
        }

        private void LVDeck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is CardDeckRelation cardRel) App.State.RaiseCardSelected(cardRel.Card.Card);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeck.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 1;
            App.State.RaiseUpdateDeck();
            App.Database.SaveChanges();
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVCommandants.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 0;
            App.State.RaiseUpdateDeck();
            App.Database.SaveChanges();
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeck.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 2;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = LVDeckSide.SelectedItem as CardDeckRelation;
            cardRel.RelationType = 0;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }
    }

}

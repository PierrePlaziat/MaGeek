using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaGeek.UI
{

    public partial class DeckTable : UserControl, INotifyPropertyChanged
    {


        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region TableState

        const int CardSize_Complete = 207;
        const int CardSize_Picture = 130;
        const int CardSize_Header = 25;
        int currentCardSize = 130;
        public int CurrentCardSize
        {
            get { return currentCardSize; }
            set { currentCardSize = value; OnPropertyChanged(); }
        }

        public enum TableOrganisation { Grids, Columns, Lines }
        TableOrganisation currentOrganisation = TableOrganisation.Grids;
        public TableOrganisation CurrentOrganisation
        {
            get { return currentOrganisation; }
            set { currentOrganisation = value; OnPropertyChanged(); }
        }

        public enum TableClassification { Cmc, Type, Tag }
        TableClassification currentClassification = TableClassification.Cmc;
        public TableClassification CurrentClassification
        {
            get { return currentClassification; }
            set { currentClassification = value; OnPropertyChanged(); }
        }

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged("Visible");
                OnPropertyChanged("CardRelations");
                OnPropertyChanged("CardRelations_Lands");
                OnPropertyChanged("CardRelations_Cmc0");
                OnPropertyChanged("CardRelations_Cmc1");
                OnPropertyChanged("CardRelations_Cmc2");
                OnPropertyChanged("CardRelations_Cmc3");
                OnPropertyChanged("CardRelations_Cmc4");
                OnPropertyChanged("CardRelations_Cmc5");
                OnPropertyChanged("CardRelations_Cmc6");
                OnPropertyChanged("CardRelations_Cmc7");
                OnPropertyChanged("HasLands");
                OnPropertyChanged("HasCmc0");
                OnPropertyChanged("HasCmc1");
                OnPropertyChanged("HasCmc2");
                OnPropertyChanged("HasCmc3");
                OnPropertyChanged("HasCmc4");
                OnPropertyChanged("HasCmc5");
                OnPropertyChanged("HasCmc6");
                OnPropertyChanged("HasCmc7");
            }
        }

        #endregion

        #region Accessors

        public Visibility Visible { 
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<CardDeckRelation> CardRelations
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations; }
        }

        public List<CardDeckRelation> CardRelations_Lands
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("land")).ToList(); }
        }
        public Visibility HasLands
        {
            get { return CardRelations_Lands!= null && CardRelations_Lands.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc0
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToLower().Contains("land") && x.Card.Card.Cmc == 0).ToList(); }
        }
        public Visibility HasCmc0
        {
            get { return CardRelations_Cmc0 != null && CardRelations_Cmc0.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc1
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 1).ToList(); }
        }
        public Visibility HasCmc1
        {
            get { return CardRelations_Cmc1 != null && CardRelations_Cmc1.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc2
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 2).ToList(); }
        }
        public Visibility HasCmc2
        {
            get { return CardRelations_Cmc2 != null && CardRelations_Lands != null && CardRelations_Cmc2.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc3
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 3).ToList(); }
        }
        public Visibility HasCmc3
        {
            get { return CardRelations_Cmc3 != null && CardRelations_Cmc3.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc4
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 4).ToList(); }
        }
        public Visibility HasCmc4
        {
            get { return CardRelations_Cmc4 != null && CardRelations_Cmc4.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc5
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 5).ToList(); }
        }
        public Visibility HasCmc5
        {
            get { return CardRelations_Cmc5 != null && CardRelations_Cmc5.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc6
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 6).ToList(); }
        }
        public Visibility HasCmc6
        {
            get { return CardRelations_Cmc6 != null && CardRelations_Cmc6.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<CardDeckRelation> CardRelations_Cmc7
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc >= 7).ToList(); }
        }
        public Visibility HasCmc7
        {
            get { return CardRelations_Cmc7 != null && CardRelations_Cmc7.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #endregion

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModified;
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            FullRefresh();
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
            FullRefresh();
        }

        private void FullRefresh()
        {
            CurrentDeck = App.state.SelectedDeck;
        }

        #endregion

        private void Resize_Complete(object sender, RoutedEventArgs e)
        {
            currentCardSize = CardSize_Complete;
            FullRefresh();
        }

        private void Resize_Picture(object sender, RoutedEventArgs e)
        {
            currentCardSize = CardSize_Picture;
            FullRefresh();
        }

        private void Resize_Header(object sender, RoutedEventArgs e)
        {
            currentCardSize = CardSize_Header;
            FullRefresh();
        }

    }

}

using MaGeek.Data.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckTable : TemplatedUserControl
    {

        #region Attributes

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
                OnPropertyChanged("CardRelations_Commandant");
                OnPropertyChanged("CardRelations_Lands");
                OnPropertyChanged("CardRelations_Lands_B");
                OnPropertyChanged("CardRelations_Lands_W");
                OnPropertyChanged("CardRelations_Lands_U");
                OnPropertyChanged("CardRelations_Lands_G");
                OnPropertyChanged("CardRelations_Lands_R");
                OnPropertyChanged("CardRelations_Lands_S");
                OnPropertyChanged("CardRelations_Cmc0");
                OnPropertyChanged("CardRelations_Cmc1");
                OnPropertyChanged("CardRelations_Cmc2");
                OnPropertyChanged("CardRelations_Cmc3");
                OnPropertyChanged("CardRelations_Cmc4");
                OnPropertyChanged("CardRelations_Cmc5");
                OnPropertyChanged("CardRelations_Cmc6");
                OnPropertyChanged("CardRelations_Cmc7");
                OnPropertyChanged("HasCommandant");
                OnPropertyChanged("HasNonLands");
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

        #endregion

        #region Visibility Binding

        public Visibility Visible { 
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCommandant
        {
            get { 
                if (CardRelations!= null && CardRelations.Count > 0)
                {
                    if(CardRelations.Where(x=>x.RelationType==1).Any()) return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility HasNonLands
        {
            get { return CardRelations != null && CardRelations.Where(x=> !x.Card.Card.Type.ToLower().Contains("land")).Any() ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasLands
        {
            get { return CardRelations_Lands != null && CardRelations_Lands.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc0
        {
            get { return CardRelations_Cmc0 != null && CardRelations_Cmc0.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc1
        {
            get { return CardRelations_Cmc1 != null && CardRelations_Cmc1.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc2
        {
            get { return CardRelations_Cmc2 != null && CardRelations_Lands != null && CardRelations_Cmc2.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc3
        {
            get { return CardRelations_Cmc3 != null && CardRelations_Cmc3.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc4
        {
            get { return CardRelations_Cmc4 != null && CardRelations_Cmc4.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc5
        {
            get { return CardRelations_Cmc5 != null && CardRelations_Cmc5.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc6
        {
            get { return CardRelations_Cmc6 != null && CardRelations_Cmc6.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility HasCmc7
        {
            get { return CardRelations_Cmc7 != null && CardRelations_Cmc7.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region CardRelations Binding

        public ObservableCollection<CardDeckRelation> CardRelations
        {
            get { return CurrentDeck == null ? null : CurrentDeck.CardRelations; }
        }

        public List<CardDeckRelation> CardRelations_Commandant
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.RelationType == 1
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }

        public List<CardDeckRelation> CardRelations_Lands
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_B
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId == "Swamp"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_W
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId == "Plains"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_U
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId == "Island"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_G
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId == "Forest"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_R
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId == "Mountain"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }
        public List<CardDeckRelation> CardRelations_Lands_S
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations_Lands = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                       x.Card.Card.Type.ToLower().Contains("land")
                   && x.Card.Card.CardId != "Swamp"
                   && x.Card.Card.CardId != "Plains"
                   && x.Card.Card.CardId != "Island"
                   && x.Card.Card.CardId != "Forest"
                   && x.Card.Card.CardId != "Mountain"
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations_Lands.Add(card);
                    }
                }
                return cardRelations_Lands;
            }
        }

        public List<CardDeckRelation> CardRelations_Cmc0
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 0
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc1
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 1
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc2
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 2
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc3
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 3
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc4
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 4
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc5
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 5
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc6
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc == 6
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }
        public List<CardDeckRelation> CardRelations_Cmc7
        {
            get
            {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
                List<CardDeckRelation> cardRelations = new List<CardDeckRelation>();
                foreach (var card in CurrentDeck.CardRelations.Where(x =>
                        x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Cmc >= 7
                ))
                {
                    for (int i = 0; i < card.Quantity; i++)
                    {
                        cardRelations.Add(card);
                    }
                }
                return cardRelations;
            }
        }

        #endregion

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.State.SelectDeckEvent += HandleDeckSelected;
            App.State.UpdateDeckEvent += HandleDeckModified;
        }

        void HandleDeckModified()
        {
            FullRefresh();
        }

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
        }

        private void FullRefresh()
        {
            var deck = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = deck;
        }

        #endregion

        #region methods

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

        #endregion

        private void SetCommandant_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as CardDeckRelation;
            cr.RelationType = 1;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        private void UnsetCommandant_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as CardDeckRelation;
            cr.RelationType = 0;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        private void ToSide_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as CardDeckRelation;
            cr.RelationType = 2;
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        private void AddOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.MaGeek.Utils.AddCardToDeck(c, CurrentDeck,1);
        }

        private void RemoveOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.MaGeek.Utils.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

    }

}

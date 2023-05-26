﻿using MaGeek.AppBusiness;
using MaGeek.AppBusiness.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckTable : TemplatedUserControl
    {

        #region Attributes

        const int CardSize_Complete = 207;
        const int CardSize_Picture = 130;
        const int CardSize_Header = 25;

        private Deck currentDeck;
        public Deck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                AsyncReload();
            }
        }

        public List<DeckCard> CardRelations
        {
            get {
                if (CurrentDeck == null || CurrentDeck.CardRelations == null) return new List<DeckCard>();
                else return CurrentDeck.CardRelations.ToList(); 
            }
        }

        public List<DeckCard> CardRelations_Commandant { get; private set; }
        public List<DeckCard> CardRelations_Lands { get; private set; }
        public List<DeckCard> CardRelations_Lands_B { get; private set; }
        public List<DeckCard> CardRelations_Lands_W { get; private set; }
        public List<DeckCard> CardRelations_Lands_U { get; private set; }
        public List<DeckCard> CardRelations_Lands_G { get; private set; }
        public List<DeckCard> CardRelations_Lands_R { get; private set; }
        public List<DeckCard> CardRelations_Lands_S { get; private set; }
        public List<DeckCard> CardRelations_Cmc0 { get; private set; }
        public List<DeckCard> CardRelations_Cmc1 { get; private set; }
        public List<DeckCard> CardRelations_Cmc2 { get; private set; }
        public List<DeckCard> CardRelations_Cmc3 { get; private set; }
        public List<DeckCard> CardRelations_Cmc4 { get; private set; }
        public List<DeckCard> CardRelations_Cmc5 { get; private set; }
        public List<DeckCard> CardRelations_Cmc6 { get; private set; }
        public List<DeckCard> CardRelations_Cmc7 { get; private set; }

        public Visibility HasCommandant { get; private set; }
        public Visibility HasNonLands { get; private set; }
        public Visibility HasLands { get; private set; }
        public Visibility HasCmc0 { get; private set; }
        public Visibility HasCmc1 { get; private set; }
        public Visibility HasCmc2 { get; private set; }
        public Visibility HasCmc3 { get; private set; }
        public Visibility HasCmc4 { get; private set; }
        public Visibility HasCmc5 { get; private set; }
        public Visibility HasCmc6 { get; private set; }
        public Visibility HasCmc7 { get; private set; }
        

        #region TableState

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

        #endregion

        #region Visibilitie

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive
        {
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region Visibility Binding

        #endregion

        #endregion

        #region Data Retrieve

        private List<DeckCard> GetCardRelations_Lands()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
            foreach (var card in CurrentDeck.CardRelations.Where(x =>
                    x.Card != null && x.Card.Card.Type.ToLower().Contains("land")
            ))
            {
                for (int i = 0; i < card.Quantity; i++)
                {
                    cardRelations_Lands.Add(card);
                }
            }
            return cardRelations_Lands;
        }
        private List<DeckCard> GetCardRelations_Lands_B()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Lands_W()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Lands_U()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Lands_G()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Lands_R()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Lands_S()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations_Lands = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc0()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc1()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc2()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc3()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc4()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc5()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc6()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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
        private List<DeckCard> GetCardRelations_Cmc7()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            List<DeckCard> cardRelations = new List<DeckCard>();
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

        private Visibility GetHasCommandant()
        {
            if (CardRelations != null && CardRelations.Count > 0)
            {
                if (CardRelations.Where(x => x.RelationType == 1).Any()) return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        private Visibility GetHasNonLands() {
            return CardRelations != null && CardRelations
                .Where(x => !x.Card.Card.Type.ToLower().Contains("land"))
                .Any() ? Visibility.Visible : Visibility.Collapsed; 
        }
        private Visibility GetHasLands() {
            return CardRelations_Lands != null && CardRelations_Lands.Count > 0 ? Visibility.Visible : Visibility.Collapsed; 
        }
        private Visibility GetHasCmc0() { 
            return CardRelations_Cmc0 != null && CardRelations_Cmc0.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc1() { 
            return CardRelations_Cmc1 != null && CardRelations_Cmc1.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc2() { 
            return CardRelations_Cmc2 != null && CardRelations_Lands != null && CardRelations_Cmc2.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc3() { 
            return CardRelations_Cmc3 != null && CardRelations_Cmc3.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc4() {
            return CardRelations_Cmc4 != null && CardRelations_Cmc4.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc5() {
            return CardRelations_Cmc5 != null && CardRelations_Cmc5.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc6() { 
            return CardRelations_Cmc6 != null && CardRelations_Cmc6.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetHasCmc7() { 
            return CardRelations_Cmc7 != null && CardRelations_Cmc7.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }


        #endregion

        #region CTOR

        public DeckTable()
                {
                    DataContext = this;
                    InitializeComponent();
                    ConfigureEvents();
                }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.SelectDeckEvent += HandleDeckSelected;
            App.Events.UpdateDeckEvent += HandleDeckModified;
        }

        void HandleDeckSelected(Deck deck)
        {
            CurrentDeck = deck;
        }

        void HandleDeckModified()
        {
            Deck tmp = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = tmp;
        }

        #endregion

        private void FullRefresh()
        {
            var deck = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = deck;
            AsyncReload();
        }

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                CardRelations_Commandant = (await MageekStats.GetCommanders(CurrentDeck)).ToList();
                CardRelations_Lands = GetCardRelations_Lands();
                CardRelations_Lands_B = GetCardRelations_Lands_B();
                CardRelations_Lands_W = GetCardRelations_Lands_W();
                CardRelations_Lands_U = GetCardRelations_Lands_U();
                CardRelations_Lands_G = GetCardRelations_Lands_G();
                CardRelations_Lands_R = GetCardRelations_Lands_R();
                CardRelations_Lands_S = GetCardRelations_Lands_S();
                CardRelations_Lands_S = GetCardRelations_Lands_S();
                CardRelations_Cmc0 = GetCardRelations_Cmc0();
                CardRelations_Cmc1 = GetCardRelations_Cmc1();
                CardRelations_Cmc2 = GetCardRelations_Cmc2();
                CardRelations_Cmc3 = GetCardRelations_Cmc3();
                CardRelations_Cmc4 = GetCardRelations_Cmc4();
                CardRelations_Cmc5 = GetCardRelations_Cmc5();
                CardRelations_Cmc6 = GetCardRelations_Cmc6();
                CardRelations_Cmc7 = GetCardRelations_Cmc7();
                HasCommandant = GetHasCommandant();
                HasNonLands = GetHasNonLands();
                HasLands = GetHasLands();
                HasCmc0 = GetHasCmc0();
                HasCmc1 = GetHasCmc1();
                HasCmc2 = GetHasCmc2();
                HasCmc3 = GetHasCmc3();
                HasCmc4 = GetHasCmc4();
                HasCmc5 = GetHasCmc5();
                HasCmc6 = GetHasCmc6();
                HasCmc7 = GetHasCmc7();
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(CardRelations));
                OnPropertyChanged(nameof(CardRelations_Commandant));
                OnPropertyChanged(nameof(CardRelations_Lands));
                OnPropertyChanged(nameof(CardRelations_Lands_B));
                OnPropertyChanged(nameof(CardRelations_Lands_W));
                OnPropertyChanged(nameof(CardRelations_Lands_U));
                OnPropertyChanged(nameof(CardRelations_Lands_G));
                OnPropertyChanged(nameof(CardRelations_Lands_R));
                OnPropertyChanged(nameof(CardRelations_Lands_S));
                OnPropertyChanged(nameof(CardRelations_Cmc0));
                OnPropertyChanged(nameof(CardRelations_Cmc1));
                OnPropertyChanged(nameof(CardRelations_Cmc2));
                OnPropertyChanged(nameof(CardRelations_Cmc3));
                OnPropertyChanged(nameof(CardRelations_Cmc4));
                OnPropertyChanged(nameof(CardRelations_Cmc5));
                OnPropertyChanged(nameof(CardRelations_Cmc6));
                OnPropertyChanged(nameof(CardRelations_Cmc7));
                OnPropertyChanged(nameof(HasCommandant));
                OnPropertyChanged(nameof(HasNonLands));
                OnPropertyChanged(nameof(HasLands));
                OnPropertyChanged(nameof(HasCmc0));
                OnPropertyChanged(nameof(HasCmc1));
                OnPropertyChanged(nameof(HasCmc2));
                OnPropertyChanged(nameof(HasCmc3));
                OnPropertyChanged(nameof(HasCmc4));
                OnPropertyChanged(nameof(HasCmc5));
                OnPropertyChanged(nameof(HasCmc6));
                OnPropertyChanged(nameof(HasCmc7));
                IsLoading = Visibility.Collapsed;
            });
        }

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

        private async void SetCommandant_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await MageekCollection.ChangeCardDeckRelation(cr, 1);
        }

        private async void UnsetCommandant_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await MageekCollection.ChangeCardDeckRelation(cr, 0);
        }

        private async void ToSide_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await MageekCollection.ChangeCardDeckRelation(cr, 2);
        }

        private async void AddOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            var c = cr.Card;
            await MageekCollection.AddCardToDeck(c, CurrentDeck,1);
        }

        private async void RemoveOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            var c = cr.Card;
            await MageekCollection.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

    }

}

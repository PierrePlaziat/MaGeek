using MageekSdk.Collection.Entities;
using MtgSqliveSdk;
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

        public List<DeckCard> CardRelations_Commandant { get; private set; }
        public List<DeckCard> CardRelations_Content { get; private set; }
        public List<DeckCard> CardRelations_Side{ get; private set; }
        public List<DeckCard> CardRelations_Lands { get; private set; }
        public Visibility HasCommandant { get; private set; }
        //public Visibility HasContent{ get; private set; }
        public Visibility HasSide { get; private set; }
        public Visibility HasLands { get; private set; }
        

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

        void HandleDeckSelected(int deck)
        {
            CurrentDeck = Mageek.GetDeck(deck).Result;
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
            OnPropertyChanged(nameof(IsActive));
            CardRelations_Content = await Mageek.GetDeckContent_Related(CurrentDeck.DeckId,0);
            CardRelations_Commandant = await Mageek.GetDeckContent_Related(CurrentDeck.DeckId,1);
            CardRelations_Side = await Mageek.GetDeckContent_Related(CurrentDeck.DeckId,2);
            CardRelations_Lands = await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Land");
            OnPropertyChanged(nameof(CardRelations_Content));
            OnPropertyChanged(nameof(CardRelations_Commandant));
            OnPropertyChanged(nameof(CardRelations_Side));
            OnPropertyChanged(nameof(CardRelations_Lands));
            HasCommandant = CardRelations_Commandant.Count>0 ? Visibility.Visible : Visibility.Hidden;
            HasSide = CardRelations_Side.Count>0 ? Visibility.Visible : Visibility.Hidden;
            //HasContent= CardRelations_Content.Count>0 ? Visibility.Visible : Visibility.Hidden;
            HasLands = CardRelations_Lands.Count>0 ? Visibility.Visible : Visibility.Hidden;
            OnPropertyChanged(nameof(HasCommandant));
            OnPropertyChanged(nameof(HasSide));
            OnPropertyChanged(nameof(HasContent));
            OnPropertyChanged(nameof(HasLands));
            IsLoading = Visibility.Collapsed;
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
            await Mageek.ChangeDeckRelationType(cr, 1);
        }

        private async void UnsetCommandant_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await Mageek.ChangeDeckRelationType(cr, 0);
        }

        private async void ToSide_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await Mageek.ChangeDeckRelationType(cr, 2);
        }

        private async void AddOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await Mageek.AddCardToDeck(cr.CardUuid, CurrentDeck,1);
        }

        private async void RemoveOne_Click(object sender, RoutedEventArgs e)
        {
            var b = (MenuItem)sender;
            var cr = b.DataContext as DeckCard;
            await Mageek.RemoveCardFromDeck(cr.CardUuid, CurrentDeck);
        }

    }

}

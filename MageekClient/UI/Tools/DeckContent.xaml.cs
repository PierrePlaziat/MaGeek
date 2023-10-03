using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekSdk.Collection.Entities;
using MtgSqliveSdk;

namespace MaGeek.UI
{

    public partial class DeckContent : TemplatedUserControl
    {

        #region Attributes

        private Deck currentDeck = null;
        public Deck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                HardReloadAsync().ConfigureAwait(false);
            }
        }

        private List<DeckCard> deckCards = new List<DeckCard>(); 
        public List<DeckCard> DeckCards 
        {
            get { return ApplyFilter(deckCards); }
            set { deckCards = value; }
        }

        public IEnumerable<DeckCard> CurrentCommanders      { get { return DeckCards.Where(card => card.RelationType == 1); } }
        public IEnumerable<DeckCard> CurrentCreatures       { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Creature")); } }
        public IEnumerable<DeckCard> CurrentInstants        { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Instant")); } }
        public IEnumerable<DeckCard> CurrentSorceries       { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Sorcery")); } }
        public IEnumerable<DeckCard> CurrentEnchantments    { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Enchantment")); } }
        public IEnumerable<DeckCard> CurrentArtifacts       { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Artifact")); } }
        public IEnumerable<DeckCard> CurrentPlaneswalkers   { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Planeswalker")); } }
        public IEnumerable<DeckCard> CurrentLands           { get { return DeckCards.Where(card => card.RelationType == 0 && card.Card.Type.Contains("Land")); } }
        public IEnumerable<DeckCard> CurrentSide            { get { return DeckCards.Where(card => card.RelationType == 2); } }

        #region Filter

        private string filterString = string.Empty;
        public string FilterString
        {
            get { return filterString; }
            set
            {
                filterString = value;
                OnPropertyChanged();
                SoftReloadAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Visibilities

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

        public Visibility HasCommander
        {
            get
            {
                if (CurrentCommanders == null) return Visibility.Collapsed;
                if (CurrentCommanders.ToList().Count <= 0) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }
        
        public Visibility HasSide
        {
            get
            {
                if (CurrentCommanders == null) return Visibility.Collapsed;
                if (CurrentSide.ToList().Count <= 0) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }

        #endregion

        #endregion

        #region CTOR

        public DeckContent()
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
            App.Events.UpdateDeckEvent += HandleDeckModif;
        }

        void HandleDeckSelected(string deck)
        {
            CurrentDeck = Mageek.GetDeck(deck).Result;
        }

        void HandleDeckModif()
        {
            Deck tmp = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = tmp;
        }

        #endregion
        
        #region Reload
        
        private async Task HardReloadAsync()
        {
            IsLoading = Visibility.Visible;
            OnPropertyChanged(nameof(IsActive));
            await Task.Delay(100);
            DeckCards = await Mageek.GetDeckContent(CurrentDeck.DeckId);
            OnPropertyChanged(nameof(CurrentCommanders));
            OnPropertyChanged(nameof(CurrentSide));
            OnPropertyChanged(nameof(CurrentCreatures));
            OnPropertyChanged(nameof(CurrentInstants));
            OnPropertyChanged(nameof(CurrentSorceries));
            OnPropertyChanged(nameof(CurrentEnchantments));
            OnPropertyChanged(nameof(CurrentArtifacts));
            OnPropertyChanged(nameof(CurrentPlaneswalkers));
            OnPropertyChanged(nameof(CurrentLands));
            OnPropertyChanged(nameof(HasCommander));
            OnPropertyChanged(nameof(HasSide));
            IsLoading = Visibility.Collapsed;
        }

        private async Task SoftReloadAsync()
        {
            IsLoading = Visibility.Visible;
            await Task.Delay(100);
            OnPropertyChanged(nameof(FilterString));
            OnPropertyChanged(nameof(CurrentCommanders));
            OnPropertyChanged(nameof(CurrentSide));
            OnPropertyChanged(nameof(CurrentCreatures));
            OnPropertyChanged(nameof(CurrentInstants));
            OnPropertyChanged(nameof(CurrentSorceries));
            OnPropertyChanged(nameof(CurrentEnchantments));
            OnPropertyChanged(nameof(CurrentArtifacts));
            OnPropertyChanged(nameof(CurrentPlaneswalkers));
            OnPropertyChanged(nameof(CurrentLands));
            OnPropertyChanged(nameof(HasCommander));
            OnPropertyChanged(nameof(HasSide));
            IsLoading = Visibility.Collapsed;
        }
        
        #endregion

        #region Methods

        private List<DeckCard> ApplyFilter(List<DeckCard> cards)
        {
            return cards.Where(
                    card => card.Card.Name.ToLower().Contains(FilterString.ToLower())
                         || card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())
            ).ToList();
        }

        private void LessCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            Mageek.RemoveCardFromDeck(cr.CardUuid, CurrentDeck).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            Mageek.AddCardToDeck(cr.CardUuid, CurrentDeck,1).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 1).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel,0).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 2).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 0).ConfigureAwait(true);
            App.Events.RaiseUpdateDeck();
        }

        #region UI LINK

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            if (sendedBy.SelectedItem is DeckCard cardRel) App.Events.RaiseCardSelected(cardRel.CardUuid);
            sendedBy.UnselectAll();
        }

        private static ListView GetListView(object sender)
        {
            MenuItem menuItem = sender as MenuItem;
            ContextMenu parentContextMenu = menuItem.CommandParameter as ContextMenu;
            return parentContextMenu.PlacementTarget as ListView;
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewerName.ScrollToVerticalOffset(ScrollViewerName.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion

        #endregion

    }

}

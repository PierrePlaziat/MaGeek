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
                OnPropertyChanged(nameof(IsActive));
                Reload();
            }
        }

        private string filterString = string.Empty;
        public string FilterString {
            get { return filterString; }
            set { 
                filterString = value;
                OnPropertyChanged(nameof(FilterString));
                Reload();
            }
        }

        public IEnumerable<DeckCard> CurrentCommanders      { get; private set; }
        public IEnumerable<DeckCard> CurrentCreatures       { get; private set; }
        public IEnumerable<DeckCard> CurrentInstants        { get; private set; }
        public IEnumerable<DeckCard> CurrentSorceries       { get; private set; }
        public IEnumerable<DeckCard> CurrentEnchantments    { get; private set; }
        public IEnumerable<DeckCard> CurrentArtifacts       { get; private set; }
        public IEnumerable<DeckCard> CurrentPlaneswalkers   { get; private set; }
        public IEnumerable<DeckCard> CurrentLands           { get; private set; }
        public IEnumerable<DeckCard> CurrentSide            { get; private set; }

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

        private void Reload()
        {
            ReloadAsync().ConfigureAwait(false);
        }

        private async Task ReloadAsync()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                CurrentCommanders =     await ApplyFilter(await Mageek.GetDeckContent_Related(CurrentDeck.DeckId,1));
                CurrentSide =           await ApplyFilter(await Mageek.GetDeckContent_Related(CurrentDeck.DeckId,2));
                CurrentCreatures =      await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Creature"));
                CurrentInstants =       await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Instant"));
                CurrentSorceries =      await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Sorcery"));
                CurrentEnchantments =   await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Enchantment"));
                CurrentArtifacts =      await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Artifact"));
                CurrentPlaneswalkers =   await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Planeswalker"));
                CurrentLands =          await ApplyFilter(await Mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Land"));
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
            });
        }

        #endregion

        #region Methods

        private async Task<IEnumerable<DeckCard>> ApplyFilter(IEnumerable<DeckCard> enumerable)
        {
            /*IEnumerable<DeckCard> rels = new List<DeckCard>();
            await Task.Run(() => {
                rels = enumerable.Where(x =>
                    x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                 || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower()));
            });*/
            return enumerable;
        }

        private void LessCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            Mageek.RemoveCardFromDeck(cr.CardUuid, CurrentDeck).ConfigureAwait(true);
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            Mageek.AddCardToDeck(cr.CardUuid, CurrentDeck,1).ConfigureAwait(true);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 1).ConfigureAwait(true);
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel,0).ConfigureAwait(true);
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 2).ConfigureAwait(true);
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            Mageek.ChangeDeckRelationType(cardRel, 0).ConfigureAwait(true);
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

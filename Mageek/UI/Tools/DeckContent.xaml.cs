using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using MaGeek.UI.Windows.Importers;
using System.Threading.Tasks;
using MaGeek.AppBusiness;
using MaGeek.Entities;

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
        public IEnumerable<DeckCard> CurrentNonBasicLands   { get; private set; }
        public IEnumerable<DeckCard> CurrentBasicLands      { get; private set; }
        public IEnumerable<DeckCard> CurrentOthers          { get; private set; }
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

        void HandleDeckSelected(Deck deck)
        {
            CurrentDeck = deck;
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
                CurrentCommanders =     await ApplyFilter(await MageekStats.GetCommanders(CurrentDeck));
                CurrentCreatures =      await ApplyFilter(await MageekStats.GetCreatures(CurrentDeck));
                CurrentInstants =       await ApplyFilter(await MageekStats.GetInstants(CurrentDeck));
                CurrentSorceries =      await ApplyFilter(await MageekStats.GetSorceries(CurrentDeck));
                CurrentEnchantments =   await ApplyFilter(await MageekStats.GetEnchantments(CurrentDeck));
                CurrentArtifacts =      await ApplyFilter(await MageekStats.GetCurrentArtifacts(CurrentDeck));
                CurrentNonBasicLands =  await ApplyFilter(await MageekStats.GetCurrentNonBasicLands(CurrentDeck));
                CurrentOthers =         await ApplyFilter(await MageekStats.GetCurrentOthers(CurrentDeck));
                CurrentBasicLands =     await ApplyFilter(await MageekStats.GetCurrentBasicLands(CurrentDeck));
                CurrentSide =           await ApplyFilter(await MageekStats.GetCurrentSide(CurrentDeck));
                OnPropertyChanged(nameof(CurrentCommanders));
                OnPropertyChanged(nameof(CurrentCreatures));
                OnPropertyChanged(nameof(CurrentInstants));
                OnPropertyChanged(nameof(CurrentSorceries));
                OnPropertyChanged(nameof(CurrentEnchantments));
                OnPropertyChanged(nameof(CurrentArtifacts));
                OnPropertyChanged(nameof(CurrentOthers));
                OnPropertyChanged(nameof(CurrentNonBasicLands));
                OnPropertyChanged(nameof(CurrentBasicLands));
                OnPropertyChanged(nameof(CurrentSide));
                OnPropertyChanged(nameof(HasCommander));
                IsLoading = Visibility.Collapsed;
            });
        }

        #endregion

        #region Methods

        private async Task<IEnumerable<DeckCard>> ApplyFilter(IEnumerable<DeckCard> enumerable)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            await Task.Run(() => {
                rels = enumerable.Where(x =>
                    x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                 || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower()));
            });
            return rels;
        }

        private void LessCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            var c = cr.Card;
            MageekCollection.RemoveCardFromDeck(c.Card, CurrentDeck).ConfigureAwait(true);
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as DeckCard;
            var c = cr.Card;
            MageekCollection.AddCardToDeck(c, CurrentDeck,1).ConfigureAwait(true);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            MageekCollection.ChangeCardDeckRelation(cardRel, 1).ConfigureAwait(true);
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            MageekCollection.ChangeCardDeckRelation(cardRel,0).ConfigureAwait(true);
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            MageekCollection.ChangeCardDeckRelation(cardRel, 2).ConfigureAwait(true);
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            DeckCard cardRel = GetListView(sender).SelectedItem as DeckCard;
            MageekCollection.ChangeCardDeckRelation(cardRel, 0).ConfigureAwait(true);
        }

        #region UI LINK

        private async void CreateDeck(object sender, RoutedEventArgs e)
        {
            await MageekCollection.AddEmptyDeck();
        }

        private void OpenDeckImport(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            if (sendedBy.SelectedItem is DeckCard cardRel) App.Events.RaiseCardSelected(cardRel.Card.Card);
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

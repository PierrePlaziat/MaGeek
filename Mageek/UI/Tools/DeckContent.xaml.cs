using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using MaGeek.UI.Windows.Importers;
using MaGeek.AppData.Entities;
using System.Threading.Tasks;
using MaGeek.AppBusiness;

namespace MaGeek.UI
{

    public partial class DeckContent : TemplatedUserControl
    {

        #region Attributes

        private MagicDeck currentDeck = null;
        public MagicDeck CurrentDeck
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

        public IEnumerable<CardDeckRelation> CurrentCommanders      { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentCreatures       { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentInstants        { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentSorceries       { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentEnchantments    { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentArtifacts       { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentNonBasicLands   { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentBasicLands      { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentOthers          { get; private set; }
        public IEnumerable<CardDeckRelation> CurrentSide            { get; private set; }

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

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
        }

        void HandleDeckModif()
        {
            MagicDeck tmp = CurrentDeck;
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
            CurrentCommanders =     await ApplyFilter(await MageekUtils.GetCommanders(CurrentDeck));
            CurrentCreatures =      await ApplyFilter(await MageekUtils.GetCreatures(CurrentDeck));
            CurrentInstants =       await ApplyFilter(await MageekUtils.GetInstants(CurrentDeck));
            CurrentSorceries =      await ApplyFilter(await MageekUtils.GetSorceries(CurrentDeck));
            CurrentEnchantments =   await ApplyFilter(await MageekUtils.GetEnchantments(CurrentDeck));
            CurrentArtifacts =      await ApplyFilter(await MageekUtils.GetCurrentArtifacts(CurrentDeck));
            CurrentNonBasicLands =  await ApplyFilter(await MageekUtils.GetCurrentNonBasicLands(CurrentDeck));
            CurrentOthers =         await ApplyFilter(await MageekUtils.GetCurrentOthers(CurrentDeck));
            CurrentBasicLands =     await ApplyFilter(await MageekUtils.GetCurrentBasicLands(CurrentDeck));
            CurrentSide =           await ApplyFilter(await MageekUtils.GetCurrentSide(CurrentDeck));
            await RaiseChanges();
            await Task.Run(() => { IsLoading = Visibility.Collapsed; });
        }

        private async Task RaiseChanges()
        {
            await Task.Run(() =>
            {
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
            });
        }

        #endregion

        #region Methods

        private async Task<IEnumerable<CardDeckRelation>> ApplyFilter(IEnumerable<CardDeckRelation> enumerable)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
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
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            MageekUtils.RemoveCardFromDeck(c.Card, CurrentDeck).ConfigureAwait(true);
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            MageekUtils.AddCardToDeck(c, CurrentDeck,1).ConfigureAwait(true);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            MageekUtils.ChangeCardDeckRelation(cardRel, 1).ConfigureAwait(true);
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            MageekUtils.ChangeCardDeckRelation(cardRel,0).ConfigureAwait(true);
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            MageekUtils.ChangeCardDeckRelation(cardRel, 2).ConfigureAwait(true);
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            MageekUtils.ChangeCardDeckRelation(cardRel, 0).ConfigureAwait(true);
        }

        #region UI LINK

        private async void CreateDeck(object sender, RoutedEventArgs e)
        {
            await MageekUtils.AddEmptyDeck();
        }

        private void OpenDeckImport(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            if (sendedBy.SelectedItem is CardDeckRelation cardRel) App.Events.RaiseCardSelected(cardRel.Card.Card);
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

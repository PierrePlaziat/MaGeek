using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using MaGeek.UI.Windows.Importers;
using MaGeek.AppData.Entities;
using System.Threading.Tasks;

namespace MaGeek.UI
{

    public partial class DeckContent : TemplatedUserControl
    {

        #region Attributes

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsActive));
                AsyncReload();
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

        #region Filter

        private string filterString = "";
        public string FilterString {
            get { return filterString; }
            set { 
                filterString = value;
                OnPropertyChanged(nameof(FilterString));
                AsyncReload();
            }
        }

        private IEnumerable<CardDeckRelation> FilterCardEnumerator(IEnumerable<CardDeckRelation> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable.Where(x =>
                    x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                 || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())
            );
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
        
        #region Async Reload

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(() => { CurrentCommanders = GetCurrentCommander(); });
            await Task.Run(() => { CurrentCreatures = GetCurrentCreatures(); });
            await Task.Run(() => { CurrentInstants = GetCurrentInstants(); });
            await Task.Run(() => { CurrentSorceries = GetCurrentSorceries(); });
            await Task.Run(() => { CurrentEnchantments = GetCurrentEnchantments(); });
            await Task.Run(() => { CurrentArtifacts = GetCurrentArtifacts(); });
            await Task.Run(() => { CurrentNonBasicLands = GetCurrentNonBasicLands(); });
            await Task.Run(() => { CurrentOthers = GetCurrentOthers(); });
            await Task.Run(() => { CurrentBasicLands = GetCurrentBasicLands(); });
            await Task.Run(() => { CurrentSide = GetCurrentSide(); });
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
            await Task.Run(() =>
            {
                IsLoading = Visibility.Collapsed;
            });
        }

        #endregion

        #region Data Retrieve

        private IEnumerable<CardDeckRelation> GetCurrentCommander()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(
                App.Biz.Utils.GetCommanders(CurrentDeck)
            );
        }
        private IEnumerable<CardDeckRelation> GetCurrentCreatures()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(App.Biz.Utils.GetCreatures(CurrentDeck));
        }
        private IEnumerable<CardDeckRelation> GetCurrentInstants()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(App.Biz.Utils.GetInstants(CurrentDeck));
        }
        private IEnumerable<CardDeckRelation> GetCurrentSorceries()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(App.Biz.Utils.GetSorceries(CurrentDeck));
        }
        private IEnumerable<CardDeckRelation> GetCurrentEnchantments()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(App.Biz.Utils.GetEnchantments(CurrentDeck));
        }
        private IEnumerable<CardDeckRelation> GetCurrentArtifacts()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("artifact")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }
        private IEnumerable<CardDeckRelation> GetCurrentNonBasicLands()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && !x.Card.Card.Type.ToLower().Contains("basic")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }
        private IEnumerable<CardDeckRelation> GetCurrentBasicLands()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return  CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && x.Card.Card.Type.ToLower().Contains("basic")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }
        private IEnumerable<CardDeckRelation> GetCurrentOthers()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && !x.Card.Card.Type.ToLower().Contains("artifact")
                && !x.Card.Card.Type.ToLower().Contains("creature")
                && !x.Card.Card.Type.ToLower().Contains("instant")
                && !x.Card.Card.Type.ToLower().Contains("sorcery")
                && !x.Card.Card.Type.ToLower().Contains("enchantment")
                && !x.Card.Card.Type.ToLower().Contains("land")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }
        private IEnumerable<CardDeckRelation> GetCurrentSide()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return CurrentDeck.CardRelations.Where(
                x => x.RelationType == 2
                && x.Card != null
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }

        #endregion

        #region Methods

        private void LessCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.Biz.Utils.RemoveCardFromDeck(c.Card, CurrentDeck).ConfigureAwait(true);
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.Biz.Utils.AddCardToDeck(c, CurrentDeck,1).ConfigureAwait(true);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 1).ConfigureAwait(true);
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel,0).ConfigureAwait(true);
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 2).ConfigureAwait(true);
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 0).ConfigureAwait(true);
        }

        #region UI LINK

        private async void CreateDeck(object sender, RoutedEventArgs e)
        {
            await App.Biz.Utils.AddDeck();
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
        }

        private ListView GetListView(object sender)
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

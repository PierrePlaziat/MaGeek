﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using MaGeek.UI.Windows.Importers;
using MaGeek.AppData.Entities;
using System;
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
            set { isLoading = value; }
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
            // Show Busy feedback
            Application.Current.Dispatcher.Invoke(new Action(() => {
                IsLoading = Visibility.Visible;
                OnPropertyChanged(nameof(IsLoading));
            }));
            // Async
            await Task.Run(async () =>
            {
                CurrentCommanders = GetCurrentCommander();
                CurrentCreatures = GetCurrentCreatures();
                CurrentInstants = GetCurrentInstants();
                CurrentSorceries = GetCurrentSorceries();
                CurrentEnchantments = GetCurrentEnchantments();
                CurrentArtifacts = GetCurrentArtifacts();
                CurrentNonBasicLands = GetCurrentNonBasicLands();
                CurrentOthers = GetCurrentOthers();
                CurrentBasicLands = GetCurrentBasicLands();
                CurrentSide = GetCurrentSide();
            }).ConfigureAwait(true);
            // Hide Busy feedback
            Application.Current.Dispatcher.Invoke(new Action(() => {
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
                OnPropertyChanged(nameof(IsLoading));
            }));
        }

        #endregion

        #region Data Retrieve

        private IEnumerable<CardDeckRelation> GetCurrentCommander()
        {
            if (CurrentDeck == null) return null;
            return FilterCardEnumerator(App.Biz.Utils.GetCommanders(CurrentDeck));
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
                && x.Card.Card.Type.ToLower().Contains("land")
                && !x.Card.Card.Type.ToLower().Contains("basic")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
        }
        private IEnumerable<CardDeckRelation> GetCurrentBasicLands()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return new ObservableCollection<CardDeckRelation>(CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card.Card.Type.ToLower().Contains("land")
                && x.Card.Card.Type.ToLower().Contains("basic")
                && (x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName)
            );
        }
        private IEnumerable<CardDeckRelation> GetCurrentOthers()
        {
            if (CurrentDeck == null || CurrentDeck.CardRelations == null) return null;
            return CurrentDeck.CardRelations.Where(
                x => x.RelationType == 0
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
            App.Biz.Utils.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

        private void MoreCard(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.Biz.Utils.AddCardToDeck(c, CurrentDeck,1);
        }

        private void SetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 1);
        }

        private void UnsetCommandant(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel,0);
        }

        private void ToSide(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 2);
        }

        private void ToDeck(object sender, RoutedEventArgs e)
        {
            CardDeckRelation cardRel = GetListView(sender).SelectedItem as CardDeckRelation;
            App.Biz.Utils.ChangeCardDeckRelation(cardRel, 0);
        }

        #region UI LINK

        private void CreateDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.AddDeck();
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
            //sendedBy.UnselectAll(); // TODO (implies some refactor)
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

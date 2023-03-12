using MaGeek.Data.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using MaGeek.UI.Windows.Importers;

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
                OnPropertyChanged("CurrentCommanders");
                OnPropertyChanged("CurrentCreatures");
                OnPropertyChanged("CurrentInstants");
                OnPropertyChanged("CurrentSorceries");
                OnPropertyChanged("CurrentEnchantments");
                OnPropertyChanged("CurrentArtifacts");
                OnPropertyChanged("CurrentOthers");
                OnPropertyChanged("CurrentNonBasicLands");
                OnPropertyChanged("CurrentBasicLands");
                OnPropertyChanged("CurrentSide");
                OnPropertyChanged("HasCommander");
                OnPropertyChanged("Visible");
            }
        }

        private string filterString = "";
        public string FilterString {
            get { return filterString; }
            set { 
                filterString = value; 
                OnPropertyChanged();
                OnPropertyChanged("CurrentCommanders");
                OnPropertyChanged("CurrentCreatures");
                OnPropertyChanged("CurrentInstants");
                OnPropertyChanged("CurrentSorceries");
                OnPropertyChanged("CurrentEnchantments");
                OnPropertyChanged("CurrentArtifacts");
                OnPropertyChanged("CurrentOthers");
                OnPropertyChanged("CurrentNonBasicLands");
                OnPropertyChanged("CurrentBasicLands");
                OnPropertyChanged("CurrentSide");
                OnPropertyChanged("HasCommander");
                OnPropertyChanged("Visible");
            }
        }

        public List<CardDeckRelation> CurrentCommanders 
        { 
            get {
                if (currentDeck == null) return null;
                return FilterCardEnumerator(
                    App.Biz.Utils.GetCommanders(currentDeck)
                ).ToList();
            }
        }
        public List<CardDeckRelation> CurrentCreatures
        {
            get
            {
                if (currentDeck == null) return null;
                return FilterCardEnumerator(
                    App.Biz.Utils.GetCreatures(currentDeck)
                ).ToList();
            }
        }
        public List<CardDeckRelation> CurrentInstants
        {
            get
            {
                if (currentDeck == null) return null;
                return FilterCardEnumerator(
                    App.Biz.Utils.GetInstants(currentDeck)
                ).ToList();
            }
        }
        public List<CardDeckRelation> CurrentSorceries
        {
            get
            {
                if (currentDeck == null) return null;
                return FilterCardEnumerator(
                    App.Biz.Utils.GetSorceries(currentDeck)
                ).ToList();
            }
        }
        public List<CardDeckRelation> CurrentEnchantments
        {
            get
            {
                if (currentDeck == null) return null;
                return FilterCardEnumerator(
                    App.Biz.Utils.GetEnchantments(currentDeck)
                ).ToList();
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentArtifacts
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 0
                    && x.Card.Card.Type.ToLower().Contains("artifact")
                    && ( x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                    .OrderBy(x => x.Card.Card.Cmc.Value)
                    .ThenBy(x => x.Card.Card.CardForeignName)
                );
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentNonBasicLands
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 0
                    && x.Card.Card.Type.ToLower().Contains("land")
                    && ! x.Card.Card.Type.ToLower().Contains("basic")
                    && ( x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                    .OrderBy(x => x.Card.Card.Cmc.Value)
                    .ThenBy(x => x.Card.Card.CardForeignName)
                );
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentBasicLands
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 0
                    && x.Card.Card.Type.ToLower().Contains("land")
                    && x.Card.Card.Type.ToLower().Contains("basic")
                    && ( x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                    .OrderBy(x => x.Card.Card.Cmc.Value)
                    .ThenBy(x => x.Card.Card.CardForeignName)
                );
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentOthers
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 0
                    && !x.Card.Card.Type.ToLower().Contains("artifact")
                    && !x.Card.Card.Type.ToLower().Contains("creature")
                    && !x.Card.Card.Type.ToLower().Contains("instant")
                    && !x.Card.Card.Type.ToLower().Contains("sorcery")
                    && !x.Card.Card.Type.ToLower().Contains("enchantment")
                    && !x.Card.Card.Type.ToLower().Contains("land")
                    && ( x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                    .OrderBy(x => x.Card.Card.Cmc.Value)
                    .ThenBy(x => x.Card.Card.CardForeignName)
                );
            }
        }

        public ObservableCollection<CardDeckRelation> CurrentSide
        {
            get
            {
                if (currentDeck == null || currentDeck.CardRelations == null) return null;
                return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                    x => x.RelationType == 2
                    && ( x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower()) || x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())))
                    .OrderBy(x => x.Card.Card.Cmc.Value)
                    .ThenBy(x => x.Card.Card.CardForeignName)
                );
            }
        }

        public Visibility Visible 
        {
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility HasCommander
        {
            get
            {
                if (CurrentCommanders == null) return Visibility.Collapsed;
                if (CurrentCommanders.Count <= 0) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }

        #endregion

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
        }

        void HandleDeckModif()
        {
            var v = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = v;
        }

        #region CTOR

        public DeckContent()
        {
            InitializeComponent();
            DataContext = this;
            App.Events.SelectDeckEvent += HandleDeckSelected;
            App.Events.UpdateDeckEvent += HandleDeckModif;
        }


        #endregion

        private IEnumerable<CardDeckRelation> FilterCardEnumerator(IEnumerable<CardDeckRelation> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable.Where(x =>
                    x.Card.Card.CardId.ToLower().Contains(FilterString.ToLower())
                && x.Card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())
            );
        }

        private void LessCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.Biz.Utils.RemoveCardFromDeck(c.Card, CurrentDeck);
        }

        private void MoreCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.Biz.Utils.AddCardToDeck(c, CurrentDeck,1);
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

        private void CreateDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.AddDeck();
        }

        private void OpenDeckImport(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }
    }

}

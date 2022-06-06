using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{

    public partial class DeckTable : UserControl, INotifyPropertyChanged
    {


        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck { 
            get { return currentDeck; }
            set { 
                currentDeck = value;
                OnPropertyChanged();
            }
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
            RefreshUGrid();
        }

        #endregion

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModified;
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            forceRefresh();
        }

        #endregion

        internal void forceRefresh()
        {
            CurrentDeck = null;
            CurrentDeck = App.state.SelectedDeck;
            RefreshUGrid();
        }

        private void LessCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.cardManager.RemoveCardFromDeck(c, CurrentDeck);
        }

        private void MoreCard(object sender, System.Windows.RoutedEventArgs e)
        {
            var b = (Button)sender;
            var cr = b.DataContext as CardDeckRelation;
            var c = cr.Card;
            App.cardManager.AddCardToDeck(c, CurrentDeck);
        }

        private void RefreshUGrid()
        {
            UGrid.Children.Clear();
            foreach(var cardrel in CurrentDeck.CardRelations)
            {
                for(int i=0;i<cardrel.Quantity;i++)
                {
                    Uri Url = new Uri(cardrel.Card.variants[0].ImageUrl, UriKind.Absolute);
                    Image img = new Image()
                    {
                        Source = new BitmapImage(Url),
                    };
                    UGrid.Children.Add(img);
                }
            }
        }

    }

}

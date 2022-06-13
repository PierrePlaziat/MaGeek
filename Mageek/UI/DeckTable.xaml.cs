using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
                OnPropertyChanged("Visible");
            }
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
            RefreshUGrid();
        }

        public Visibility Visible { get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        readonly BackgroundWorker LoadImgWorker = new();
        bool isLoading = false;
        public Visibility Loading { get { return isLoading ? Visibility.Visible : Visibility.Collapsed; } }

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModified;
            LoadImgWorker.DoWork += LoadImg;
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            RefreshUGrid();
        }

        #endregion

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
            CurrentDeck = null;
            CurrentDeck = App.state.SelectedDeck;
            if (CurrentDeck == null) return;
            isLoading = true;
            OnPropertyChanged("Loading");
            LoadImgWorker.RunWorkerAsync();
        }

        private void LoadImg(object sender, DoWorkEventArgs e)
        {
           
            try
            {
                this.Dispatcher.Invoke(
                    DispatcherPriority.Send, new Action(
                        delegate {
                            UGrid.Children.Clear();
                        }
                    )
                );
                foreach (var cardrel in CurrentDeck.CardRelations)
                {
                    for (int i = 0; i < cardrel.Quantity; i++)
                    {

                        Thread.Sleep(100);

                        this.Dispatcher.Invoke (
                            DispatcherPriority.Send, new Action (
                                delegate {
                                    UGrid.Children.Add(new CardIllustration(cardrel.Card) { Width=250});
                                }
                            )
                        );

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            isLoading = false;
            OnPropertyChanged("Loading");
        }

        private void LVDeck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LVDeck.SelectedItem is CardDeckRelation cardRel) App.state.SelectCard(cardRel.Card);
        }

    }

}

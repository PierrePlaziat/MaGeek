﻿using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            FullRefresh();
        }

        public Visibility Visible { get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        #region Async Image Load

        private BackgroundWorker Worker;
        private void ConstructWorker()
        {
            Worker = new BackgroundWorker();
            Worker.DoWork += Working;
            Worker.WorkerSupportsCancellation = true;


            void Working(object sender, DoWorkEventArgs e)
            {
                try
                {
                    int cardIndex = 0;
                    this.Dispatcher.Invoke(
                        DispatcherPriority.Send, new Action(
                            delegate
                            {
                                UGrid.Children.Clear();
                            }
                        )
                    );
                    Thread.Sleep(30);

                    while (!Worker.CancellationPending && cardIndex < CurrentDeck.CardRelations.Count)
                    {
                        var cardrel = CurrentDeck.CardRelations[cardIndex];
                        this.Dispatcher.Invoke(
                            DispatcherPriority.Send, new Action(
                                delegate
                                {
                                    for (int i = 0; i < cardrel.Quantity; i++)
                                    {
                                        CardIllustration cardIllu = new CardIllustration(cardrel.Card) { Width = 150, Height = 207, BorderBrush = Brushes.Transparent, BorderThickness = new Thickness(1) };
                                        UGrid.Children.Add(cardIllu);
                                    }
                                }
                            )
                        );
                        cardIndex++;
                        Thread.Sleep(30);
                    }
                    e.Cancel = true;
                    return;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    e.Cancel = true;
                    return;
                }
            }
        }


        #endregion

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModified;
            ConstructWorker();
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            FullRefresh();
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

        private void FullRefresh()
        {
            Worker.CancelAsync();
            CurrentDeck = null;
            CurrentDeck = App.state.SelectedDeck;
            if (CurrentDeck == null) return;
            ConstructWorker();
            Worker.RunWorkerAsync();
            
        }

    }

}

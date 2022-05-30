﻿using MaGeek.Data.Entities;
using MaGeek.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaGeek.UI
{

    public partial class Collection : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        private bool isSearching = false;
        public bool IsSearching { get { return isSearching; } set { isSearching = value; OnPropertyChanged(); OnPropertyChanged("IsNotSearching"); } }
        public bool IsNotSearching { get { return !isSearching; } }
        private int loadingProgress = 0;
        public int LoadingProgress { get { return loadingProgress; } set { loadingProgress = value; OnPropertyChanged(); } }

        public ObservableCollection<MagicCard> CardsBind { get { return App.database.cardsBind; } }

        #endregion

        #region CTOR

        public Collection()
        { 
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region UI calls

        private void LaunchSearch(object sender, System.Windows.RoutedEventArgs e)
        {
            DoSearch();
        }

        private void CurrentSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoSearch();
            }
        }

        private void CardList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MagicCard card = CardList.SelectedItem as MagicCard;
            if (card != null)
            {
                OnRaiseCustomEvent(new SelectCardEventArgs(card));
            }
        }

        #endregion

        private async void DoSearch()
        {
            IsSearching = true;
            await App.cardManager.SearchCardsOnline(CurrentSearch.Text);
            IsSearching = false;
        }

        #region Event

        public delegate void CustomEventHandler(object sender, SelectCardEventArgs args);
        public event CustomEventHandler RaiseSelectCard;
        protected virtual void OnRaiseCustomEvent(SelectCardEventArgs e)
        {
            CustomEventHandler raiseEvent = RaiseSelectCard;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        #endregion

    }

}

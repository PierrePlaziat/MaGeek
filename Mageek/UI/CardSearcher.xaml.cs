using MaGeek.Data.Entities;
using MaGeek.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace MaGeek.UI
{

    public partial class CardSearcher : UserControl, INotifyPropertyChanged
    {

        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public bool IsSearching { get { return isSearching; } set { isSearching = value; OnPropertyChanged(); OnPropertyChanged("IsNotSearching"); } }
        public bool IsNotSearching { get { return !isSearching; } } // TODO xaml converter
        private bool isSearching = false;

        public ObservableCollection<MagicCard> CardsBind { 
            get { return App.cardManager.BinderCards; } 
        }

        #endregion

        #region CTOR

        public CardSearcher()
        { 
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region UI Link

        private void CardGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var card = CardGrid.SelectedItem as MagicCard;
            if (card != null) App.state.SelectCard(card);
        }

        private void SearchButton_Pressed(object sender, System.Windows.RoutedEventArgs e)
        {
            DoSearch();
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DoSearch();
        }

        private async void DoSearch()
        {
            IsSearching = true;
            await App.cardManager.MtgApi.SearchCardsOnline(CurrentSearch.Text);

            IsSearching = false;
        }

        #endregion

    }

}

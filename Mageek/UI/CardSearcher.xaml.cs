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
            get {
                var unfiltered = App.cardManager.BinderCards;
                var filtered = unfiltered
                    .Where(x => x.Cmc >= FilterMinCmc)
                    .Where(x => x.Cmc <= FilterMaxCmc)
                    .Where(x => x.Name_VO.ToLower().Contains(FilterName.ToLower()))
                    .Where(x => x.Type.ToLower().Contains(FilterType.ToLower()));
                    //.Where(x => x.Text.Contains(FilterText))
                return new ObservableCollection<MagicCard>(filtered); 
            }
        }

        #region Filter

        private string filterName = "";
        public string FilterName
        {
            get { return filterName; }
            set { 
                filterName = value;
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private string filterType = "";
        public string FilterType
        {
            get { return filterType; }
            set { filterType = value; OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private string filterText = "";
        public string FilterText
        {
            get { return filterText; }
            set { filterText = value; OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private int filterMinCmc = 0;
        public int FilterMinCmc
        {
            get { return filterMinCmc; }
            set { filterMinCmc = value; OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private int filterMaxCmc = 20;
        public int FilterMaxCmc
        {
            get { return filterMaxCmc; }
            set { filterMaxCmc = value; OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private string filterColors; //TODO
        public string FilterColors
        {
            get { return filterColors; }
            set { filterColors = value; OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        #endregion

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
            if (string.IsNullOrEmpty(CurrentSearch.Text)) return;
            IsSearching = true;
            FilterName = CurrentSearch.Text;
            FilterType = "";
            FilterText = "";
            FilterMinCmc = 0;
            FilterMaxCmc = 20;
            await App.cardManager.MtgApi.SearchCardsOnline(CurrentSearch.Text);
            IsSearching = false;
        }

        #endregion

    }

}

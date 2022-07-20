using MaGeek.Data.Entities;
using System.Collections.Generic;
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
        private bool isSearching = false;

        public ObservableCollection<MagicCard> CardsBind { 
            get {
                var unfiltered = App.cardManager.BinderCards;
                var filtered = unfiltered
                    .Where(x => x.Cmc >= FilterMinCmc)
                    .Where(x => x.Cmc <= FilterMaxCmc)
                    .Where(x => x.CardId.ToLower().Contains(FilterName.ToLower()) || x.CardForeignName.ToLower().Contains(FilterName.ToLower()))
                    .Where(x => x.Type.ToLower().Contains(FilterType.ToLower()));
                if (!filterColorB) filtered = filtered.Where(x => !x.ManaCost.Contains('B'));
                if (!filterColorW) filtered = filtered.Where(x => !x.ManaCost.Contains('W'));
                if (!filterColorU) filtered = filtered.Where(x => !x.ManaCost.Contains('U'));
                if (!filterColorG) filtered = filtered.Where(x => !x.ManaCost.Contains('G'));
                if (!filterColorR) filtered = filtered.Where(x => !x.ManaCost.Contains('R'));

                if (!string.IsNullOrEmpty(TagFilterSelected))
                {
                    var tagged = new List<MagicCard>();
                    foreach (var card in filtered)
                    {
                        if(App.database.Tags.Where(x=>x.CardId==card.CardId && x.Tag== TagFilterSelected).Any())
                        {
                            tagged.Add(card);
                        }
                    }
                    return new ObservableCollection<MagicCard>(tagged);
                }
                return new ObservableCollection<MagicCard>(filtered); 
            }
        }

        public List<string> AvailableTags
        {
            get
            {
                return App.database.AvailableTags();
            }
        }

        #region Filter

        private string tagFilterSelected = "";
        public string TagFilterSelected
        {
            get { return tagFilterSelected; }
            set
            {
                tagFilterSelected = value;
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private string filterName = "";
        public string FilterName
        {
            get { return filterName; }
            set
            {
                filterName = value;
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private string filterType = "";
        public string FilterType
        {
            get { return filterType; }
            set { filterType = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private int filterMinCmc = 0;
        public int FilterMinCmc
        {
            get { return filterMinCmc; }
            set { filterMinCmc = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private int filterMaxCmc = 20;
        public int FilterMaxCmc
        {
            get { return filterMaxCmc; }
            set { filterMaxCmc = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorB = true;
        public bool FilterColorB
        {
            get { return filterColorB; }
            set
            {
                filterColorB = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorW = true;
        public bool FilterColorW
        {
            get { return filterColorW; }
            set
            {
                filterColorW = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorU = true;
        public bool FilterColorU
        {
            get { return filterColorU; }
            set
            {
                filterColorU = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorG = true;
        public bool FilterColorG
        {
            get { return filterColorG; }
            set
            {
                filterColorG = value; 
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorR = true;
        public bool FilterColorR
        {
            get { return filterColorR; }
            set
            {
                filterColorR = value;
                OnPropertyChanged();
                OnPropertyChanged("CardsBind");
            }
        }

        private bool filterColorI = true;
        public bool FilterColorI
        {
            get { return filterColorI; }
            set
            {
                filterColorI = value;
                OnPropertyChanged();
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
            if (CardGrid.SelectedItem is MagicCard card) App.state.SelectCard(card);
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
            await App.cardManager.MtgApi.SearchCardsOnline(CurrentSearch.Text);
            ResetFilters();
            FilterName = CurrentSearch.Text;
            CurrentSearch.Text = "";
            IsSearching = false;
        }

        private void ResetFilters()
        {
            FilterName = "";
            FilterType = "";
            FilterMinCmc = 0;
            FilterMaxCmc = 20;
            FilterColorB = true;
            FilterColorW = true;
            FilterColorU = true;
            FilterColorG = true;
            FilterColorR = true;
        }

        #endregion

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (MagicCard c in CardGrid.SelectedItems)
            {
                App.cardManager.AddCardToDeck(c.Variants[0], App.state.SelectedDeck);
            }
        }

        private void FilterTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("CardsBind");
        }

        private void FilterTag_DropDownOpened(object sender, System.EventArgs e)
        {
            OnPropertyChanged("AvailableTags");
        }
    }

}

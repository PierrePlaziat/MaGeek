using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System;
using MageekSdk.MtgSqlive.Entities;
using MageekSdk.Collection.Entities;
using MtgSqliveSdk;

namespace MaGeek.UI
{

    public partial class CardSearcher : TemplatedUserControl
    {

        #region Attributes

        public List<Cards> CardList { get; private set; }

        private List<Tag> availableTags; 
        public List<Tag> AvailableTags
        {
            get { return availableTags; }
            private set { availableTags = value; OnPropertyChanged(); }
        }

        #region Filters

        private string filterName = "";
        public string FilterName
        {
            get { return filterName; }
            set { filterName = value; OnPropertyChanged(); }
        }

        private string filterType = "";
        public string FilterType
        {
            get { return filterType; }
            set { filterType = value; OnPropertyChanged(); }
        }
        
        private string filterKeyword = "";
        public string FilterKeyword
        {
            get { return filterKeyword; }
            set { filterKeyword = value; OnPropertyChanged(); }
        }
        
        private string filterText = "";
        public string FilterText
        {
            get { return filterText; }
            set { filterText = value; OnPropertyChanged(); }
        }

        private MtgColorFilter filterColor = MtgColorFilter._;
        public MtgColorFilter FilterColor
        {
            get { return filterColor; }
            set { filterColor = value; OnPropertyChanged(); }
        }

        private Tag filterTag = null;
        public Tag FilterTag
        {
            get { return filterTag; }
            set { filterTag = value; OnPropertyChanged(); }
        }

        private bool onlyGot = false;
        public bool OnlyGot
        {
            get { return onlyGot; }
            set { onlyGot = value; OnPropertyChanged(); }
        }

        #endregion

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        private Visibility showAdvanced = Visibility.Collapsed;
        public Visibility ShowAdvanced
        {
            get { return showAdvanced; }
            set { showAdvanced = value; OnPropertyChanged(); }
        }
        
        private Visibility showNormal = Visibility.Visible;
        public Visibility ShowNormal
        {
            get { return showNormal; }
            set { showNormal = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #region CTOR

        public CardSearcher()
        {
            DataContext = this;
            InitializeComponent();
            FillColorFilterCombo();
            App.Events.UpdateCardCollecEvent += async () => { await ReloadData(); };
        }

        #endregion

        #region DATA LOAD

        private async Task ReloadData()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                if (ShowAdvanced == Visibility.Collapsed)
                {
                    CardList = await Mageek.NormalSearch(
                        App.Config.Settings[Setting.ForeignLanguage],
                        FilterName
                    );
                }
                else
                {
                    CardList = await Mageek.AdvancedSearch(
                        App.Config.Settings[Setting.ForeignLanguage],
                        FilterName,
                        FilterType,
                        FilterKeyword,
                        FilterText,
                        FilterColor.ToString(),
                        FilterTag.TagContent, OnlyGot
                    );
                }
                OnPropertyChanged(nameof(CardList));
                await Task.Delay(50);
            });
            IsLoading = Visibility.Collapsed;
        }

        #endregion

        #region UI Link

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            ReloadData().ConfigureAwait(false);
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            ShowAdvanced = Visibility.Collapsed;
            ShowNormal = Visibility.Visible;
            FilterName = "";
            FilterType = "";
            FilterKeyword = "";
            FilterText = "";
            FilterColor = MtgColorFilter._;
            FilterTag = null;
            OnlyGot = false;
        }

        private void FilterName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                ReloadData().ConfigureAwait(false);
            }
        }

        private void Button_AdvancedSearcher(object sender, RoutedEventArgs e)
        {
            if (ShowAdvanced == Visibility.Collapsed)
            {
                ShowAdvanced = Visibility.Visible;
                ShowNormal = Visibility.Collapsed;
            }
            else if (ShowAdvanced == Visibility.Visible)
            {
                ShowAdvanced = Visibility.Collapsed;
                ShowNormal = Visibility.Visible;
            }
        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CardGrid.SelectedItem is Cards card) App.Events.RaiseCardSelected(card.Uuid);
        }

        private void AddToDeck(object sender, RoutedEventArgs e)
        {
            foreach (Cards c in CardGrid.SelectedItems)
            {
                Mageek.AddCardToDeck(c.Uuid, App.State.SelectedDeck,1)
                    .ConfigureAwait(true);
            }
        }

        private void FilterTag_DropDownOpened(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableTags));
        }

        private void FillColorFilterCombo()
        {
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        }

        #endregion

    }

    public enum MtgColorFilter
    {
        _,//NoFilter,
        X,//Colorless
        W,//White
        B,//Black
        G,//Green
        U,//Blue
        R,//Red
        GW,//SELESNYA
        WU,//AZORIUS
        BU,//DIMIR
        RB,//RAKDOS
        GR,//GRUUL
        GU,//SIMIC
        WB,//ORZHOV
        RU,//IZZET
        GB,//GOLGARI
        RW,//BOROS
        GBW,//ABZAN
        GWU,//BANT
        WRU,//JESKAI
        GRW,//NAYA
        WUB,//ESPER
        GUR,//TEMUR
        GRB,//JUND
        RUB,//GRIXIS
        BGU,//SULTAI
        RWB,//MARDU
        BGUR,//noWhite
        GURW,//noBlack
        URWB,//noGreen
        RWBG,//noBlue
        WBGU,//noRed
        WBGUR,//AllColors
    }

}

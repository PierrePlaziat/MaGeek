using MaGeek.AppData.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MaGeek.AppBusiness;
using MaGeek.AppFramework;
using MaGeek.AppFramework.Utils;
using System;

namespace MaGeek.UI
{

    public enum MtgColorFilter
    {
        None,
        Colorless,
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
        NoW,//noWhite
        NoB,//noBlack
        NoG,//noGreen
        NoU,//noBlue
        NoR,//noRed
        WBGUR,//AllColors
    }

    public partial class CardSearcher : TemplatedUserControl
    {

        #region Attributes

        public List<CardModel> CardList { get; private set; }
        public List<CardTag> AvailableTags { get { return MageekStats.GetTagsDistinct().Result; } }

        #region Filters

        private void ResetFilters()
        {
            FilterName = "";
            FilterType = "";
            FilterKeyword = "";
            FilterText = "";
            ColorSelected = "";
            TagFilterSelected = "";
            OnlyGot = false;
        }

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

        private string colorSelected = "";
        public string ColorSelected
        {
            get { return colorSelected; }
            set { colorSelected = value; OnPropertyChanged(); }
        }

        private string tagFilterSelected = "";
        public string TagFilterSelected
        {
            get { return tagFilterSelected; }
            set { tagFilterSelected = value; OnPropertyChanged(); }
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

        #endregion

        #endregion

        #region CTOR

        public CardSearcher()
        {
            DataContext = this;
            InitializeComponent();
            App.Events.UpdateCardCollecEvent += async () => { await ReloadData(); };
            //DelayLoad().ConfigureAwait(false);
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            App.Events.RaiseUpdateCardCollec();
        }

        #endregion

        #region Async data reload

        private void Button_SearchLocal(object sender, RoutedEventArgs e)
        {
            ReloadData().ConfigureAwait(false);
        }

        //private async void Button_SearchOnline(object sender, RoutedEventArgs e)
        //{
        //    IsLoading = Visibility.Visible;
        //    var cardlist = await MageekApi.RetrieveCard(FilterName, false, true,true);
        //    await MageekApi.RecordCards(cardlist);
        //    await ReloadData();
        //}

        private async Task ReloadData()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                CardList = await LoadCards();
                OnPropertyChanged(nameof(CardList));
                await Task.Delay(50);
            });
            IsLoading = Visibility.Collapsed;
        }

        private async Task<List<CardModel>> LoadCards()
        {
            if (ShowAdvanced == Visibility.Collapsed) 
                return await NormalSearch();
            else
                return await AdvancedSearch();
        }

        private async Task<List<CardModel>> NormalSearch()
        {
            List<CardModel> retour = new List<CardModel>();
            string lang = App.Config.Settings[Setting.ForeignLanguage];
            string lowerFilterName = FilterName.ToLower();
            string normalizedFilterName = StringExtension.RemoveDiacritics(FilterName).ToLower();
            if (!string.IsNullOrEmpty(FilterName))
            {

                using (var DB = App.DB.GetNewContext())
                {

                    // Search in VO
                    retour.AddRange(await DB.CardModels.Where(x => x.CardId.ToLower().Contains(lowerFilterName))
                                                       .ToArrayAsync());

                    // Search in foreign
                    var trads = DB.CardTraductions.Include(x => x.Card)
                                                  .Where(x => x.Language == lang && x.Normalized.Contains(normalizedFilterName));
                    foreach (var trad in trads)
                    {
                        retour.AddRange(await DB.CardModels.Where(x => x.CardId.Contains(trad.CardId))
                                                           .ToArrayAsync()
                        );
                    }

                }
                // Remove duplicata
                retour = retour.GroupBy(x => x.CardId).Select(g => g.First()).ToList();
            }
            else
            {
                using (var DB = App.DB.GetNewContext())
                {
                    retour.AddRange(await DB.CardModels.ToArrayAsync());
                }
            }
            return retour;
        }

        private async Task<List<CardModel>> AdvancedSearch()
        {
            string lang = App.Config.Settings[Setting.ForeignLanguage];
            List<CardModel> retour = new List<CardModel>();
            string lowerFilterName = FilterName.ToLower();
            string normalizedFilterName = StringExtension.RemoveDiacritics(FilterName).ToLower();

            if (!string.IsNullOrEmpty(FilterType))
            {
                retour = retour.Where(x => x.Type.ToLower().Contains(FilterType.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(FilterKeyword))
            {
                retour = retour.Where(x => x.KeyWords.ToLower().Contains(FilterKeyword.ToLower())).ToList();
            }

            //if (!string.IsNullOrEmpty(TagFilterSelected))
            //{
            //    var tagged = new List<CardModel>();
            //    foreach (var card in retour)
            //    {
            //        if (await MageekStats.DoesCardHasTag(card.CardId, TagFilterSelected))
            //        {
            //            tagged.Add(card);
            //        }
            //    }
            //    return new List<CardModel>(tagged);
            //}

            return retour;
        }

        #endregion


        #region UI Link

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            ShowAdvanced = Visibility.Collapsed;
            ResetFilters();
            ReloadData().ConfigureAwait(false); 
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
            if (ShowAdvanced == Visibility.Collapsed) ShowAdvanced = Visibility.Visible;
            else if (ShowAdvanced == Visibility.Visible) ShowAdvanced = Visibility.Collapsed;
        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CardGrid.SelectedItem is CardModel card) App.Events.RaiseCardSelected(card);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (CardModel c in CardGrid.SelectedItems)
            {
                MageekCollection.AddCardToDeck(c, App.State.SelectedDeck,1)
                    .ConfigureAwait(true);
            }
        }

        private void OnlyGot_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FilterTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CardList));
        }

        private void FilterTag_DropDownOpened(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableTags));
        }

        #endregion

    }

}

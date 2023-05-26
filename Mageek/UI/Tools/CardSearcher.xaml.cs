using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MaGeek.AppBusiness;
using System;
using MaGeek.Entities;
using MaGeek.Framework.Extensions;

namespace MaGeek.UI
{

    public partial class CardSearcher : TemplatedUserControl
    {

        #region Attributes

        public List<CardModel> CardList { get; private set; }

        private List<CardTag> availableTags; 
        public List<CardTag> AvailableTags
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

        private CardTag filterTag = null;
        public CardTag FilterTag
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
                if (ShowAdvanced == Visibility.Collapsed) CardList = await NormalSearch();
                else                                      CardList = await AdvancedSearch();
                OnPropertyChanged(nameof(CardList));
                await Task.Delay(50);
            });
            IsLoading = Visibility.Collapsed;
        }

        private async Task<List<CardModel>> NormalSearch()
        {
            List<CardModel> retour = new List<CardModel>();
            string lang = App.Config.Settings[Setting.ForeignLanguage];
            string lowerFilterName = FilterName.ToLower();
            string normalizedFilterName = StringExtension.RemoveDiacritics(FilterName).Replace('-', ' ').ToLower();
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
            AvailableTags = await MageekStats.GetTagsDistinct();
            List <CardModel> retour = new();
            using (var DB = App.DB.GetNewContext())
            {
                if (OnlyGot)
                {
                    retour.AddRange(await DB.CardModels.Where(x => x.Got > 0).ToArrayAsync());
                } 
                else
                {
                    retour.AddRange(await DB.CardModels.ToArrayAsync());
                }
            }
           
            if (!string.IsNullOrEmpty(FilterType))
            {
                string type = FilterType.ToLower();
                retour = retour.Where(x => x.Type.ToLower().Contains(type)).ToList();
            }

            if (!string.IsNullOrEmpty(FilterKeyword))
            {
                string keyword = FilterKeyword.ToLower();
                retour = retour.Where(x => x.KeyWords.ToLower().Contains(keyword)).ToList();
            }

            if (!string.IsNullOrEmpty(FilterText))
            {
                string text = FilterText.ToLower();
                retour = retour.Where(x => x.Text.ToLower().Contains(FilterKeyword.ToLower())).ToList();
            }

            if (FilterColor != MtgColorFilter._)
            {
                switch(FilterColor)
                {
                    case MtgColorFilter.X : retour = retour.Where(x => string.IsNullOrEmpty(x.ColorIdentity)).ToList(); break;
                    case MtgColorFilter.W :         retour = retour.Where(x => x.ColorIdentity == "W").ToList();                break;
                    case MtgColorFilter.B :         retour = retour.Where(x => x.ColorIdentity == "B").ToList();                break;
                    case MtgColorFilter.U :         retour = retour.Where(x => x.ColorIdentity == "U").ToList();                break;
                    case MtgColorFilter.G :         retour = retour.Where(x => x.ColorIdentity == "G").ToList();                break;
                    case MtgColorFilter.R :         retour = retour.Where(x => x.ColorIdentity == "R").ToList();                break;
                    case MtgColorFilter.GW:         retour = retour.Where(x => x.ColorIdentity == "G,W").ToList();              break;
                    case MtgColorFilter.WU:         retour = retour.Where(x => x.ColorIdentity == "U,W").ToList();              break;
                    case MtgColorFilter.BU:         retour = retour.Where(x => x.ColorIdentity == "B,U").ToList();              break;
                    case MtgColorFilter.RB:         retour = retour.Where(x => x.ColorIdentity == "B,R").ToList();              break;
                    case MtgColorFilter.GR:         retour = retour.Where(x => x.ColorIdentity == "G,R").ToList();              break;
                    case MtgColorFilter.GU:         retour = retour.Where(x => x.ColorIdentity == "G,U").ToList();              break;
                    case MtgColorFilter.WB:         retour = retour.Where(x => x.ColorIdentity == "B,W").ToList();              break;
                    case MtgColorFilter.RU:         retour = retour.Where(x => x.ColorIdentity == "R,U").ToList();              break;
                    case MtgColorFilter.GB:         retour = retour.Where(x => x.ColorIdentity == "B,G").ToList();              break;
                    case MtgColorFilter.RW:         retour = retour.Where(x => x.ColorIdentity == "R,W").ToList();              break;
                    case MtgColorFilter.GBW:        retour = retour.Where(x => x.ColorIdentity == "B,G,W").ToList();            break;
                    case MtgColorFilter.GWU:        retour = retour.Where(x => x.ColorIdentity == "G,U,W").ToList();            break;
                    case MtgColorFilter.WRU:        retour = retour.Where(x => x.ColorIdentity == "R,U,W").ToList();            break;
                    case MtgColorFilter.GRW:        retour = retour.Where(x => x.ColorIdentity == "G,R,W").ToList();            break;
                    case MtgColorFilter.WUB:        retour = retour.Where(x => x.ColorIdentity == "B,U,W").ToList();            break;
                    case MtgColorFilter.GUR:        retour = retour.Where(x => x.ColorIdentity == "G,R,U").ToList();            break;
                    case MtgColorFilter.GRB:        retour = retour.Where(x => x.ColorIdentity == "B,G,R").ToList();            break;
                    case MtgColorFilter.RUB:        retour = retour.Where(x => x.ColorIdentity == "B,R,U").ToList();            break;
                    case MtgColorFilter.BGU:        retour = retour.Where(x => x.ColorIdentity == "B,G,U").ToList();            break;
                    case MtgColorFilter.RWB:        retour = retour.Where(x => x.ColorIdentity == "B,R,W").ToList();            break;
                    case MtgColorFilter.BGUR:       retour = retour.Where(x => x.ColorIdentity == "B,G,R,U").ToList();          break;
                    case MtgColorFilter.GURW:       retour = retour.Where(x => x.ColorIdentity == "G,R,U,W").ToList();          break;
                    case MtgColorFilter.URWB:       retour = retour.Where(x => x.ColorIdentity == "B,R,U,W").ToList();          break;
                    case MtgColorFilter.RWBG:       retour = retour.Where(x => x.ColorIdentity == "B,G,R,W").ToList();          break;
                    case MtgColorFilter.WBGU:       retour = retour.Where(x => x.ColorIdentity == "B,G,U,W").ToList();          break;
                    case MtgColorFilter.WBGUR:      retour = retour.Where(x => x.ColorIdentity == "B,G,R,U,W").ToList();        break;
                }

            }

            if (FilterTag!=null)
            {
                var tagged = new List<CardModel>();
                foreach (var card in retour)
                {
                    if (await MageekStats.DoesCardHasTag(card.CardId, FilterTag.Tag))
                    {
                        tagged.Add(card);
                    }
                }
                retour = new List<CardModel>(tagged);
            }

            return retour;
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
            if (CardGrid.SelectedItem is CardModel card) App.Events.RaiseCardSelected(card);
        }

        private void AddToDeck(object sender, RoutedEventArgs e)
        {
            foreach (CardModel c in CardGrid.SelectedItems)
            {
                MageekCollection.AddCardToDeck(c, App.State.SelectedDeck,1)
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

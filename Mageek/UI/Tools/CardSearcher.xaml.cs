using MaGeek.AppData.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MaGeek.AppBusiness;
using MaGeek.AppFramework;

namespace MaGeek.UI
{

    public partial class CardSearcher : TemplatedUserControl
    {

        #region Attributes

        public List<CardModel> CardList { get; private set; }
        public List<CardTag> AvailableTags { get { return MageekStats.GetTagsDistinct().Result; } }

        #region Filters

        private string tagFilterSelected = "";
        public string TagFilterSelected
        {
            get { return tagFilterSelected; }
            set
            {
                tagFilterSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
            }
        }

        private string filterType = "";
        public string FilterType
        {
            get { return filterType; }
            set { filterType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CardList));
            }
        }
        
        private string filterKeyword = "";
        public string FilterKeyword
        {
            get { return filterKeyword; }
            set {
                filterKeyword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CardList));
            }
        }

        private int filterMinCmc = 0;
        public int FilterMinCmc
        {
            get { return filterMinCmc; }
            set { filterMinCmc = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CardList));
            }
        }

        private int filterMaxCmc = 20;
        public int FilterMaxCmc
        {
            get { return filterMaxCmc; }
            set { filterMaxCmc = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
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
                OnPropertyChanged(nameof(CardList));
            }
        }

        #endregion

        #region Visibilities

        private Visibility isLoading = Visibility.Visible;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #region CTOR

        public CardSearcher()
        {
            DataContext = this;
            InitializeComponent();
            App.Events.UpdateCardCollecEvent += async () => { await ReloadData(); };
            DelayLoad().ConfigureAwait(false);
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            App.Events.RaiseUpdateCardCollec();
        }

        #endregion

        #region Async data reload

        private async Task ReloadData()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(() =>
            {
                IsLoading = Visibility.Visible;
            });
            CardList = await LoadCards();
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(CardList));
                IsLoading = Visibility.Collapsed;
            });
        }

        private async Task<List<CardModel>> LoadCards()
        {
            List<CardModel> retour = new List<CardModel>();

            if (!string.IsNullOrEmpty(FilterName))
            {
                using (var DB = App.DB.GetNewContext())
                {

                    retour.AddRange(
                        await DB.CardModels.Include(x => x.Traductions)
                                           .Where(x => x.CardId.ToLower().Contains(FilterName.ToLower()))
                                           .ToArrayAsync()
                    );

                    string lang = App.Config.Settings[Setting.ForeignLanguage];
                    var t = DB.CardTraductions.Include(x => x.Card)
                                              .Where(x => x.Language == lang)
                                              .Where(x => x.TraductedName.ToLower().Contains(FilterName.ToLower()))
                                              .GroupBy(x=>x.CardId).Select(g => g.First());
                    foreach (var tt in t)
                    {
                        string traductedname = tt.CardId;
                        retour.AddRange(
                            await DB.CardModels.Include(x => x.Traductions)
                                               .Where(x => x.CardId.ToLower().Contains(traductedname.ToLower()))
                                               .ToArrayAsync()
                        );
                    }
                }

            }
            else
            {
                using (var DB = App.DB.GetNewContext())
                {
                    retour.AddRange( await DB.CardModels.Include(card => card.Traductions).ToArrayAsync());
                }
            }

            if(!string.IsNullOrEmpty(FilterType))
            {
                retour = retour.Where(x => x.Type.ToLower().Contains(FilterType.ToLower())).ToList();
            }

            if(!string.IsNullOrEmpty(FilterKeyword))
            {
                retour = retour.Where(x => x.KeyWords.ToLower().Contains(FilterKeyword.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(TagFilterSelected))
            {
                var tagged = new List<CardModel>();
                foreach (var card in retour)
                {
                    if (await MageekStats.DoesCardHasTag(card.CardId, TagFilterSelected))
                    {
                        tagged.Add(card);
                    }
                }
                return new List<CardModel>(tagged);
            }

            return retour;
        }

        #endregion

        #region UI Link

        private void CardGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardGrid.SelectedItem is CardModel card) App.Events.RaiseCardSelected(card);
        }

        private async void Button_SearchLocal(object sender, RoutedEventArgs e)
        {
            await ReloadData();
        }

        private async void Button_SearchOnline(object sender, RoutedEventArgs e)
        {
            IsLoading = Visibility.Visible;
            var cardlist = await MageekApi.RetrieveCard(FilterName, false, true,true);
            await MageekApi.RecordCards(cardlist);
            await ReloadData();
        }

        private async void Button_Reset(object sender, RoutedEventArgs e)
        {
            ResetFilters();
            await ReloadData();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (CardModel c in CardGrid.SelectedItems)
            {
                MageekCollection.AddCardToDeck(c, App.State.SelectedDeck,1)
                    .ConfigureAwait(true);
            }
        }

        private void FilterTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CardList));
        }

        private void FilterTag_DropDownOpened(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableTags));
        }

    }

}

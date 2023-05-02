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

        public List<MagicCard> CardList { get; private set; }
        public List<CardTag> AvailableTags { get { return MageekUtils.GetTagsDistinct().Result; } }

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
            CardList = await LoadCards();
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(CardList));
                IsLoading = Visibility.Collapsed;
            });
        }

        private async Task<List<MagicCard>> LoadCards()
        {
            IEnumerable<MagicCard> retour = new List<MagicCard>();
            string lang = App.Config.Settings[Setting.ForeignLangugage];

            using (var DB = App.DB.GetNewContext())
            {
                retour = await DB.Cards.Include(card => card.Traductions)
                                       .Where(x => x.Cmc >= FilterMinCmc)
                                       .Where(x => x.Cmc <= FilterMaxCmc)
                                       .Where(x => x.CardId.ToLower().Contains(FilterName.ToLower()) 
                                                || x.Traductions.Where(y=>y.Language==lang && y.TraductedName.ToLower().Contains(FilterName.ToLower())).Any())
                                       .Where(x => x.Type.ToLower().Contains(FilterType.ToLower()))
                                       .ToArrayAsync();
            }

            if (!filterColorB) retour = retour.Where(x => !x.ManaCost.Contains('B'));
            if (!filterColorW) retour = retour.Where(x => !x.ManaCost.Contains('W'));
            if (!filterColorU) retour = retour.Where(x => !x.ManaCost.Contains('U'));
            if (!filterColorG) retour = retour.Where(x => !x.ManaCost.Contains('G'));
            if (!filterColorR) retour = retour.Where(x => !x.ManaCost.Contains('R'));

            if (!string.IsNullOrEmpty(TagFilterSelected))
            {
                var tagged = new List<MagicCard>();
                foreach (var card in retour)
                {
                    if (await MageekUtils.DoesCardHasTag(card.CardId, TagFilterSelected))
                    {
                        tagged.Add(card);
                    }
                }
                return new List<MagicCard>(tagged);
            }
            return retour.ToList();
        }

        #endregion

        #region UI Link

        private void CardGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardGrid.SelectedItem is MagicCard card) App.Events.RaiseCardSelected(card);
        }

        private void Button_SearchLocal(object sender, RoutedEventArgs e)
        {
            ReloadData().ConfigureAwait(false);
        }

        private async void Button_SearchOnline(object sender, RoutedEventArgs e)
        {
            IsLoading = Visibility.Visible;
            var cardlist = await MageekApi.RetrieveCard(FilterName, false, true,true);
            await MageekApi.RecordCards(cardlist);
            await ReloadData();
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            ResetFilters();
            ReloadData().ConfigureAwait(false);
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
            foreach (MagicCard c in CardGrid.SelectedItems)
            {
                MageekUtils.AddCardToDeck(c.Variants[0], App.State.SelectedDeck,1)
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

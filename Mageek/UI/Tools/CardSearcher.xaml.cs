using MaGeek.AppData.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MaGeek.AppBusiness;
using MaGeek.AppFramework;
using System.Text.RegularExpressions;
using System.Text;
using System;
using MaGeek.AppFramework.Utils;

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
                filterName = StringExtension.RemoveDiacritics(value).ToLower();
                //filterName = value.ToLower();
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

        //private bool isReloading = false;
        private async Task ReloadData()
        {
            //if (isReloading) return;
            IsLoading = Visibility.Visible;
            await Task.Run(async () =>
            {
                //isReloading = true;
                CardList = await LoadCards();
                OnPropertyChanged(nameof(CardList));
                await Task.Delay(50);
                //isReloading = false;
            });
            IsLoading = Visibility.Collapsed;
        }


        private async Task<List<CardModel>> LoadCards()
        {
            string lang = App.Config.Settings[Setting.ForeignLanguage];
            List<CardModel> retour = new List<CardModel>();

            if (!string.IsNullOrEmpty(FilterName))
            {
                using (var DB = App.DB.GetNewContext())
                {

                    retour.AddRange(
                        await DB.CardModels//.Include(x => x.Traductions)
                            .Where(x => x.CardId.ToLower().Contains(FilterName))
                            .ToArrayAsync()
                    );

                    var trads = DB.CardTraductions.Include(x => x.Card)
                        .Where(x => x.Language == lang && x.Normalized.Contains(FilterName))
                        .GroupBy(x => x.CardId).Select(g => g.First());




                    foreach (var trad in trads)
                    {
                        string traductedname = trad.CardId.ToLower(); 
                        retour.AddRange(await DB.CardModels.Include(x => x.Traductions)
                                               .Where(x => x.CardId.ToLower().Contains(traductedname))
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

        public string ReplaceDiatrics(string input)
        {
            return Regex.Replace(input, @"[ÀÁÂÄÇÚÎÉÈÊ]", match => {
                switch (match.Value)
                {
                    case "À": return "à";
                    case "Á": return "á";
                    case "Â": return "â";
                    case "Ä": return "ä";
                    case "Ç": return "ç";
                    case "Ú": return "ú";
                    case "Î": return "î";
                    case "É": return "é";
                    case "È": return "è";
                    case "Ê": return "ê";
                    default: throw new Exception("Unexpected match!");
                }
            });
        }

        #endregion

        #region UI Link

        private void CardGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (CardGrid.SelectedItem is CardModel card) App.Events.RaiseCardSelected(card);
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

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                ReloadData().ConfigureAwait(false);
            }
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

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CardGrid.SelectedItem is CardModel card) App.Events.RaiseCardSelected(card);
        }
    }

}

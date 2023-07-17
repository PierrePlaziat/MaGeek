using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MageekSdk.Collection.Entities;
using MageekSdk.MtgSqlive.Entities;
using MtgSqliveSdk;
using MageekSdk.Tools;

namespace MaGeek.UI
{

    public partial class CardInspector : TemplatedUserControl
    {

        #region Attributes

        string loadMsg;
        public string LoadMsg
        {
            get { return loadMsg; }
            set { loadMsg = value; OnPropertyChanged(); }
        }
        
        private MageekSdk.Collection.Entities.ArchetypeCard selectedCard;
        public MageekSdk.Collection.Entities.ArchetypeCard SelectedCard
        {
            get { return selectedCard; }
            set
            {
                selectedCard = value;
                if (value != null) ReloadCard().ConfigureAwait(false);
                //UpdateButton.Visibility = Visibility.Visible;
            }
        }


        public List<CardLegalities> Legalities { get; private set; }
        public List<CardRulings> Rulings { get; private set; }
        //public List<CardRelation> RelatedCards { get; private set; }
        public List<Tag> Tags { get; private set; }

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive
        {
            get { return SelectedCard == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        //public Visibility ShowRelateds
        //{
        //    get { return RelatedCards == null || RelatedCards.Count>0 ? Visibility.Visible : Visibility.Collapsed; }
        //}
        
        public Visibility ShowRules
        {
            get { return Rulings == null || Rulings.Count>0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #endregion

        #region CTOR

        public CardInspector()
        {
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.CardSelectedEvent += HandleCardSelected;
        }

        void HandleCardSelected(string cardUuid)
        {
            //if (!isPinned)
            {
                if (string.IsNullOrEmpty(null)) SelectedCard = null;
                else
                {
                    IsLoading = Visibility.Visible;
                    Task.Run(() =>
                    {
                        LoadMsg = "Finding card";
                        SelectedCard = Mageek.FindCard_Ref(cardUuid).Result;
                    }).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region Async Reload

        private async Task ReloadCard()
        {
            try
            {
                IsLoading = Visibility.Visible; 
                await Task.Run(async () => {
                    await AutoSelectVariant();
                    LoadMsg = "Loading legalities";
                    Legalities = await Mageek.GetLegalities(SelectedCard);
                    LoadMsg = "Loading rulings";
                    Rulings =  await Mageek.GetRulings(SelectedCard);
                    LoadMsg = "Loading prices";
                    foreach (var v in await Mageek.FindCard_Variants(selectedCard.ArchetypeId)) 
                        await Mageek.EstimateCardPrice(v);
                    LoadMsg = "Loading relateds";
                    //RelatedCards = await MageekApi.GetRelatedCards(SelectedCard);
                    LoadMsg = "Loading tags";
                    Tags = await GetTags();
                    LoadMsg = "Updating";
                    //OnPropertyChanged(nameof(SelectedCard.Variants));
                    OnPropertyChanged(nameof(Legalities));
                    OnPropertyChanged(nameof(Tags));
                    OnPropertyChanged(nameof(Rulings));
                    //OnPropertyChanged(nameof(RelatedCards));
                    //OnPropertyChanged(nameof(ShowRelateds));
                    OnPropertyChanged(nameof(ShowRules));
                    OnPropertyChanged(nameof(SelectedCard));
                    OnPropertyChanged(nameof(IsActive));
                    await AutoSelectVariant();
                });
                IsLoading = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLvl.Error); 
            }
        }

        #endregion

        #region Variants

        private async Task AutoSelectVariant()
        {
            /*await Task.Run(() =>
            {
                if (selectedCard == null) return;
                if (!string.IsNullOrEmpty(selectedCard.FavouriteVariant))
                {
                    SelectedVariant = selectedCard.Variants.Where(x => x.Id == selectedCard.FavouriteVariant).FirstOrDefault();
                }
                else
                {
                    SelectedVariant = selectedCard.Variants.FirstOrDefault();
                }
            });*/
        }

        private async void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            string variant = (string) ((Button)sender).DataContext;
            await Mageek.CollectedCard_Add(variant);
            HandleCardSelected(variant);
        }

        private async void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            string variant = (string)((Button)sender).DataContext;
            await Mageek.CollectedCard_Remove(variant);
            HandleCardSelected(variant);
        }

        private async void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            await Mageek.AddCardToDeck(SelectedCard.CardUuid, App.State.SelectedDeck,1);
        }

        private async void SetFav(object sender, RoutedEventArgs e)
        {
            var cardvar = VariantListBox.Items[VariantListBox.SelectedIndex] as string;  
            await Mageek.SetFav(selectedCard.ArchetypeId, cardvar);
        }

        private void GotoRelated(object sender, RoutedEventArgs e)
        {
           /* CardRelation rel = (CardRelation)((Button)sender).DataContext;
            CardModel relatedCard;
            using (var DB = App.DB.NewContext)
            {
                relatedCard = DB.CardModels.Where(x => x.CardId == rel.Card2Id)
                    .ToList()
                    .FirstOrDefault();
            }
            if (relatedCard != null) App.Events.RaiseCardSelected(relatedCard);*/
        }

        private void LinkWeb_Cardmarket(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + SelectedCard.CardUuid);
        }
        
        private void LinkWeb_EdhRec(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://edhrec.com/cards/" + selectedCard.CardUuid.Split(" // ")[0].ToLower().Replace(' ','-').Replace("\"","").Replace(",","").Replace("'",""));
        }
        
        private void LinkWeb_Scryfall(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://scryfall.com/search?q=" + selectedCard.CardUuid);
        }

        #endregion

        #region Tags

        private async Task<List<Tag>> GetTags()
        {
            if (selectedCard == null) return null;
            return await Mageek.GetTags(selectedCard.ArchetypeId);
        }

        private async void AddTag(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTag.Text))
            {
                await Mageek.TagCard(selectedCard.ArchetypeId,NewTag.Text);
                OnPropertyChanged(nameof(Tags));
                NewTag.Text = "";
                sugestions.Visibility = Visibility.Collapsed;
            }
            HandleCardSelected(SelectedCard.CardUuid);
        }

        private async void DeleteTag(object sender, RoutedEventArgs e)
        {
            Tag cardTag = (Tag)((Button)sender).DataContext;
            await Mageek.UnTagCard(cardTag);
            OnPropertyChanged(nameof(Tags));
            sugestions.Visibility = Visibility.Collapsed;
            HandleCardSelected(SelectedCard.CardUuid);
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await Mageek.GetTags();
            string query = (sender as TextBox).Text;
            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }
            resultStack.Children.Clear();
            foreach (var obj in data)
            {
                if (obj.TagContent.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj.TagContent);
                    found = true;
                }
            }
            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void AddItem(string text)
        {
            TextBlock block = new()
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };
            block.MouseLeftButtonUp += (sender, e) =>
            {
                NewTag.Text = (sender as TextBlock).Text;
            };
            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Gray;
            };
            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
            resultStack.Children.Add(block);
        }

        private void NewTag_LostFocus(object sender, RoutedEventArgs e)
        {
            sugestions.Visibility = Visibility.Collapsed;
        }

        #endregion

    }

}

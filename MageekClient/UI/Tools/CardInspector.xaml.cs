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
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        
        private ArchetypeCard selectedCard;
        public ArchetypeCard SelectedCard
        {
            get { return selectedCard; }
            set
            {
                if (value != null)
                {
                    selectedCard = value;
                    OnPropertyChanged();
                }
            }
        }

        private CardVariant selectedvariant;
        public CardVariant SelectedVariant
        {
            get { return selectedvariant; }
            set { selectedvariant = value; OnPropertyChanged(); }
        }

        public List<CardVariant> Variants { get; private set; } = new();
        public List<CardLegalities> Legalities { get; private set; } = new();
        public List<CardRulings> Rulings { get; private set; } = new();
        public List<Cards> RelatedCards { get; private set; } = new();
        public List<Tag> Tags { get; private set; } = new();

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

        public Visibility ShowRelateds
        {
            get { return RelatedCards == null || RelatedCards.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

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
            if (Variants.Where(x => x.Card.Uuid == cardUuid).Any()) 
                ChangeVariant(cardUuid).ConfigureAwait(false);
            else 
                ReloadCard(cardUuid).ConfigureAwait(false);
        }

        #endregion

        #region Async Reload

        private async Task ChangeVariant(string cardUuid)
        {
        }

        private async Task ReloadCard(string cardUuid)
        {
            try
            {
                IsLoading = Visibility.Visible; 
                await Task.Run(
                    async () => {

                        LoadMsg = "Finding card";
                        SelectedCard = await Mageek.FindCard_Ref(cardUuid);
                        Variants = null;
                        Variants = new List<CardVariant>();
                        foreach (var variant in await Mageek.FindCard_Variants(SelectedCard.ArchetypeId))
                        {
                            var x = await Mageek.FindCard_Data(variant);
                            if (x != null)
                            {
                                CardVariant v = new();
                                v.Card = x;
                                v.Collected = await Mageek.CollectedCard_HowMany(x.Uuid);
                                v.PriceValue = await Mageek.EstimateCardPrice(x.Uuid);
                                v.Set = await Mageek.RetrieveSet(x.SetCode);
                                Variants.Add(v);
                                if (x.Uuid==cardUuid) selectedvariant = Variants.Last();
                            }
                        }
                        OnPropertyChanged(nameof(Variants));

                        LoadMsg = "Loading legalities";
                        Legalities = new();
                        Legalities = await Mageek.GetLegalities(SelectedCard);
                        OnPropertyChanged(nameof(Legalities));
                        LoadMsg = "Loading rulings";
                        Rulings = new();
                        Rulings =  await Mageek.GetRulings(SelectedCard);
                        OnPropertyChanged(nameof(Rulings));
                        OnPropertyChanged(nameof(ShowRules));
                        LoadMsg = "Loading relateds";
                        RelatedCards = new();
                        RelatedCards = await Mageek.FindCard_Related(SelectedVariant.Card);
                        OnPropertyChanged(nameof(RelatedCards));
                        OnPropertyChanged(nameof(ShowRelateds));
                        LoadMsg = "Loading tags";
                        Tags = new();
                        Tags = await GetTags();
                        OnPropertyChanged(nameof(Tags));

                        LoadMsg = "Loading Details";
                        foreach (var v in Variants)
                        {
                        }
                        LoadMsg = "Done";
                        OnPropertyChanged(nameof(IsActive));
                    }
                );
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
                //Cursor = Cursors.Hand
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

    public class CardVariant/* : INotifyPropertyChanged */
    {

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged([CallerMemberName] string name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}


        Cards card;
        public Cards Card { get; set; }

        int collected;
        public int Collected { get; set; }

        Sets set;
        public Sets Set { get; set; }

        PriceLine priceValue;
        public PriceLine PriceValue{ get; set; }

        public Brush GetPriceColor { get { return Brushes.Gray; } }
        public Brush GetRarityColor { get { return Brushes.Black; } }
        public string  GetPrice
        { 
            get 
            {
                if (PriceValue == null) return "";
                if (PriceValue.PriceEur == null) return "";
                return PriceValue.PriceEur.ToString();
            } 
        } //TODO multi monaie
    }

}
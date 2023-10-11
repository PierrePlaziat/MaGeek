using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using MageekSdk;
using MageekSdk.Data.Collection.Entities;
using MageekSdk.Data.Mtg.Entities;
using MageekSdk.Data;
using MageekSdk.Tools;

namespace MaGeek.UI
{

    public partial class CardInspector : TemplatedUserControl
    {

        #region Attributes
 
        

        private CardVariant selectedvariant;
        public CardVariant SelectedVariant
        {
            get { return selectedvariant; }
            set { selectedvariant = value; OnPropertyChanged(); }
        }

        public List<CardVariant> Variants { get; private set; } = new();
        public CardLegalities Legalities { get; private set; } = new();
        public List<CardRulings> Rulings { get; private set; } = new();
        public List<CardCardRelation> RelatedCards { get; private set; } = new();
        public List<Tag> Tags { get; private set; } = new();

        #region Calculated on the fly

        public int TotalGot { get { return Variants.Sum(x => x.Card.Collected); } }
        public decimal MeanPrice { get { return Variants.Count == 0 ? 0 : Variants.Where(x => x.GetPrice.HasValue).Sum(x => x.GetPrice.Value) / Variants.Count; } }
        public int VariantCount { get { return Variants.Count; } }

        #endregion

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        string loadMsg;
        public string LoadMsg
        {
            get { return loadMsg; }
            set { loadMsg = value; OnPropertyChanged(); }
        }
       
        public Visibility IsActive
        {
            get { return SelectedVariant == null ? Visibility.Visible : Visibility.Collapsed; }
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
            App.Events.CardSelectedEvent += SelectCard;
        }

        #endregion

        #region Methods

        void SelectCard(string cardUuid)
        {
            ReloadCard(cardUuid).ConfigureAwait(false);
        }

        private async Task ReloadCard(string variantUuid)
        {
            try
            {
                LoadMsg = "...";
                IsLoading = Visibility.Visible; 
                string cardArchetype = await MageekService.FindCard_Archetype(variantUuid);

                LoadMsg = "Finding variants";
                SelectedVariant = null;
                Variants = null;
                Variants = new List<CardVariant>();

                foreach (string variant in await MageekService.FindCard_Variants(cardArchetype))
                {
                    var x = await MageekService.FindCard_Data(variant);
                    if (x != null)
                    {
                        CardVariant v = new()
                        {
                            Card = x,
                            PriceValue = await MageekService.EstimateCardPrice(x.Uuid)
                        };
                        Variants.Add(v);
                        if (x.Uuid==variantUuid) selectedvariant = v;
                    }
                }
                OnPropertyChanged(nameof(Variants));
                OnPropertyChanged(nameof(SelectedVariant));

                LoadMsg = "Loading legalities";
                Legalities = new();
                Legalities = await MageekService.GetLegalities(variantUuid);
                OnPropertyChanged(nameof(Legalities));

                LoadMsg = "Loading rulings";
                Rulings = new();
                Rulings = await MageekService.GetRulings(variantUuid);
                OnPropertyChanged(nameof(Rulings));
                OnPropertyChanged(nameof(ShowRules));

                LoadMsg = "Loading relateds";
                RelatedCards = new();
                RelatedCards = await MageekService.FindCard_Related(SelectedVariant.Card);
                OnPropertyChanged(nameof(RelatedCards));
                OnPropertyChanged(nameof(ShowRelateds));

                LoadMsg = "Loading tags";
                Tags = new();
                Tags = await GetTags();
                OnPropertyChanged(nameof(Tags));

                LoadMsg = "Done";
                OnPropertyChanged(nameof(TotalGot));
                OnPropertyChanged(nameof(MeanPrice));
                OnPropertyChanged(nameof(VariantCount));
                OnPropertyChanged(nameof(IsActive));

                IsLoading = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                Logger.Log(e); 
            }
        }

        #region Operations

        private async void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            CardVariant variant = (CardVariant) ((Button)sender).DataContext;
            await MageekService.CollectedCard_Add(variant.Card.Uuid);
            SelectCard(variant.Card.Uuid);
        }

        private async void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            CardVariant variant = (CardVariant)((Button)sender).DataContext;
            await MageekService.CollectedCard_Remove(variant.Card.Uuid);
            SelectCard(variant.Card.Uuid);
        }

        private async void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            try
            {
                await MageekService.AddCardToDeck(SelectedVariant.Card.Uuid, App.State.SelectedDeck, 1);
                App.Events.RaiseUpdateDeck();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async void SetFav(object sender, RoutedEventArgs e)
        {
            await MageekService.SetFav(SelectedVariant.Card.Name, SelectedVariant.Card.Uuid);
        }

        #region Tags

        private async Task<List<Tag>> GetTags()
        {
            if (SelectedVariant == null) return null;
            return await MageekService.GetTags(SelectedVariant.Card.Name);
        }

        private async void AddTag(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTag.Text))
            {
                await MageekService.TagCard(SelectedVariant.Card.Name, NewTag.Text);
                OnPropertyChanged(nameof(Tags));
                NewTag.Text = "";
                sugestions.Visibility = Visibility.Collapsed;
            }
            SelectCard(SelectedVariant.Card.Uuid);
        }

        private async void DeleteTag(object sender, RoutedEventArgs e)
        {
            Tag cardTag = (Tag)((Button)sender).DataContext;
            await MageekService.UnTagCard(cardTag);
            OnPropertyChanged(nameof(Tags));
            sugestions.Visibility = Visibility.Collapsed;
            SelectCard(SelectedVariant.Card.Uuid);
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await MageekService.GetTags();
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
                if (obj != null && obj.TagContent.ToLower().StartsWith(query.ToLower()))
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

        #region Links

        private void LinkWeb_Cardmarket(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + SelectedVariant.Card.Name);
        }
        
        private void LinkWeb_EdhRec(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://edhrec.com/cards/" + SelectedVariant.Card.Name.Split(" // ")[0].ToLower().Replace(' ','-').Replace("\"","").Replace(",","").Replace("'",""));
        }
        
        private void LinkWeb_Scryfall(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://scryfall.com/search?q=" + SelectedVariant.Card.Name);
        }

        #endregion

        #endregion

        #endregion

        private void GoToRelated(object sender, MouseButtonEventArgs e)
        {
            ListView lv = (ListView)sender;
            if (lv.SelectedItem!=null)
            {
                CardCardRelation c = lv.SelectedItem as CardCardRelation;
                if(c.Role == CardCardRelationRole.token)
                    InspectToken();
                else 
                    SelectCard(c.Card.Uuid);
            }
        }

        private void InspectToken()
        {
            //TODO
            throw new NotImplementedException();
        }
    }

    public class CardVariant
    {
        public Cards Card { get; set; }

        public PriceLine PriceValue{ get; set; }

        public Brush GetPriceColor 
        { 
            get 
            {
                if (PriceValue==null) return Brushes.Black;
                if (!PriceValue.PriceEur.HasValue) return Brushes.Black;

                decimal? price;
                switch (App.Config.Settings[Setting.Currency])
                {
                    case "Eur" : price = PriceValue.PriceEur; break;
                    case "Usd" : price = PriceValue.PriceUsd; break;
                    default: return Brushes.Black; 
                }
                if (!price.HasValue) return Brushes.Black;

                if ((float)price <= .1) return Brushes.DarkGray;
                else if (price <= 1) return Brushes.LightGray;
                else if (price <= 2) return Brushes.White;
                else if (price <= 5) return Brushes.Green;
                else if (price <= 10) return Brushes.Blue;
                else if (price <= 20) return Brushes.Red;
                else if (price <= 50) return Brushes.Yellow;
                else if (price <= 100) return Brushes.Cyan;
                else return Brushes.Purple;
            }
        }
        public Brush GetRarityColor
        { 
            get 
            {
                return Card.Rarity switch
                {
                    "common" => Brushes.White,
                    "uncommon" => Brushes.Gray,
                    "rare" => Brushes.Gold,
                    "mythic" => Brushes.Orange,
                    "bonus" => Brushes.Cyan,
                    _ => Brushes.Purple,
                };
            } 
        }
        public decimal?  GetPrice
        { 
            get 
            {
                if (PriceValue == null) return null;
                if (PriceValue.PriceEur == null) return null;
                return PriceValue.PriceEur;
            } 
        } //TODO multi monaie & colors
    }

}
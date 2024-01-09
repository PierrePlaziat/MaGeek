using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.ViewModels
{

    public partial class CardInspectorViewModel : BaseViewModel
    {

        private AppEvents events;
        private AppState state;
        private SettingService settings;
        public CardInspectorViewModel(
            AppEvents events,
            AppState state,
            SettingService settings
        ){
            this.events = events;
            this.state = state;
            this.settings = settings;
            events.CardSelectedEvent += SelectCard;
        }


        [ObservableProperty] private string selectedUuid;
        [ObservableProperty] private CardVariant selectedVariant;
        [ObservableProperty] private List<CardVariant> variants;
        [ObservableProperty] private CardLegalities legalities;
        [ObservableProperty] private List<CardRulings> rulings;
        [ObservableProperty] private List<CardCardRelation> relatedCards;
        [ObservableProperty] private List<Tag> tags;
        [ObservableProperty] private string loadMsg;
        [ObservableProperty] private bool isLoading = false;

        public int TotalGot { get { return Variants.Sum(x => x.Card.Collected); } }
        public decimal MeanPrice { get { return Variants.Count == 0 ? 0 : Variants.Where(x => x.GetPrice.HasValue).Sum(x => x.GetPrice.Value) / Variants.Count; } }
        public int VariantCount { get { return Variants.Count; } }
        public Visibility IsActive { get { return SelectedVariant == null ? Visibility.Visible : Visibility.Collapsed; } }

        private void SelectCard(string cardUuid)
        {
            ReloadCard(cardUuid).ConfigureAwait(false);
        }

        [RelayCommand] 
        private async Task AddCardToCollection(CardVariant variant)
        {
            await MageekService.MageekService.CollecMove(variant.Card.Uuid, 1);
            SelectCard(variant.Card.Uuid);
        }

        [RelayCommand]
        private async Task GoToRelated(CardCardRelation c)
        {
            if (c.Role == CardCardRelationRole.token)
                InspectToken(c.Token.Uuid);
            else
                SelectCard(c.Card.Uuid);
        }

        [RelayCommand]
        private async Task ReloadCard(string variantUuid)
        {
            try
            {
                LoadMsg = "...";
                IsLoading = true;
                string cardArchetype = await MageekService.MageekService.FindCard_Archetype(variantUuid);

                LoadMsg = "Finding variants";
                SelectedVariant = null;
                Variants = null;
                Variants = new List<CardVariant>();

                foreach (string variant in await MageekService.MageekService.FindCard_Variants(cardArchetype))
                {
                    var x = await MageekService.MageekService.FindCard_Data(variant);
                    if (x != null)
                    {
                        CardVariant v = new()
                        {
                            Card = x,
                            PriceValue = await MageekService.MageekService.EstimateCardPrice(x.Uuid)
                        };
                        Variants.Add(v);
                        if (x.Uuid == variantUuid) selectedVariant = v;
                    }
                }

                LoadMsg = "Loading legalities";
                Legalities = new();
                Legalities = await MageekService.MageekService.GetLegalities(variantUuid);

                LoadMsg = "Loading rulings";
                Rulings = new();
                Rulings = await MageekService.MageekService.GetRulings(variantUuid);

                LoadMsg = "Loading relateds";
                RelatedCards = new();
                RelatedCards = await MageekService.MageekService.FindCard_Related(SelectedVariant.Card);

                LoadMsg = "Loading tags";
                Tags = new();
                //Tags = await GetTags();
                OnPropertyChanged(nameof(Tags));

                LoadMsg = "Done";
                OnPropertyChanged(nameof(TotalGot));
                OnPropertyChanged(nameof(MeanPrice));
                OnPropertyChanged(nameof(VariantCount));
                OnPropertyChanged(nameof(IsActive));

                IsLoading = false;
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        [RelayCommand]
        private async Task SubstractCardFromCollection(CardVariant variant)
        {
            await MageekService.MageekService.CollecMove(variant.Card.Uuid, -1);
            SelectCard(variant.Card.Uuid);
        }

        [RelayCommand]
        private async Task AddToCurrentDeck()
        {
            try
            {
                await MageekService.MageekService.AddCardToDeck(SelectedVariant.Card.Uuid, state.SelectedDeck, 1);
                events.RaiseUpdateDeck();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        [RelayCommand]
        private async void SetFav()
        {
            await MageekService.MageekService.SetFav(SelectedVariant.Card.Name, SelectedVariant.Card.Uuid);
        }

        [RelayCommand]
        private async void LinkWeb(string linkto)
        {
            switch(linkto)
            {
                case "Cardmarket": HttpUtils.HyperLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + SelectedVariant.Card.Name); break;
                case "EdhRec": HttpUtils.HyperLink("https://edhrec.com/cards/" + SelectedVariant.Card.Name.Split(" // ")[0].ToLower().Replace(' ', '-').Replace("\"", "").Replace(",", "").Replace("'", "")); break;
                case "Scryfall": HttpUtils.HyperLink("https://scryfall.com/search?q=" + SelectedVariant.Card.Name); break;
            }
        }

        [RelayCommand]
        private void InspectToken(string tokenUuid)
        {
            //TODO
            throw new NotImplementedException();
        }

        internal void SelectCard(DeckCard cardRel)
        {
            throw new NotImplementedException();
        }

        //#region Tags

        //private async Task<List<Tag>> GetTags()
        //{
        //    if (SelectedVariant == null) return null;
        //    return await MageekService.MageekService.GetTags(SelectedVariant.Card.Name);
        //}

        //private async void AddTag(object sender, RoutedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(NewTag.Text))
        //    {
        //        await MageekService.MageekService.TagCard(SelectedVariant.Card.Name, NewTag.Text);
        //        OnPropertyChanged(nameof(Tags));
        //        NewTag.Text = "";
        //        sugestions.Visibility = Visibility.Collapsed;
        //    }
        //    SelectCard(SelectedVariant.Card.Uuid);
        //}

        //private async void DeleteTag(object sender, RoutedEventArgs e)
        //{
        //    Tag cardTag = (Tag)((Button)sender).DataContext;
        //    await MageekService.MageekService.UnTagCard(cardTag.ArchetypeId, cardTag.TagContent);
        //    OnPropertyChanged(nameof(Tags));
        //    sugestions.Visibility = Visibility.Collapsed;
        //    SelectCard(SelectedVariant.Card.Uuid);
        //}

        //private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        //{
        //    bool found = false;
        //    var border = (resultStack.Parent as ScrollViewer).Parent as Border;
        //    var data = await MageekService.MageekService.GetTags();
        //    string query = (sender as TextBox).Text;
        //    if (query.Length == 0)
        //    {
        //        resultStack.Children.Clear();
        //        border.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        border.Visibility = Visibility.Visible;
        //    }
        //    resultStack.Children.Clear();
        //    foreach (var obj in data)
        //    {
        //        if (obj != null && obj.TagContent.ToLower().StartsWith(query.ToLower()))
        //        {
        //            AddItem(obj.TagContent);
        //            found = true;
        //        }
        //    }
        //    if (!found)
        //    {
        //        resultStack.Children.Add(new TextBlock() { Text = "No results found." });
        //    }
        //}

        //private void AddItem(string text)
        //{
        //    TextBlock block = new()
        //    {
        //        Text = text,
        //        Margin = new Thickness(2, 3, 2, 3),
        //        //Cursor = Cursors.Hand
        //    };
        //    block.MouseLeftButtonUp += (sender, e) =>
        //    {
        //        NewTag.Text = (sender as TextBlock).Text;
        //    };
        //    block.MouseEnter += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Gray;
        //    };
        //    block.MouseLeave += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Transparent;
        //    };
        //    resultStack.Children.Add(block);
        //}

        //private void NewTag_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    sugestions.Visibility = Visibility.Collapsed;
        //}

        //#endregion

    }

    public class CardVariant
    {
        public Cards Card { get; set; }

        public PriceLine PriceValue { get; set; }

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
        public decimal? GetPrice
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

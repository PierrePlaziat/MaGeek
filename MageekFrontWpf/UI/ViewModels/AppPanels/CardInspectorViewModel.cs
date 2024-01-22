using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using MageekService.Tools;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MageekFrontWpf.ViewModels
{

    public partial class CardInspectorViewModel : BaseViewModel
    {


        private AppEvents events;
        private AppState state;

        public CardInspectorViewModel(
            AppEvents events,
            AppState state,
            SettingService settings
        ){
            this.events = events;
            this.state = state;
            events.CardSelectedEvent += SelectCard;
        }

        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private string selectedArchetype;
        [ObservableProperty] private string selectedUuid;
        [ObservableProperty] private List<CardRulings> rulings;
        [ObservableProperty] private List<CardCardRelation> relatedCards;
        [ObservableProperty] private List<Tag> tags;
        [ObservableProperty] private CardLegalities legalities;
        [ObservableProperty] private bool isFav = false;
        [ObservableProperty] private int totalGot;
        [ObservableProperty] private decimal meanPrice;
        [ObservableProperty] private int variantCount;
        [ObservableProperty] private CardVariant selectedVariant;
        [ObservableProperty] private List<CardVariant> variants;

        public void SelectCard(string cardUuid)
        {
             Reload(cardUuid).ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task Reload(string uuid)
        {
            if (uuid == null) return;
            IsLoading = true;
            string archetype = await MageekService.MageekService.FindCard_Archetype(uuid);
            if (archetype == null)
            {
                IsLoading = false;
                return;
            }
            SelectedArchetype = archetype;
            SelectedUuid = uuid;
            await Task.WhenAll(
                new List<Task>
                {
                    GetCardVariants(),
                    GetLegalities(),
                    GetRulings(),  
                    GetRelatedCards(), 
                    GetTags(), 
                }
            );
            await Task.WhenAll(
                new List<Task>
                {
                    GetTotalGot(),
                    GetMeanPrice(),
                    GetVariantCount()
                }
            );
            IsLoading = false;
        }
        private async Task GetCardVariants()
        {
            SelectedVariant = null;
            Variants = null;
            Variants = new List<CardVariant>();

            foreach (string variant in await MageekService.MageekService.FindCard_Variants(SelectedArchetype))
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
                    if (x.Uuid == SelectedUuid) SelectedVariant = v;
                }
            }
        }
        private async Task GetLegalities()
        {
            Legalities = await MageekService.MageekService.GetLegalities(SelectedUuid); 
        }
        private async Task GetRulings()
        {
            Rulings = await MageekService.MageekService.GetRulings(SelectedUuid);
        }
        private async Task GetRelatedCards()
        {
            RelatedCards = await MageekService.MageekService.FindCard_Related(SelectedVariant.Card); 
        }
        private async Task GetTags()
        {
            Tags = await MageekService.MageekService.GetTags(SelectedVariant.Card.Name);
        }
        private async Task GetTotalGot() 
        {
            TotalGot = Variants.Sum(x => x.Card.Collected);
        }
        private async Task GetMeanPrice()
        {
            MeanPrice = Variants.Count == 0 ? 0 : Variants.Where(x => x.GetPrice.HasValue).Sum(x => x.GetPrice.Value) / Variants.Count;
        }
        private async Task GetVariantCount()
        {
            VariantCount = Variants.Count;
        }

        [RelayCommand]
        private async Task SetFav(string uuid)
        {
            await MageekService.MageekService.SetFav(SelectedArchetype, uuid);
            IsFav = true;
        }

        [RelayCommand]
        private async Task AddToCurrentDeck(string uuid)
        {
            await MageekService.MageekService.AddCardToDeck(uuid, state.SelectedDeck, 1);
            events.RaiseUpdateDeck();
        }

        [RelayCommand] 
        private async Task AddCardToCollection(string uuid)
        {
            await MageekService.MageekService.CollecMove(uuid, 1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task SubstractCardFromCollection(string uuid)
        {
            await MageekService.MageekService.CollecMove(uuid, -1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task AddTag(string txt)
        {
            await MageekService.MageekService.TagCard(SelectedVariant.Card.Name, txt);
            await GetTags();
        }

        [RelayCommand]
        private async Task DeleteTag(string txt)
        {
            await MageekService.MageekService.UnTagCard(SelectedVariant.Card.Name, txt);
            await GetTags();
        }

        [RelayCommand]
        private async Task GoToRelated(CardCardRelation c)
        {
            if (c.Role == CardCardRelationRole.token) { }//TODO InspectToken(c.Token.Uuid);
            else await Reload(c.Card.Uuid);
        }

        [RelayCommand]
        private void LinkWeb(string linkto)
        {
            switch (linkto)
            {
                case "Cardmarket": HttpUtils.HyperLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + SelectedVariant.Card.Name); break;
                case "EdhRec": HttpUtils.HyperLink("https://edhrec.com/cards/" + SelectedVariant.Card.Name.Split(" // ")[0].ToLower().Replace(' ', '-').Replace("\"", "").Replace(",", "").Replace("'", "")); break;
                case "Scryfall": HttpUtils.HyperLink("https://scryfall.com/search?q=" + SelectedVariant.Card.Name); break;
            }
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

}

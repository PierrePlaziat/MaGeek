using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using PlaziatTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MageekCore;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{

    public partial class CardInspectorViewModel : 
        BaseViewModel, 
        IRecipient<CardSelectedMessage>
    {

        MageekService mageek;

        public CardInspectorViewModel(MageekService mageek)
        {
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] private string selectedArchetype;
        [ObservableProperty] private string selectedUuid;
        [ObservableProperty] private CardVariant selectedVariant;
        [ObservableProperty] private List<CardVariant> variants;
        [ObservableProperty] private List<CardCardRelation> relatedCards;
        [ObservableProperty] private CardLegalities legalities;
        [ObservableProperty] private List<CardRulings> rulings;
        [ObservableProperty] private List<Tag> tags;
        [ObservableProperty] private int variantCount;
        [ObservableProperty] private int totalGot;
        [ObservableProperty] private decimal meanPrice;
        [ObservableProperty] private bool isFav = false;
        [ObservableProperty] private bool pinned = false;
        [ObservableProperty] private bool isLoading = false;

        public void Receive(CardSelectedMessage message)
        {
            if (!Pinned) Reload(message.Value).ConfigureAwait(false);
        }

        [RelayCommand]
        public async Task Reload(string uuid)
        {
            if (uuid == null) return;
            Logger.Log("Reload");
            IsLoading = true;
            await Task.Delay(100);
            string archetype = await mageek.FindCard_Archetype(uuid);
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

            foreach (string variant in await mageek.FindCard_Variants(SelectedArchetype))
            {
                var card = await mageek.FindCard_Data(variant);
                if (card != null)
                {
                    CardVariant v = new(
                        card,
                        await mageek.GetSet(card.SetCode),
                        await mageek.Collected(card.Uuid),
                        await mageek.EstimateCardPrice(card.Uuid)
                    );
                    Variants.Add(v);
                    if (card.Uuid == SelectedUuid) SelectedVariant = v;
                }
            }
        }
        private async Task GetLegalities()
        {
            Legalities = await mageek.GetLegalities(SelectedUuid); 
        }
        private async Task GetRulings()
        {
            Rulings = await mageek.GetRulings(SelectedUuid);
        }
        private async Task GetRelatedCards()
        {
            RelatedCards = await mageek.FindCard_Related(SelectedUuid, SelectedArchetype); 
        }
        private async Task GetTags()
        {
            Tags = await mageek.GetTags(SelectedArchetype);
        }
        private async Task GetTotalGot() 
        {
            TotalGot = await mageek.Collected_AllVariants(SelectedArchetype);
        }
        private async Task GetMeanPrice()
        {
            await Task.Run(() => {
                MeanPrice = Variants.Count == 0 ? 0 : Variants.Where(x => x.GetPrice.HasValue).Sum(x => x.GetPrice.Value) / Variants.Count;
            });
        }
        private async Task GetVariantCount()
        {
            await Task.Run(() => {
                VariantCount = Variants.Count;
            });
        }

        [RelayCommand]
        private async Task SetFav(string uuid)
        {
            await mageek.SetFav(SelectedArchetype, uuid);
            IsFav = true;
        }

        [RelayCommand]
        private async Task AddToCurrentDeck(string uuid)
        {
            // TODO think a system to select deck
            throw new NotImplementedException();
            //await mageek.AddCardToDeck(uuid, state.SelectedDeck, 1);
            //events.RaiseUpdateDeck();
        }

        [RelayCommand] 
        private async Task AddCardToCollection(string uuid)
        {
            await mageek.CollecMove(uuid, 1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task SubstractCardFromCollection(string uuid)
        {
            await mageek.CollecMove(uuid, -1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task AddTag(string txt)
        {
            await mageek.TagCard(SelectedVariant.Card.Name, txt);
            await GetTags();
        }

        [RelayCommand]
        private async Task DeleteTag(string txt)
        {
            await mageek.UnTagCard(SelectedVariant.Card.Name, txt);
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
                case "Cardmarket": HttpUtils.OpenLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + SelectedVariant.Card.Name); break;
                case "EdhRec": HttpUtils.OpenLink("https://edhrec.com/cards/" + SelectedVariant.Card.Name.Split(" // ")[0].ToLower().Replace(' ', '-').Replace("\"", "").Replace(",", "").Replace("'", "")); break;
                case "Scryfall": HttpUtils.OpenLink("https://scryfall.com/search?q=" + SelectedVariant.Card.Name); break;
            }
        }

    }

}

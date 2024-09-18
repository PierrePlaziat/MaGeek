using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using PlaziatTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;
using PlaziatWpf.Services;

namespace MageekDesktopClient.UI.ViewModels.AppPanels
{

    public partial class CardInspectorViewModel : 
        ObservableViewModel, 
        IRecipient<CardSelectedMessage>
    {

        IMageekService mageek;
        private SessionBag session;

        public CardInspectorViewModel(IMageekService mageek, SessionBag session)
        {
            this.mageek = mageek;
            this.session = session;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] private string selectedArchetype;
        [ObservableProperty] private string selectedUuid;
        [ObservableProperty] private CardVariant selectedVariant;
        [ObservableProperty] private List<CardVariant> variants;
        [ObservableProperty] private List<CardRelation> relatedCards;
        [ObservableProperty] private CardLegalities legalities;
        [ObservableProperty] private List<CardRulings> rulings;
        [ObservableProperty] private List<Tag> tags;
        [ObservableProperty] private int variantCount;
        [ObservableProperty] private int totalGot;
        [ObservableProperty] private float meanPrice;
        [ObservableProperty] private bool isFav = false;
        [ObservableProperty] private bool pinned = false;
        [ObservableProperty] private bool variantsLoading = false;
        [ObservableProperty] private bool isLoading = false;

        public void Receive(CardSelectedMessage message)
        {
            if (!Pinned) Reload(message.Value).ConfigureAwait(false);
        }

        [RelayCommand]
        public async Task Reload(string uuid)
        {
            if (uuid == null) return;
            SelectedUuid = uuid;
            if (Variants == null || !Variants.Where(x => x.Card.Uuid == uuid).Any())
            {
                Logger.Log("Reload");
                IsLoading = true;
                string archetype = await mageek.Cards_NameForGivenCardUuid(uuid);
                if (archetype == null) throw new Exception("Archetype not found");
                SelectedArchetype = archetype;
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
                        DetermineTotalGot(),
                        DetermineMeanPrice(),
                        DetermineVariantCount()
                    }
                );
            }
            IsLoading = false;
        }

        private async Task GetCardVariants()
        {
            VariantsLoading = true;
            SelectedVariant = null;
            var VVariants = new List<CardVariant>();

            foreach (string variant in await mageek.Cards_UuidsForGivenCardName(SelectedArchetype))
            {
                var card = await mageek.Cards_GetData(variant);
                if (card != null)
                {
                    CardVariant v = new(
                        card,
                        await mageek.Sets_Get(card.SetCode),
                        await mageek.Collec_OwnedVariant(session.UserName, card.Uuid),
                        await mageek.Cards_GetPrice(card.Uuid)
                    );
                    VVariants.Add(v);
                    if (card.Uuid == SelectedUuid) SelectedVariant = v;
                }
            }
            Variants = VVariants;
            VariantsLoading = false;
        }
        private async Task GetLegalities()
        {
            Legalities = await mageek.Cards_GetLegalities(SelectedUuid); 
        }
        private async Task GetRulings()
        {
            Rulings = await mageek.Cards_GetRulings(SelectedUuid);
        }
        private async Task GetRelatedCards()
        {
            RelatedCards = await mageek.Cards_GetRelations(SelectedUuid); 
        }
        private async Task GetTags()
        {
            Tags = await mageek.Tags_GetCardTags(session.UserName, SelectedArchetype);
        }
        private async Task DetermineTotalGot() 
        {
            TotalGot = await mageek.Collec_OwnedCombined(session.UserName, SelectedArchetype);
        }
        private async Task DetermineMeanPrice()
        {
            await Task.Run(() => {
                MeanPrice = Variants.Count == 0 ? 0 : Variants.Where(x => x.Price.LastPriceEur.HasValue).Sum(x => x.Price.LastPriceEur.Value) / Variants.Count;
            });
        }
        private async Task DetermineVariantCount()
        {
            await Task.Run(() => {
                VariantCount = Variants.Count;
            });
        }

        [RelayCommand]
        private async Task SetFav(string uuid)
        {
            await mageek.Collec_SetFavCardVariant(session.UserName, SelectedArchetype, uuid);
            IsFav = true;
        }

        [RelayCommand] 
        private async Task AddCardToCollection(string uuid)
        {
            await mageek.Collec_Move(session.UserName, uuid, 1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task SubstractCardFromCollection(string uuid)
        {
            await mageek.Collec_Move(session.UserName, uuid, -1);
            await GetCardVariants();
        }

        [RelayCommand]
        private async Task AddTag(string txt)
        {
            await mageek.Tags_TagCard(session.UserName, SelectedVariant.Card.Name, txt);
            await GetTags();
        }

        [RelayCommand]
        private async Task DeleteTag(string txt)
        {
            await mageek.Tags_UntagCard(session.UserName, SelectedVariant.Card.Name, txt);
            await GetTags();
        }

        [RelayCommand]
        private async Task GoToRelated(CardRelation c)
        {
            if (c.Role == CardRelationRole.token) 
            {
                //TODO Token support
            }
            else await Reload(c.CardUuid);
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

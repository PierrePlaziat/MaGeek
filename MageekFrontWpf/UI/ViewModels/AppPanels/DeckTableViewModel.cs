using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekServices.Data.Collection.Entities;
using MageekServices.Data.Mtg.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{

    public partial class DeckTableViewModel : BaseViewModel,
        IRecipient<DeckSelectMessage>,
        IRecipient<UpdateDeckMessage>
    {

        private MageekServices.MageekService mageek;
        private readonly ILogger<DeckTableViewModel> logger;

        public DeckTableViewModel(
            MageekServices.MageekService mageek,
            ILogger<DeckTableViewModel> logger
        ){
            this.logger = logger;
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] private Deck currentDeck;
        [ObservableProperty] private List<DeckCard> cardRelations_Commandant;
        [ObservableProperty] private List<DeckCard> cardRelations_Content;
        [ObservableProperty] private List<DeckCard> cardRelations_Side;
        [ObservableProperty] private List<DeckCard> cardRelations_Lands;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc0;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc1;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc2;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc3;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc4;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc5;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc6;
        [ObservableProperty] private List<DeckCard> cardRelations_Cmc7;
        [ObservableProperty] private bool hasCommandant;
        [ObservableProperty] private bool hasSide;
        [ObservableProperty] private bool hasLands;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private int currentCardSize;
        private readonly Dictionary<DeckCard, Cards> cardData = new();

        const int CardSize_Complete = 207;
        const int CardSize_Picture = 130;
        const int CardSize_Header = 25;

        public void Receive(UpdateDeckMessage message)
        {
            Deck tmp = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = tmp;
        }

        public void Receive(DeckSelectMessage message)
        {
            CurrentDeck = mageek.GetDeck(message.Value).Result;
        }

        private void FullRefresh()
        {
            var deck = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = deck;
            AsyncReload();
        }

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            try
            {
                IsLoading = true;
                OnPropertyChanged(nameof(IsActive));
                CardRelations_Content = await mageek.GetDeckContent_Related(CurrentDeck.DeckId, 0);
                CardRelations_Commandant = await mageek.GetDeckContent_Related(CurrentDeck.DeckId, 1);
                CardRelations_Side = await mageek.GetDeckContent_Related(CurrentDeck.DeckId, 2);
                CardRelations_Lands = await mageek.GetDeckContent_Typed(CurrentDeck.DeckId, "Land");
                OnPropertyChanged(nameof(CardRelations_Content));
                OnPropertyChanged(nameof(CardRelations_Commandant));
                OnPropertyChanged(nameof(CardRelations_Side));
                OnPropertyChanged(nameof(CardRelations_Lands));
                HasCommandant = CardRelations_Commandant.Count > 0;
                HasSide = CardRelations_Side.Count > 0;
                HasLands = CardRelations_Lands.Count > 0;
                OnPropertyChanged(nameof(HasCommandant));
                OnPropertyChanged(nameof(HasSide));
                //OnPropertyChanged(nameof(HasContent));
                OnPropertyChanged(nameof(HasLands));
                await RetrieveCardData();
                CardRelations_Cmc0 = GetCmc(0);
                CardRelations_Cmc1 = GetCmc(1);
                CardRelations_Cmc2 = GetCmc(2);
                CardRelations_Cmc3 = GetCmc(3);
                CardRelations_Cmc4 = GetCmc(4);
                CardRelations_Cmc5 = GetCmc(5);
                CardRelations_Cmc6 = GetCmc(6);
                CardRelations_Cmc7 = GetCmc(7);
                OnPropertyChanged(nameof(CardRelations_Cmc0));
                OnPropertyChanged(nameof(CardRelations_Cmc1));
                OnPropertyChanged(nameof(CardRelations_Cmc2));
                OnPropertyChanged(nameof(CardRelations_Cmc3));
                OnPropertyChanged(nameof(CardRelations_Cmc4));
                OnPropertyChanged(nameof(CardRelations_Cmc5));
                OnPropertyChanged(nameof(CardRelations_Cmc6));
                OnPropertyChanged(nameof(CardRelations_Cmc7));
                IsLoading = false;
            }
            catch (Exception ex) { logger.LogError(ex.Message); }
            finally { IsLoading = false; }
        }

        #region methods

        private async Task RetrieveCardData()
        {
            cardData.Clear();
            var cardRelations = CardRelations_Content;
            cardRelations.AddRange(CardRelations_Commandant);
            foreach (var cardRelation in cardRelations)
            {
                var data = await mageek.FindCard_Data(cardRelation.CardUuid);
                cardData.Add(cardRelation, data);
            }
        }

        private List<DeckCard> GetCmc(int cmc)
        {
            List<DeckCard> deckCards = new();
            foreach (var cardData in cardData)
            {
                if (cardData.Value.FaceConvertedManaCost == cmc)
                {
                    deckCards.Add(cardData.Key);
                }
            }
            return deckCards;
        }

        [RelayCommand]
        private async Task ResizeComplete()
        {
            currentCardSize = CardSize_Complete;
            FullRefresh();
        }

        [RelayCommand]
        private async Task ResizePicture()
        {
            currentCardSize = CardSize_Picture;
            FullRefresh();
        }

        [RelayCommand]
        private async Task ResizeHeader()
        {
            currentCardSize = CardSize_Header;
            FullRefresh();
        }

        #endregion

        [RelayCommand]
        private async Task SetCommandant_Click(DeckCard cr)
        {
            await mageek.ChangeDeckRelationType(cr, 1);
        }

        [RelayCommand]
        private async Task UnsetCommandant_Click(DeckCard cr)
        {
            await mageek.ChangeDeckRelationType(cr, 0);
        }

        [RelayCommand]
        private async Task ToSide_Click(DeckCard cr)
        {
            await mageek.ChangeDeckRelationType(cr, 2);
        }

        [RelayCommand]
        private async Task AddOne_Click(DeckCard cr)
        {
            await mageek.AddCardToDeck(cr.CardUuid, CurrentDeck, 1);
        }

        [RelayCommand]
        private async Task RemoveOne_Click(DeckCard cr)
        {
            await mageek.RemoveCardFromDeck(cr.CardUuid, CurrentDeck);
        }

    }

}

﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{

    public partial class DeckTableViewModel : BaseViewModel
    {

        readonly Dictionary<DeckCard, Cards> cardData = new();

        const int CardSize_Complete = 207;
        const int CardSize_Picture = 130;
        const int CardSize_Header = 25;

        public DeckTableViewModel(AppEvents events)
        {
            events.SelectDeckEvent += HandleDeckSelected;
            events.UpdateDeckEvent += HandleDeckModified;
        }

        #region Attributes

        [ObservableProperty] Deck currentDeck;
        [ObservableProperty] List<DeckCard> cardRelations_Commandant;
        [ObservableProperty] List<DeckCard> cardRelations_Content;
        [ObservableProperty] List<DeckCard> cardRelations_Side;
        [ObservableProperty] List<DeckCard> cardRelations_Lands;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc0;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc1;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc2;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc3;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc4;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc5;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc6;
        [ObservableProperty] List<DeckCard> cardRelations_Cmc7;
        [ObservableProperty] bool hasCommandant;
        [ObservableProperty] bool hasSide;
        [ObservableProperty] bool hasLands;

        #region TableState

        int currentCardSize = 130;
        public int CurrentCardSize
        {
            get { return currentCardSize; }
            set { currentCardSize = value; OnPropertyChanged(); }
        }

        #endregion

        #region Visibilitie

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive
        {
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #endregion

        #region CTOR

        #endregion

        #region Events

        private void ConfigureEvents()
        {
        }

        void HandleDeckSelected(string deck)
        {
            CurrentDeck = MageekService.MageekService.GetDeck(deck).Result;
        }

        void HandleDeckModified()
        {
            Deck tmp = CurrentDeck;
            CurrentDeck = null;
            CurrentDeck = tmp;
        }

        #endregion

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
                IsLoading = Visibility.Visible;
                OnPropertyChanged(nameof(IsActive));
                CardRelations_Content = await MageekService.MageekService.GetDeckContent_Related(CurrentDeck.DeckId, 0);
                CardRelations_Commandant = await MageekService.MageekService.GetDeckContent_Related(CurrentDeck.DeckId, 1);
                CardRelations_Side = await MageekService.MageekService.GetDeckContent_Related(CurrentDeck.DeckId, 2);
                CardRelations_Lands = await MageekService.MageekService.GetDeckContent_Typed(CurrentDeck.DeckId, "Land");
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
                IsLoading = Visibility.Collapsed;
            }
            catch (Exception ex) { Logger.Log(ex); }
            finally { IsLoading = Visibility.Collapsed; }
        }

        #region methods

        private async Task RetrieveCardData()
        {
            cardData.Clear();
            var cardRelations = CardRelations_Content;
            cardRelations.AddRange(CardRelations_Commandant);
            foreach (var cardRelation in cardRelations)
            {
                var data = await MageekService.MageekService.FindCard_Data(cardRelation.CardUuid);
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
            await MageekService.MageekService.ChangeDeckRelationType(cr, 1);
        }

        [RelayCommand]
        private async Task UnsetCommandant_Click(DeckCard cr)
        {
            await MageekService.MageekService.ChangeDeckRelationType(cr, 0);
        }

        [RelayCommand]
        private async Task ToSide_Click(DeckCard cr)
        {
            await MageekService.MageekService.ChangeDeckRelationType(cr, 2);
        }

        [RelayCommand]
        private async Task AddOne_Click(DeckCard cr)
        {
            await MageekService.MageekService.AddCardToDeck(cr.CardUuid, CurrentDeck, 1);
        }

        [RelayCommand]
        private async Task RemoveOne_Click(DeckCard cr)
        {
            await MageekService.MageekService.RemoveCardFromDeck(cr.CardUuid, CurrentDeck);
        }

    }

}

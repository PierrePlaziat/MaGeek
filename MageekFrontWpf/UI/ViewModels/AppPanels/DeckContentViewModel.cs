﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekServices.Data.Collection.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{
    public partial class DeckContentViewModel : BaseViewModel
    {

        private MageekServices.MageekService mageek;

        public DeckContentViewModel(MageekServices.MageekService mageek)
        {
            this.mageek = mageek;
        }

        [ObservableProperty] Deck currentDeck = null;
        [ObservableProperty] List<DeckCard> deckCards = new List<DeckCard>();
        [ObservableProperty] IEnumerable<DeckCard> currentCommanders;
        [ObservableProperty] IEnumerable<DeckCard> currentSide;
        [ObservableProperty] IEnumerable<DeckCard> currentCreatures;
        [ObservableProperty] IEnumerable<DeckCard> currentInstants;
        [ObservableProperty] IEnumerable<DeckCard> currentSorceries;
        [ObservableProperty] IEnumerable<DeckCard> currentEnchantments;
        [ObservableProperty] IEnumerable<DeckCard> currentArtifacts;
        [ObservableProperty] IEnumerable<DeckCard> currentPlaneswalkers;
        [ObservableProperty] IEnumerable<DeckCard> currentLands;
        [ObservableProperty] string filterString = string.Empty;
        [ObservableProperty] bool hasCommander;
        [ObservableProperty] bool hasSide;
        [ObservableProperty] bool isActive;
        [ObservableProperty] bool isLoading = false;

        #region Loading

        private async void HandleDeckSelected(string deck)
        {
            IsLoading = true;
            CurrentDeck = await mageek.GetDeck(deck);
            await HardReloadAsync();
        }

        private async void HandleDeckModif()
        {
            await SoftReloadAsync();
        }

        private async Task HardReloadAsync()
        {
            IsLoading = true;
            DeckCards = await mageek.GetDeckContent(CurrentDeck.DeckId);
            await SoftReloadAsync();
            IsLoading = false;
        }

        private async Task SoftReloadAsync()
        {
            IsLoading = true;
            await Task.WhenAll(
                new List<Task>
                {
                    GetCurrentCommanders(),
                    GetCurrentSide(),
                    GetCurrentCreatures(),
                    GetCurrentInstants(),
                    GetCurrentSorceries(),
                    GetCurrentEnchantments(),
                    GetCurrentArtifacts(),
                    GetCurrentPlaneswalkers(),
                    GetCurrentLands(),
                    GetHasCommander(),
                    GetHasSide()
                }
            );
            IsLoading = false;
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentCommanders()
        { 
            return DeckCards.Where(card => card.RelationType == 1); 
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentCreatures()
        { 
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Creature"));
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentInstants()
        {
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Instant"));
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentSorceries() 
        { 
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Sorcery")); 
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentEnchantments() 
        {
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Enchantment")); 
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentArtifacts() 
        { 
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Artifact"));
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentPlaneswalkers() 
        {
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Planeswalker"));
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentLands()
        {
            return DeckCards.Where(card => card.RelationType == 0 && card.Type.Contains("Land"));
        }

        private async Task<IEnumerable<DeckCard>> GetCurrentSide() 
        { 
            return DeckCards.Where(card => card.RelationType == 2);
        }

        private async Task<bool> GetHasCommander()
        {
            if (CurrentCommanders == null) return false;
            if (CurrentCommanders.ToList().Count <= 0) return false;
            else return true;
        }

        private async Task<bool> GetHasSide()
        {
            if (CurrentCommanders == null) return false;
            if (CurrentSide.ToList().Count <= 0) return false;
            else return true;
        }

        //private IEnumerable<DeckCard> ApplyFilter(IEnumerable<DeckCard> cards)
        //{
        //    return cards.Where(
        //            card => card.Card.Name.ToLower().Contains(FilterString.ToLower())
        //                 || card.Card.CardForeignName.ToLower().Contains(FilterString.ToLower())
        //    );
        //}

        #endregion

        [RelayCommand]
        private void LessCard(Button b)
        {
            var cr = b.DataContext as DeckCard;
            mageek.RemoveCardFromDeck(cr.CardUuid, CurrentDeck).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

        [RelayCommand]
        private void MoreCard(Button b)
        {
            var cr = b.DataContext as DeckCard;
            mageek.AddCardToDeck(cr.CardUuid, CurrentDeck, 1).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

        [RelayCommand]
        private void SetCommandant(DeckCard cardRel)
        {
            mageek.ChangeDeckRelationType(cardRel, 1).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

        [RelayCommand]
        private void UnsetCommandant(DeckCard cardRel)
        {
            mageek.ChangeDeckRelationType(cardRel, 0).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

        [RelayCommand]
        private void ToSide(DeckCard cardRel)
        {
            mageek.ChangeDeckRelationType(cardRel, 2).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

        [RelayCommand]
        private void ToDeck(DeckCard cardRel)
        {
            mageek.ChangeDeckRelationType(cardRel, 0).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(CurrentDeck.DeckId));
        }

    }
}

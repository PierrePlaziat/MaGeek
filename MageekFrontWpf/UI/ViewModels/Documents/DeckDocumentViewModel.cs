using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekCore;
using MageekFrontWpf.Framework.BaseMvvm;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels
{

    public partial class DeckDocumentViewModel : ObservableViewModel, 
        IRecipient<AddCardToDeckMessage>
    {

        private MageekService mageek;

        public DeckDocumentViewModel(MageekService mageek)
        {
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] OpenedDeck deck;
        [ObservableProperty] string filter = string.Empty;
        [ObservableProperty] bool isLoading;

        public async Task Initialize(Framework.AppValues.DocumentArguments args)
        {
            IsLoading = true;
            OpenedDeck d = new OpenedDeck(mageek);
            if (args.deck != null) await d.Initialize(args.deck).ConfigureAwait(false);
            else if (args.preco != null) await d.Initialize(args.preco).ConfigureAwait(false);
            else if (args.import != null) await d.Initialize(args.import).ConfigureAwait(false);
            Deck = d;
            IsLoading = false;
        }

        #region Manipulate Deck

        public void Receive(AddCardToDeckMessage message)
        {
            if ( Deck.Header.DeckId == message.Value.Item1)
            {
                Deck.AddCard(message.Value.Item2).ConfigureAwait(false);
                OnPropertyChanged(nameof(Deck));
            }
        }

        [RelayCommand]
        public async Task SaveDeck()
        {
            Deck.Header.DeckColors = Deck.DetermineColors();
            await mageek.SaveDeck(Deck.Header,Deck.GetLines());
            WeakReferenceMessenger.Default.Send(new UpdateDeckListMessage(Deck.Header.DeckId));
        }

        [RelayCommand]
        public async Task LessCard(OpenedDeckEntry entry)
        {
            Deck.LessCards(entry);
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public async Task MoreCard(OpenedDeckEntry entry)
        {
            Deck.MoreCard(entry);
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToCommandant(OpenedDeckEntry entry)
        {
            entry.Line.RelationType = 1;
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToSide(OpenedDeckEntry entry)
        {
            entry.Line.RelationType = 2;
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToDeck(OpenedDeckEntry entry)
        {
            entry.Line.RelationType = 0;
            OnPropertyChanged(nameof(Deck));
        }

        #endregion

        #region UI 

        //TODO Make a flying hand system
        #region Hand

        [ObservableProperty] List<string> hand;
        List<int> alreadyDrawed;
        readonly Random random = new();

        private void DrawNewHand()
        {
            Hand = new List<string>();
            alreadyDrawed = new List<int>();
            for (int i = 0; i < 7; i++) DrawNewCard();
        }

        private async void DrawNewCard()
        {
            string newCard = await DoDraw();
            if (newCard != null)
            {
                Hand.Add(newCard);
            }
        }

        private async Task<string> DoDraw()
        {
            if (Deck.Header == null) return null;
            if (Deck.Header.CardCount <= alreadyDrawed.Count) return null;
            int rgn;
            do { rgn = random.Next(Deck.Header.CardCount); }
            while (alreadyDrawed.Contains(rgn));
            //TODO
            await Task.Delay(0);
            return "";
            //return await Mageek.GetDeckContent(currentDeck.DeckId)[rgn].;
        }

        private void DrawNewHandButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewHand();
        }

        private void DrawCardButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewCard();
        }

        #endregion

        //TODO Scroll Zoom
        #region CardSize

        [ObservableProperty] private int currentCardSize;
        const int CardSize_Complete = 207;
        const int CardSize_Picture = 130;
        const int CardSize_Header = 25;

        [RelayCommand]
        private async Task ResizeComplete()
        {
            CurrentCardSize = CardSize_Complete;
        }

        [RelayCommand]
        private async Task ResizePicture()
        {
            CurrentCardSize = CardSize_Picture;
        }

        [RelayCommand]
        private async Task ResizeHeader()
        {
            CurrentCardSize = CardSize_Header;
        }

        #endregion

        #endregion

    }

}

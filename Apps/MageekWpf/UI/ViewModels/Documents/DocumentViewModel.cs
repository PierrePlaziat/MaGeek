using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.MageekTools.DeckTools;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekFrontWpf.Framework;

namespace MageekFrontWpf.UI.ViewModels
{

    public partial class DocumentViewModel : ObservableViewModel, 
        IRecipient<AddCardToDeckMessage>
    {

        private IMageekService mageek;

        public DocumentViewModel(IMageekService mageek)
        {
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] ManipulableDeck deck;
        [ObservableProperty] bool isLoading;

        public async Task OpenDocument(DocumentArguments args)
        {
            IsLoading = true;
            ManipulableDeck deck = ServiceHelper.GetService<ManipulableDeck>();
            if (args.deck != null) 
                await deck.OpenDeck(args.deck).ConfigureAwait(false);
            else if (args.preco != null) 
                await deck.OpenDeck(args.preco).ConfigureAwait(false);
            else if (args.import != null) 
                await deck.OpenDeck(args.import).ConfigureAwait(false);
            Deck = deck;
            IsLoading = false;
        }

        public void Receive(AddCardToDeckMessage message)
        {
            if (Deck.Header.DeckId == message.Value.Item1)
            {
                AddCardToDeck(message.Value.Item2).ConfigureAwait(false);
            }
        }

        [RelayCommand]
        public async Task SaveDeck()
        {
            await Deck.SaveDeck();
        }

        [RelayCommand]
        public async Task AddCardToDeck(string cardUuid)
        {
            await Deck.AddCard(cardUuid);
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public async Task LessCard(ManipulableDeckEntry entry)
        {
            Deck.LessOf(entry);
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public async Task MoreCard(ManipulableDeckEntry entry)
        {
            Deck.MoreOf(entry);
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToCommandant(ManipulableDeckEntry entry)
        {
            entry.Line.RelationType = 1;
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToSide(ManipulableDeckEntry entry)
        {
            entry.Line.RelationType = 2;
            OnPropertyChanged(nameof(Deck));
        }

        [RelayCommand]
        public void ToDeck(ManipulableDeckEntry entry)
        {
            entry.Line.RelationType = 0;
            OnPropertyChanged(nameof(Deck));
        }

    }

}

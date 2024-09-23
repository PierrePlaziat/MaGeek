using CommunityToolkit.Mvvm.Input;
using MageekCore.Data.Collection.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.UI.Controls;
using CommunityToolkit.Mvvm.Messaging;
using MageekDesktopClient.Framework;
using PlaziatWpf.Services;
using System;

namespace MageekDesktopClient.UI.ViewModels.AppWindows
{

    public partial class PrintWindowViewModel : ObservableViewModel,
        IRecipient<PrintDeckMessage>
    {

        private IMageekService mageek;
        private WindowsService wins;
        private SessionBag bag;

        public PrintWindowViewModel(IMageekService mageek, WindowsService wins, SessionBag bag)
        {
            this.mageek = mageek;
            this.wins = wins;
            this.bag = bag;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] Deck selectedDeck;
        [ObservableProperty] List<PrintingPage> pages = new();
        [ObservableProperty] PrintingPage showedPage = null;
        [ObservableProperty] bool includeBasicLands = false;
        [ObservableProperty] private bool onlyMissing = false;
        [ObservableProperty] int currentPage;
        [ObservableProperty] List<string> listOfCardsToPrint;

        public void Receive(PrintDeckMessage message)
        {
            Reload(message.Value).ConfigureAwait(false);
        }
        
        public async Task Reload(string deckkId)
        {
            CurrentPage = 0;
            SelectedDeck = await mageek.Decks_Get(bag.UserName, deckkId);
            ListOfCardsToPrint = await DetermineListOfCardsToPrint(SelectedDeck);
            Pages = await MakePrintingPages(SelectedDeck);
            ShowedPage = Pages.Count > 0 ? Pages[0] : null;
            ShowedPage.Refresh();
        }

        private async Task<List<string>> DetermineListOfCardsToPrint(Deck deck)
        {
            List<string> cards = new List<string>();
            foreach (var v in await mageek.Decks_Content(bag.UserName,deck.DeckId))
            {
                //if (IncludeBasicLands || !await mageek.CardHasType(v.CardUuid, "Basic Land"))
                {
                    //if (!OnlyMissing || await mageek.Collected(v.CardUuid) == 0)
                    {
                        for (int i = 0; i < v.Quantity; i++)
                        {
                            cards.Add(v.CardUuid);
                        }
                    }
                }
            }
            return cards;
        }

        private async Task<List<PrintingPage>> MakePrintingPages(Deck deck)
        {
            List<PrintingPage> newpages = new List<PrintingPage>();
            for (int page = 0; page <= ListOfCardsToPrint.Count / 9 + 1; page++)
            {
                newpages.Add(await GeneratePage(deck, page));
            }
            return newpages;
        }

        private async Task<PrintingPage> GeneratePage(Deck deck,int pageNb)
        {
            PrintingPage printing = new();
            for (int emplacement = 0; emplacement < 9; emplacement++)
            {
                string cardUuid = null;
                if (9 * pageNb + emplacement < deck.CardCount) cardUuid = ListOfCardsToPrint[9 * pageNb + emplacement];
                if (cardUuid != null) await printing.SetCard(cardUuid, emplacement);
            }
            return printing;
        }

        private void Print(Deck deck, System.Windows.Controls.PrintDialog printer)
        {
            foreach (var page in Pages)
            {
                printer.PrintVisual(page.ContentToPrint, "Mageek Proxy : " + deck.Title + " - page " + page);
            }
        }

        [RelayCommand]
        private async Task PreviousPage()
        {
            CurrentPage--;
            if (CurrentPage < 0) CurrentPage = Pages.Count - 2;
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[CurrentPage];
            }
        }

        [RelayCommand]
        private async Task NextPage()
        {
            CurrentPage++;
            if (CurrentPage == Pages.Count - 1) CurrentPage = 0;
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[CurrentPage];
            }
        }

        [RelayCommand]
        private async Task LaunchPrint()
        {
            System.Windows.Controls.PrintDialog printer = new();
            if (printer.ShowDialog() == true) Print(SelectedDeck, printer);
        }
        
        [RelayCommand]
        private async Task CheckBoxIncludeBasicLands()
        {
            IncludeBasicLands = !IncludeBasicLands;
            await Reload(SelectedDeck.DeckId);
        }

        [RelayCommand]
        private async Task CheckBoxOnlyMissing()
        {
            OnlyMissing = !OnlyMissing;
            await Reload(SelectedDeck.DeckId);
        }

    }

}

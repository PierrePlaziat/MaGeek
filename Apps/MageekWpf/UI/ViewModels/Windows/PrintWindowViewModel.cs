using CommunityToolkit.Mvvm.Input;
using MaGeek.UI.Controls;
using MageekCore.Data.Collection.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Services;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class PrintWindowViewModel : ObservableViewModel
    {

        private IMageekService mageek;

        public PrintWindowViewModel(IMageekService mageek)
        {
            this.mageek = mageek;
        }

        [ObservableProperty] Deck selectedDeck;
        [ObservableProperty] List<PrintingPage> pages = new();
        [ObservableProperty] PrintingPage showedPage = null;
        [ObservableProperty] bool includeBasicLands = false;
        [ObservableProperty] private bool onlyMissing = false;
        [ObservableProperty] int currentPage;
        [ObservableProperty] List<string> listOfCardsToPrint;

        [RelayCommand]
        private async Task Reload(Deck deck)
        {
            Pages.Clear();
            ShowedPage = null;
            CurrentPage = 0;
            SelectedDeck = deck;
            await DetermineListOfCardsToPrint(deck);
            await MakePrintingPages(deck);
        }
        private async Task DetermineListOfCardsToPrint(Deck deck)
        {
            //TODO
            ListOfCardsToPrint = new List<string>();
            //foreach (var v in await mageek.GetDeckContent(deck.DeckId))
            //{
            //    if (IncludeBasicLands || !await mageek.CardHasType(v.CardUuid, "Basic Land"))
            //    {
            //        if (!OnlyMissing || await mageek.Collected(v.CardUuid) == 0)
            //        {
            //            for (int i = 0; i < v.Quantity; i++)
            //            {
            //                ListOfCardsToPrint.Add(v.CardUuid);
            //            }
            //        }
            //    }
            //}
        }
        private async Task MakePrintingPages(Deck deck)
        {
            for (int page = 0; page <= ListOfCardsToPrint.Count / 9 + 1; page++)
            {
                Pages.Add(await GeneratePage(deck, page));
            }
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[0];
            }
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
            PrintDialog printer = new();
            if (printer.ShowDialog() == true) Print(SelectedDeck, printer);
        }
        private void Print(Deck deck, PrintDialog printer)
        {
            foreach (var page in Pages)
            {
                printer.PrintVisual(page.ContentToPrint, "Mageek Proxy : " + deck.Title + " - page " + page);
            }
        }


        [RelayCommand]
        private async Task CheckBoxIncludeBasicLands()
        {
            IncludeBasicLands = !IncludeBasicLands;
            await Reload(SelectedDeck);
        }

        [RelayCommand]
        private async Task CheckBoxOnlyMissing()
        {
            OnlyMissing = !OnlyMissing;
            await Reload(SelectedDeck);
        }

    }

}

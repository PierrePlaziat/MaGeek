using CommunityToolkit.Mvvm.Input;
using MaGeek.UI.Controls;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekServices.Data.Collection.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public class PrintViewModel : BaseViewModel
    {

        private MageekServices.MageekService mageek;

        public PrintViewModel(MageekServices.MageekService mageek)
        {
            this.mageek = mageek;
            if (selectedDeck != null)
            {
                SelectedDeck = selectedDeck;
                DelayLoad().ConfigureAwait(false);
            }

        }

        public ICommand PreviousPageCommand { get; } = new AsyncRelayCommand(DoPreviousPageCommand);
        public ICommand NextPageCommand { get; } = new AsyncRelayCommand(DoNextPageCommand);
        public ICommand LaunchPrintCommand { get; } = new AsyncRelayCommand(DoLaunchPrintCommand);


        private static Task DoPreviousPageCommand()
        {
            throw new NotImplementedException();
        }
        private static Task DoNextPageCommand()
        {
            throw new NotImplementedException();
        }
        private static Task DoLaunchPrintCommand()
        {
            throw new NotImplementedException();
        }

        Deck selectedDeck;
        public Deck SelectedDeck
        {
            get { return selectedDeck; }
            set { selectedDeck = value; OnPropertyChanged(); }
        }

        List<string> ListOfCardsToPrint;
        List<PrintingPage> Pages = new();
        PrintingPage ShowedPage;

        private int currentPage;

        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentPageP1)); }
        }

        public int CurrentPageP1
        {
            get { return currentPage + 1; }
        }

        private bool IncludeBasicLands = false;
        private bool OnlyMissing = false;

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            await Reload();
        }

        private async Task Reload()
        {
            Pages.Clear();
            ShowedPage = null;
            CurrentPage = 0;
            await DetermineListOfCardsToPrint();
            for (int page = 0; page <= ListOfCardsToPrint.Count / 9 + 1; page++)
            {
                Pages.Add(await GeneratePage(page));
            }
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[0];
            }
        }

        private async Task DetermineListOfCardsToPrint()
        {
            ListOfCardsToPrint = new List<string>();
            foreach (var v in await mageek.GetDeckContent(selectedDeck.DeckId))
            {
                ;
                if (IncludeBasicLands || !await mageek.CardHasType(v.CardUuid, "Basic Land"))
                {
                    if (!OnlyMissing || await mageek.Collected(v.CardUuid) == 0)
                    {
                        for (int i = 0; i < v.Quantity; i++)
                        {
                            ListOfCardsToPrint.Add(v.CardUuid);
                        }
                    }
                }
            }
        }

        private async Task<PrintingPage> GeneratePage(int pageNb)
        {
            PrintingPage printing = new();
            for (int emplacement = 0; emplacement < 9; emplacement++)
            {
                string cardUuid = null;
                if (9 * pageNb + emplacement < selectedDeck.CardCount) cardUuid = ListOfCardsToPrint[9 * pageNb + emplacement];
                if (cardUuid != null) await printing.SetCard(cardUuid, emplacement);
            }
            return printing;
        }

        private void Print(PrintDialog printer)
        {
            foreach (var page in Pages)
            {
                printer.PrintVisual(page.ContentToPrint, "Mageek Proxy : " + selectedDeck.Title + " - page " + page);
            }
        }

        private void LaunchPrint(object sender, System.Windows.RoutedEventArgs e)
        {
            PrintDialog printer = new();
            if (printer.ShowDialog() == true)
            {
                Print(printer);
            }
        }

        private void NextPageButton(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentPage++;
            if (CurrentPage == Pages.Count - 1) CurrentPage = 0;
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[CurrentPage];
            }
        }

        private void PreviousPageButton(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentPage--;
            if (CurrentPage < 0) CurrentPage = Pages.Count - 2;
            if (Pages.Count > 0)
            {
                ShowedPage = Pages[CurrentPage];
            }
        }

        private void CheckBox_IncludeBasicLands(object sender, System.Windows.RoutedEventArgs e)
        {
            IncludeBasicLands = ((CheckBox)sender).IsChecked ?? false;
            Reload().ConfigureAwait(false);
        }

        private void CheckBox_OnlyMissing(object sender, System.Windows.RoutedEventArgs e)
        {
            OnlyMissing = ((CheckBox)sender).IsChecked ?? false;
            Reload().ConfigureAwait(false);
        }

    }

}

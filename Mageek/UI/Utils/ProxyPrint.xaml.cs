using MaGeek.AppBusiness;
using MaGeek.Entities;
using MaGeek.UI.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace MaGeek.UI.Windows.ImportExport
{

    public partial class ProxyPrint : TemplatedWindow, INotifyPropertyChanged
    {

        Deck selectedDeck;
        public  Deck SelectedDeck
        {
            get { return selectedDeck; }
            set { selectedDeck = value; OnPropertyChanged(); }
        }

        List<CardVariant> ListOfCardsToPrint;
        List<PrintingPage> Pages = new();

        private int currentPage;

        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged();OnPropertyChanged(nameof(CurrentPageP1)); }
        }
        
        public int CurrentPageP1
        {
            get { return currentPage+1; }
        }

        private bool IncludeBasicLands = false;
        private bool OnlyMissing = false;


        public ProxyPrint(Deck selectedDeck)
        {
            InitializeComponent();
            DataContext = this;
            if (selectedDeck != null)
            {
                SelectedDeck = selectedDeck;
                DelayLoad().ConfigureAwait(false);
            }
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            await Reload();
        }

        private async Task Reload()
        {
            Pages.Clear();
            ShowedPage.Children.Clear();
            CurrentPage = 0;
            DetermineListOfCardsToPrint();
            for (int page = 0; page <= ListOfCardsToPrint.Count / 9+1; page++)
            {
                Pages.Add(await GeneratePage(page));
            }
            if (Pages.Count > 0)
            {
                ShowedPage.Children.Add(Pages[0]);
            }
        }

        private void DetermineListOfCardsToPrint()
        {
            ListOfCardsToPrint= new List<CardVariant>();
            foreach(var v in selectedDeck.DeckCards)
            {
                if (IncludeBasicLands || !v.Card.Card.Type.Contains("Basic Land"))
                {
                    if (!OnlyMissing || MageekCollection.GotCard_HaveOne(v.Card.Card).Result==0)
                    {
                        for (int i = 0; i < v.Quantity; i++)
                        {
                            ListOfCardsToPrint.Add(v.Card);
                        }
                    }
                }
            }
        }

        private async Task<PrintingPage> GeneratePage(int pageNb)
        {
            PrintingPage printing = new PrintingPage();
            for (int emplacement = 0; emplacement < 9; emplacement++)
            {
                CardVariant c = null;
                if (9 * pageNb + emplacement < selectedDeck.CardCount) c = ListOfCardsToPrint[9 * pageNb + emplacement];
                if (c != null) await printing.SetCard(c, emplacement);
            }
            return printing;
        }

        private async Task Print(PrintDialog printer)
        {
            foreach (var page in Pages)
            {
                printer.PrintVisual(page.ContentToPrint, "Mageek Proxy : " + selectedDeck.Title + " - page " + page);
            }
        }

        private async void LaunchPrint(object sender, System.Windows.RoutedEventArgs e)
        {
            PrintDialog printer = new PrintDialog();
            if (printer.ShowDialog() == true)
            {
                await Print(printer);
            }
        }

        private void NextPageButton(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentPage++;
            if (CurrentPage == Pages.Count-1) CurrentPage = 0;
            if (Pages.Count > 0)
            {
                ShowedPage.Children.Clear();
                ShowedPage.Children.Add(Pages[CurrentPage]);
            }
        }

        private void PreviousPageButton(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentPage--;
            if (CurrentPage < 0) CurrentPage = Pages.Count-2;
            if (Pages.Count > 0)
            {
                ShowedPage.Children.Clear();
                ShowedPage.Children.Add(Pages[CurrentPage]);
            }
        }

        private void CheckBox_IncludeBasicLands(object sender, System.Windows.RoutedEventArgs e)
        {
            IncludeBasicLands = ((CheckBox)sender).IsChecked.HasValue? ((CheckBox)sender).IsChecked.Value : false;
            Reload().ConfigureAwait(false);
        }

        private void CheckBox_OnlyMissing(object sender, System.Windows.RoutedEventArgs e)
        {
            OnlyMissing = ((CheckBox)sender).IsChecked.HasValue ? ((CheckBox)sender).IsChecked.Value : false;
            Reload().ConfigureAwait(false);
        }
    }

}
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaGeek.Entities;
using MaGeek.UI.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using PrintDialog = System.Windows.Controls.PrintDialog;
using System.Windows.Forms.VisualStyles;

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


        private int pageCount;
        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; OnPropertyChanged(); }
        }

        public ProxyPrint(Deck selectedDeck)
        {
            InitializeComponent();
            DataContext = this;
            if (selectedDeck != null) 
            {
                SelectedDeck = selectedDeck;
                DetermineListOfCardsToPrint();
                DelayLoad().ConfigureAwait(false);
            }
        }

        private void DetermineListOfCardsToPrint()
        {
            ListOfCardsToPrint= new List<CardVariant>();
            foreach(var v in selectedDeck.DeckCards)
            {
                for (int i = 0; i < v.Quantity; i++)
                {
                    ListOfCardsToPrint.Add(v.Card);
                }
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
            CurrentPage = 0;
            PageCount = SelectedDeck.CardCount / 9;
            for (int page = 0; page <= PageCount; page++)
            {
                Pages.Add(await GeneratePage(page));
            }
            if (Pages.Count > 0)
            {
                ShowedPage.Children.Add(Pages[0]);
                //ShowedPage.InvalidateVisual();
                //Pages[0].Refresh();
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
                //ShowedPage.InvalidateVisual();
                //Pages[CurrentPage].Refresh();
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
                //ShowedPage.InvalidateVisual();
                //Pages[CurrentPage].Refresh();
            }
        }
    }

}
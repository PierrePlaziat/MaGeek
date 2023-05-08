using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using MaGeek.AppData.Entities;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace MaGeek.UI.Windows.ImportExport
{

    public partial class ProxyPrint : Window, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        Deck selectedDeck;
        public  Deck SelectedDeck
        {
            get { return selectedDeck; }
            set { selectedDeck = value; OnPropertyChanged(); }
        }

        List<CardVariant> ListOfCardsToPrint;

        private BitmapImage card0;
        public BitmapImage Card0
        {
            get { return card0; }
            set { card0 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card1;
        public BitmapImage Card1
        {
            get { return card1; }
            set { card1 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card2;
        public BitmapImage Card2
        {
            get { return card2; }
            set { card2 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card3;
        public BitmapImage Card3
        {
            get { return card3; }
            set { card3 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card4;
        public BitmapImage Card4
        {
            get { return card4; }
            set { card4 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card5;
        public BitmapImage Card5
        {
            get { return card5; }
            set { card5 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card6;
        public BitmapImage Card6
        {
            get { return card6; }
            set { card6 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card7;
        public BitmapImage Card7
        {
            get { return card7; }
            set { card7 = value; OnPropertyChanged(); }
        }
        
        private BitmapImage card8;
        public BitmapImage Card8
        {
            get { return card8; }
            set { card8 = value; OnPropertyChanged(); }
        }


        public ProxyPrint(Deck _selectedDeck)
        {
            if (_selectedDeck != null) 
            {
                SelectedDeck= _selectedDeck;
                InitializeComponent();
                DataContext = this;
                DetermineListOfCardsToPrint();
                DelayLoad().ConfigureAwait(false);
            }
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            await LetsGo();
        }

        private void DetermineListOfCardsToPrint()
        {
            ListOfCardsToPrint= new List<CardVariant>();
            foreach(var v in selectedDeck.CardRelations)
            {
                for(int i=0;i<v.Quantity;i++) ListOfCardsToPrint.Add(v.Card);
            }
        }

        private async Task LetsGo()
        {
            for (int page = 0; page <= SelectedDeck.CardCount/9; page++)
            {
                Card0 = null;
                Card1 = null;
                Card2 = null;
                Card3 = null;
                Card4 = null;
                Card5 = null;
                Card6 = null;
                Card7 = null;
                Card8 = null;
                for(int emplacement=0;emplacement<9;emplacement++)
                {
                    await SetCard(page, emplacement);
                }
                await Task.Delay(1000);
                Print(page);
                await Task.Delay(1000);
            }
        }

        private async Task SetCard(int page,int emplacement)
        {
            CardVariant c = null;
            if (9 * page + emplacement < selectedDeck.CardCount) c = ListOfCardsToPrint[9 * page + emplacement];
            if (c == null) return;
            switch(emplacement)
            {
                case 0: Card0 = await c.RetrieveImage(); break;
                case 1: Card1 = await c.RetrieveImage(); break;
                case 2: Card2 = await c.RetrieveImage(); break;
                case 3: Card3 = await c.RetrieveImage(); break;
                case 4: Card4 = await c.RetrieveImage(); break;
                case 5: Card5 = await c.RetrieveImage(); break;
                case 6: Card6 = await c.RetrieveImage(); break;
                case 7: Card7 = await c.RetrieveImage(); break;
                case 8: Card8 = await c.RetrieveImage(); break;
            }
        }

        private void Print(int page)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
                printDialog.PrintVisual(yo, "Mageek Proxy : " + selectedDeck.Title+" - page " + page);
        }
    }

}
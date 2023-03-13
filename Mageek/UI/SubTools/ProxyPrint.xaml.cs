using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using MaGeek.AppData.Entities;

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

        MagicDeck selectedDeck;
        public  MagicDeck SelectedDeck
        {
            get { return selectedDeck; }
            set { selectedDeck = value; OnPropertyChanged(); }
        }

        List<MagicCardVariant> ListOfCardsToPrint;

        private MagicCardVariant card0;
        public MagicCardVariant Card0
        {
            get { return card0; }
            set { card0 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card1;
        public MagicCardVariant Card1
        {
            get { return card1; }
            set { card1 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card2;
        public MagicCardVariant Card2
        {
            get { return card2; }
            set { card2 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card3;
        public MagicCardVariant Card3
        {
            get { return card3; }
            set { card3 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card4;
        public MagicCardVariant Card4
        {
            get { return card4; }
            set { card4 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card5;
        public MagicCardVariant Card5
        {
            get { return card5; }
            set { card5 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card6;
        public MagicCardVariant Card6
        {
            get { return card6; }
            set { card6 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card7;
        public MagicCardVariant Card7
        {
            get { return card7; }
            set { card7 = value; OnPropertyChanged(); }
        }
        
        private MagicCardVariant card8;
        public MagicCardVariant Card8
        {
            get { return card8; }
            set { card1 = value; OnPropertyChanged(); }
        }


        public ProxyPrint(MagicDeck _selectedDeck)
        {
            if (_selectedDeck != null) 
            {
                SelectedDeck= _selectedDeck;
                InitializeComponent();
                DataContext = this;
                DetermineListOfCardsToPrint();
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true) LetsGo(printDialog);
            }
        }

        private void DetermineListOfCardsToPrint()
        {
            ListOfCardsToPrint= new List<MagicCardVariant>();
            foreach(var v in selectedDeck.CardRelations)
            {
                for(int i=0;i<v.Quantity;i++) ListOfCardsToPrint.Add(v.Card);
            }
        }

        private void LetsGo(PrintDialog printDialog)
        {
            SetCard(0, 0);
            for (int page = 0; page <= SelectedDeck.CardCount/9; page++)
            {
                for(int emplacement=0;emplacement<9;emplacement++)
                {
                    SetCard(page, emplacement);
                }
                Print(printDialog,page);
            }
        }

        private void SetCard(int page,int emplacement)
        {
            MagicCardVariant c = null;
            if (9 * page + emplacement < selectedDeck.CardCount) c = ListOfCardsToPrint[9 * page + emplacement]; 
            switch(emplacement)
            {
                case 0: Card0 = c; break;
                case 1: Card1 = c; break;
                case 2: Card2 = c; break;
                case 3: Card3 = c; break;
                case 4: Card4 = c; break;
                case 5: Card5 = c; break;
                case 6: Card6 = c; break;
                case 7: Card7 = c; break;
                case 8: Card8 = c; break;
            }
        }

        private void Print(PrintDialog printDialog,int page)
        {
            VisualBrush visualBrush = new VisualBrush(this); 
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), new Size(ActualWidth, ActualHeight)));
            }
            printDialog.PrintVisual(drawingVisual, "Mageek Proxy : " + selectedDeck.Title+" - page " + page);
        }
    }

}
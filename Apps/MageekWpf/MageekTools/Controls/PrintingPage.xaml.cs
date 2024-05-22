using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore.Data;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MageekFrontWpf.Framework.Services;
using MageekCore.Services;

namespace MaGeek.UI.Controls
{
    /// <summary>
    /// Logique d'interaction pour PrintingPage.xaml
    /// </summary>
    public partial class PrintingPage : BaseUserControl
    {

        private IMageekService mageek;

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

        public PrintingPage()
        {
            mageek = ServiceHelper.GetService<IMageekService>();
            InitializeComponent();
            DataContext = this;
        }

        public async Task SetCard(string cardUuid, int emplacement)
        {
            BitmapImage bmp = new BitmapImage(await mageek.Cards_GetIllustration(cardUuid, CardImageFormat.png));
            switch (emplacement)
            {
                case 0: Card0 = bmp; break;
                case 1: Card1 = bmp; break;
                case 2: Card2 = bmp; break;
                case 3: Card3 = bmp; break;
                case 4: Card4 = bmp; break;
                case 5: Card5 = bmp; break;
                case 6: Card6 = bmp; break;
                case 7: Card7 = bmp; break;
                case 8: Card8 = bmp; break;
            }
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Card0));
            OnPropertyChanged(nameof(Card1));
            OnPropertyChanged(nameof(Card2));
            OnPropertyChanged(nameof(Card3));
            OnPropertyChanged(nameof(Card4));
            OnPropertyChanged(nameof(Card5));
            OnPropertyChanged(nameof(Card6));
            OnPropertyChanged(nameof(Card7));
            OnPropertyChanged(nameof(Card8));
        }

    }

}

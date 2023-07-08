using MtgSqliveSdk;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{

    public partial class CardIllustration : TemplatedUserControl
    {

        #region Dependancy Properties

        public static readonly DependencyProperty CardUuidProperty = DependencyProperty.Register(
            "SelectedVariant", typeof(string), typeof(CardIllustration),
            new FrameworkPropertyMetadata( null, OnDefectIdChanged )
        );

        private static void OnDefectIdChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs) 
        {
            CardIllustration control = _control as CardIllustration;
            control.Reload(eventArgs.NewValue as string).ConfigureAwait(false);
        }

        #endregion

        #region Attributes

        private string cardUuid = null;
        public string CardUuid
        {

            get { return cardUuid; }
            set
            {
                cardUuid = value;
                OnPropertyChanged();
            }
        }

        private string cardface = null;
        public string CardFace
        {

            get { return cardface; }
            set
            {
                cardface = value;
                OnPropertyChanged();
            }
        }
        
        private string cardBack = null;
        public string CardBack
        {

            get { return cardBack; }
            set
            {
                cardBack = value;
                OnPropertyChanged();
            }
        }

        private bool hasBack = false;
        public bool HasBack
        {
            get { return hasBack; }
            set
            {
                hasBack = value;
                OnPropertyChanged();
            }
        }
        
        private bool showingBack = false;
        public bool ShowingBack
        {
            get { return showingBack; }
            set
            {
                showingBack = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage cardImage = null;
        public BitmapImage CardImage
        {
            get { return cardImage; }
            set { cardImage = value;OnPropertyChanged(); }
        }

        #endregion

        #region CTOR

        public CardIllustration(string cardUuid)
        {
            InitializeComponent();
            DataContext = this;
            Reload(cardUuid).ConfigureAwait(false);
        }

        public CardIllustration()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Methods

        private async Task Reload(string cardUuid)
        {
            SetValue(CardUuidProperty, cardUuid);
            if (!string.IsNullOrEmpty(cardUuid))
            {
                string otherFaceUuid= await Mageek.GetCardBack(cardUuid);
                if (!string.IsNullOrEmpty(otherFaceUuid))
                {
                    CardBack = otherFaceUuid;
                    HasBack = true;
                }
                else
                {
                    CardBack = null;
                    HasBack = false;
                }
            }
            else
            {
                CardFace = null;
                CardBack = null;
                HasBack = false;
            }
            ShowingBack = false;
            await ShowCard();
        }

        private void SwitchFaceClic(object sender, RoutedEventArgs e)
        {
            ShowingBack = !ShowingBack;
            ShowCard().ConfigureAwait(false);
        }
        private async Task ShowCard()
        {
            var url = await Mageek.RetrieveImage(ShowingBack ? CardBack : CardFace);
            CardImage = new BitmapImage(url);
        }


        #endregion

    }

}

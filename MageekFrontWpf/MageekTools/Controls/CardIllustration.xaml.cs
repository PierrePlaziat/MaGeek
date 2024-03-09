using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore.Data;
using MageekCore.Data.Mtg.Entities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MageekFrontWpf.Framework.Services;
using MageekCore.Service;
using System;

namespace MaGeek.UI
{

    public partial class CardIllustration : BaseUserControl
    {

        private IMageekService mageek;
        BitmapImage ImageDefault;

        public CardIllustration()
        {
            mageek = ServiceHelper.GetService<IMageekService>();
            DataContext = this;
            InitializeComponent();
            ImageDefault = new BitmapImage(new Uri("D:\\PROJECTS\\VS\\MaGeek\\MageekFrontWpf\\Resources\\Images\\cardback.jpg", UriKind.Absolute));
            SelectCard(null).ConfigureAwait(false);
        }

        private string cardUuid;
        public string CardUuid
        {
            get { return cardUuid; }
            set {
                cardUuid = value; 
                SetValue(CardUuidProperty, value); 
            }
        }

        public static readonly DependencyProperty CardUuidProperty = DependencyProperty.Register
        (
            nameof(CardUuid),typeof(string),typeof(CardIllustration),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnCardUuidChanged)
        );

        private Cards selectedCard;
        public Cards SelectedCard
        {
            get { return selectedCard; }
            set { selectedCard = value; OnPropertyChanged(); }
        }

        private BitmapImage selectedImage;
        public BitmapImage SelectedImage
        {
            get { return selectedImage; }
            set { selectedImage = value; OnPropertyChanged(); }
        }

        private Cards cardFront;
        public Cards CardFront
        {
            get { return cardFront; }
            set { cardFront = value; OnPropertyChanged(); }
        }
        
        private BitmapImage imageFront;
        public BitmapImage ImageFront
        {
            get { return imageFront; }
            set { imageFront = value; OnPropertyChanged(); }
        }
        
        private Cards cardBack;
        public Cards CardBack
        {
            get { return cardBack; }
            set { cardBack = value; OnPropertyChanged(); }
        }

        private BitmapImage imageBack;
        public BitmapImage ImageBack
        {
            get { return imageBack; }
            set { imageBack = value; OnPropertyChanged(); }
        }

        private bool flipped;

        public bool Flipped
        {
            get { return flipped; }
            set { flipped = value; }
        }


        private static void OnCardUuidChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs)
        {
            CardIllustration control = _control as CardIllustration;
            control.SelectCard(eventArgs.NewValue as string).ConfigureAwait(false);
        }

        private async Task SelectCard(string uuid)
        {
            Flipped = false;
            Cards cardFront = await mageek.FindCard_Data(uuid);
            Cards cardBack = null;
            if (cardFront != null && cardFront.OtherFaceIds != null)
            {
                string backUuid = cardFront.OtherFaceIds;
                cardBack = await mageek.FindCard_Data(backUuid);
            }
            CardBack = cardBack;
            try
            {
                ImageBack = new BitmapImage(await mageek.RetrieveImage(cardBack.Uuid, CardImageFormat.png));
                CardFront = cardFront;
            }
            catch 
            {
                ImageBack = ImageDefault;
            }
            try
            {
                ImageFront = new BitmapImage(await mageek.RetrieveImage(cardFront.Uuid, CardImageFormat.png));
                SelectedCard = cardFront;
            }
            catch 
            {
                ImageFront = ImageDefault;
            }
            SelectedImage = ImageFront;
        }

        private async Task LoadFront(Cards cardFront)
        {
        }

        private async Task LoadBack(Cards cardBack)
        {
        }


        private void FlipClick(object sender, RoutedEventArgs e)
        {
            flipped = !flipped;
            if (flipped)
            {
                SelectedCard = CardBack;
                SelectedImage = ImageBack;
            }
            else
            {
                SelectedCard = CardFront;
                SelectedImage = ImageFront;
            }
        }

        private bool showHud = false;

        public bool ShowHud
        {
            get { return showHud; }
            set { showHud = value; OnPropertyChanged(); }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHud = true;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ShowHud = false;
        }

    }

}

using MageekCore.Data;
using MageekCore.Data.Mtg.Entities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatTools;
using System.Windows.Controls;

namespace MageekDesktopClient.UI.Controls
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
            ImageDefault = new BitmapImage(new Uri("D:\\PROJECTS\\VS\\MaGeek\\VsProject\\Apps\\MageekDesktopClient\\Resources\\Images\\cardback.jpg", UriKind.Absolute)); //TODO softer please
            SelectCard(null).ConfigureAwait(false);
        }

        private string cardUuid;
        public string CardUuid
        {
            get { return cardUuid; }
            set
            {
                cardUuid = value;
                SetValue(CardUuidProperty, value);
            }
        }

        public static readonly DependencyProperty CardUuidProperty = DependencyProperty.Register
        (
            nameof(CardUuid), typeof(string), typeof(CardIllustration),
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
            if (uuid == null) return; //!\\ guard
            Flipped = false;
            Cards cardFront = await mageek.Cards_GetData(uuid);
            Cards cardBack = null;
            if (cardFront != null && cardFront.OtherFaceIds != null)
            {
                string backUuid = cardFront.OtherFaceIds;
                cardBack = await mageek.Cards_GetData(backUuid);
            }
            CardBack = cardBack;
            CardFront = cardFront;
            try
            {
                ImageBack = cardBack == null ? null: new BitmapImage(
                    await mageek.Cards_GetIllustration(
                        cardBack.Uuid,
                        CardImageFormat.png,
                        false
                    )
                );
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                ImageBack = ImageDefault;
            }
            try
            {
                var v = await mageek.Cards_GetIllustration(
                    cardFront.Uuid,
                    CardImageFormat.png,
                    true
                );
                ImageFront = new BitmapImage(v); 
            }
            catch (Exception e)
            {
                Logger.Log(e);
                ImageFront = ImageDefault;
            }
            SelectedCard = cardFront;
            SelectedImage = ImageFront;
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

        #region DragAndDrop support

        Grid lv;
        private Point _startPoint;
        private void UIElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            lv = (Grid)sender;
            if (lv == null) return;
        }
        private void UIElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(null);
                    Vector diff = _startPoint - currentPosition;
                    if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        DragDrop.DoDragDrop(lv, SelectedCard.Uuid, DragDropEffects.Move);
                    }
                }
            }
            catch { }
        }

        #endregion

    }

}

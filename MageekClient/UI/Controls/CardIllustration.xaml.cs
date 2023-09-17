using MageekSdk;
using MageekSdk.MtgSqlive.Entities;
using MtgSqliveSdk;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{

    public partial class CardIllustration : TemplatedUserControl
    {

        #region Attributes

        #region Dependancy Property

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "SelectedVariant", typeof(Cards), typeof(CardIllustration),
            new FrameworkPropertyMetadata(null, OnDefectIdChanged)
        );

        private static void OnDefectIdChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs)
        {
            CardIllustration control = _control as CardIllustration;
            control.SelectedVariant = eventArgs.NewValue as Cards;
        }

        #endregion

        #region Properties


        private Cards selectedVariant;
        public Cards SelectedVariant
        {
            get { return selectedVariant; }
            set
            {
                selectedVariant = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPower));
                ShowBack = false;
                OnPropertyChanged(nameof(HasBackFace));
                SetValue(CardProperty, value);
                if (SelectedVariant != null) LoadIllustration();
                else UnloadIllustration();
            }
        }

        private async void LoadIllustration()
        {
            cardImage = null;
            var url = await Mageek.RetrieveImage(selectedVariant.Uuid,CardImageFormat.png);
            if (url == null) return;
            CardImage = new BitmapImage(url);
        }
        private async void UnloadIllustration()
        {
            cardImage = null;
            CardImage = null;
        }

        private void CardImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CardImage));
        }

        //public CardModel SelectedCard { get { return SelectedVariant == null ? null : SelectedVariant.Card; } }

        private BitmapImage cardImage;
        public BitmapImage CardImage
        {
            get { return cardImage; }
            set
            {
                cardImage = value;
                OnPropertyChanged(nameof(CardImage));
            }
        }


        private Visibility isHudVisible = Visibility.Collapsed;
        public Visibility IsHudVisible
        {
            get { return isHudVisible; }
            set { isHudVisible = value; OnPropertyChanged(); }
        }

        #endregion

        #region Accessors

        public Visibility HasPower
        {
            get
            {
                if (selectedVariant == null)
                    return Visibility.Collapsed;
                return selectedVariant.Type.ToLower().Contains("creature") ?
                       Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Back Face

        bool showBack = false;
        public bool ShowBack
        {
            get { return showBack; }
            set { showBack = value; OnPropertyChanged(); }
        }

        public Visibility HasBackFace
        {
            get
            {
                if (SelectedVariant== null) return Visibility.Collapsed;
                return string.IsNullOrEmpty(SelectedVariant.OtherFaceIds) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion

        #endregion

        #region CTOR

        public CardIllustration(string uuid)
        {
            InitializeComponent();
            DataContext = this;
            SelectedVariant = Mageek.FindCard_Data(uuid).Result;
        }
        
        public CardIllustration(Cards card)
        {
            InitializeComponent();
            DataContext = this;
            SelectedVariant = card;
        }

        public CardIllustration()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Mouse Gestion

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            IsHudVisible = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            IsHudVisible = Visibility.Collapsed;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedVariant != null) App.Events.RaiseCardSelected(selectedVariant.Uuid);
        }

        #endregion

        private void SwitchFaceClic(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseCardSelected(SelectedVariant.OtherFaceIds);
        }

        public void ReLoad(Cards c)
        {
            SelectedVariant = c;
        }

    }

}

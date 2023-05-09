using MaGeek.AppData.Entities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{

    public partial class CardIllustration : UserControl, INotifyPropertyChanged
    {

        #region Attributes

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Dependancy Property

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "SelectedVariant", typeof(CardVariant), typeof(CardIllustration),
            new FrameworkPropertyMetadata( null, OnDefectIdChanged )
        );

        private static void OnDefectIdChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs) 
        {
            CardIllustration control = _control as CardIllustration;
            control.SelectedVariant = eventArgs.NewValue as CardVariant;
        }

        #endregion

        #region Properties


        private CardVariant selectedVariant;
        public CardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { 
                selectedVariant = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPower));
                OnPropertyChanged(nameof(SelectedCard));
                ShowBack = false;
                OnPropertyChanged(nameof(HasBackFace));
                SetValue(CardProperty, value);
                if (SelectedVariant != null)
                {
                    CardImage = new NotifyTaskCompletion<BitmapImage>(SelectedVariant.RetrieveImage(ShowBack));
                }
            }
        }

        private void CardImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CardImage.Result));
        }

        public CardModel SelectedCard { get { return SelectedVariant == null ? null:SelectedVariant.Card; } }

        private NotifyTaskCompletion<BitmapImage> cardImage;
        public NotifyTaskCompletion<BitmapImage> CardImage { 
            get { return cardImage; }
            set { 
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

        public Visibility HasPower { 
            get { 
                if (selectedVariant==null || selectedVariant.Card==null || selectedVariant.Card.Type==null)
                    return Visibility.Collapsed;
                return selectedVariant.Card.Type.ToLower().Contains("creature") ? 
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
                if (SelectedCard == null) return Visibility.Collapsed;
                return string.IsNullOrEmpty(SelectedCard.Variants[0].ImageUrl_Back) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion

        #endregion

        #region CTOR

        public CardIllustration(CardVariant card)
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
            if (selectedVariant != null) App.Events.RaiseCardSelected(selectedVariant.Card);
        }

        #endregion

        private void SwitchFaceClic(object sender, RoutedEventArgs e)
        {
            ShowBack = !showBack;
            CardImage = new NotifyTaskCompletion<BitmapImage>(SelectedVariant.RetrieveImage(ShowBack));
        }

        public void ReLoad(CardVariant c)
        {
            SelectedVariant = c;
        }

    }

}

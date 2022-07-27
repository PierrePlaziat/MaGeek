using MaGeek.Data.Entities;
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
            "SelectedVariant", typeof(MagicCardVariant), typeof(CardIllustration),
            new FrameworkPropertyMetadata( null, OnDefectIdChanged )
        );

        private static void OnDefectIdChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs) 
        {
            CardIllustration control = _control as CardIllustration;
            control.SelectedVariant = eventArgs.NewValue as MagicCardVariant;
        }

        #endregion

        #region Properties


        private MagicCardVariant selectedVariant;
        public MagicCardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { 
                selectedVariant = value;
                OnPropertyChanged();
                OnPropertyChanged("HasPower");
                OnPropertyChanged("SelectedCard");
                SetValue(CardProperty, value);
                CardImage = new NotifyTaskCompletion<BitmapImage>(SelectedVariant.RetrieveImage());
            }
        }

        private void CardImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CardImage.Result");
        }

        public MagicCard SelectedCard { get { return SelectedVariant == null ? null:SelectedVariant.Card; } }

        private NotifyTaskCompletion<BitmapImage> cardImage;
        public NotifyTaskCompletion<BitmapImage> CardImage { 
            get { return cardImage; }
            set { 
                cardImage = value;
                OnPropertyChanged("CardImage");
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

        #endregion

        #region CTOR

        public CardIllustration(MagicCardVariant card)
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
            if (selectedVariant != null) App.state.SelectCard(selectedVariant.Card);
        }

        #endregion

    }

}

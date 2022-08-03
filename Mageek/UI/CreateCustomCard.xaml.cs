using MaGeek.Data.Entities;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{
    public partial class CreateCustomCard : Window, INotifyPropertyChanged
    {

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        MagicCard selectedCard;
        public MagicCard SelectedCard
        {
            get { return selectedCard; }
            set { selectedCard = value; OnPropertyChanged(); }
        }

        public BitmapImage customImage;
        public BitmapImage CustomImage
        {
            get { return customImage; }
            set { customImage = value; OnPropertyChanged(); }
        }

        public string customNameInput { get; set; }
        public string CustomNameInput
        {
            get { return customNameInput; }
            set { 
                customNameInput = value; 
                OnPropertyChanged();
                OnPropertyChanged("CustomNameOutput");
            }
        }

        public string customNameOutput{ get; set; }
        public string CustomNameOutput
        {
            get {
                if (string.IsNullOrEmpty(CustomNameInput)) return SelectedCard.CardId;
                else return CustomNameInput;
            }
        }

        public MagicCardVariant CustomCard;

        public Visibility HasPower
        {
            get
            {
                if (SelectedCard == null) return Visibility.Collapsed;
                return SelectedCard.Type.ToLower().Contains("creature") ?
                       Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region CTOR

        public CreateCustomCard(MagicCard card)
        {
            InitializeComponent();
            SelectedCard = card;
            CustomCard = new MagicCardVariant();
            DataContext = this;
        }

        #endregion

        #region Methods

        private void LoadImg(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                CustomImage = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void SaveCustomCard(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}

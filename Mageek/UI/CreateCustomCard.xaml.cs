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

        MagicCard card;
        public MagicCard Card
        {
            get { return card; }
            set { card = value; OnPropertyChanged(); }
        }

        public BitmapImage customImage;
        public BitmapImage CustomImage
        {
            get { return customImage; }
            set { customImage = value; OnPropertyChanged(); }
        }

        public string customName { get; set; }
        public string CustomName
        {
            get { return customName; }
            set { customName = value; OnPropertyChanged(); }
        }

        public MagicCardVariant CustomCard;

        #endregion

        #region CTOR

        public CreateCustomCard(MagicCard card)
        {
            InitializeComponent();
            this.card = card;
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

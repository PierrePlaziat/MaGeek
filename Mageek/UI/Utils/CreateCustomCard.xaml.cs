using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using MaGeek.AppData.Entities;

namespace MaGeek.UI
{
    public partial class CreateCustomCard : Window, INotifyPropertyChanged, IXmlSerializable
    {

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {

            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
        }

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        CardModel selectedCard;
        public CardModel SelectedCard
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

        public CardVariant CustomCard;

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

        public CreateCustomCard(CardModel card)
        {
            InitializeComponent();
            SelectedCard = card;
            CustomCard = new CardVariant();
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

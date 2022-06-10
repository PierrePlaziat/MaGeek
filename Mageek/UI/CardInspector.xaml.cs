using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MaGeek.UI
{

    public partial class CardInspector : UserControl, INotifyPropertyChanged
    {

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        private MagicCard selectedCard;
        public MagicCard SelectedCard
        {
            get
            {
                return selectedCard;
            }
            set
            {
                selectedIllus = 0;
                selectedCard = value;
                OnPropertyChanged();
                OnPropertyChanged("ImgUrl");
                OnPropertyChanged("Variants");
                OnPropertyChanged("CollectedQuantity");
                OnPropertyChanged("Visible");
                OnPropertyChanged("GetImage");
            }
        }

        void HandleCardSelected(object sender, SelectCardEventArgs e)
        {
            SelectedCard = e.Card;
        }

        public int CollectedQuantity {
            get
            {
                if (selectedCard != null) return selectedCard.CollectedQuantity;
                else return 0;
            }
        }

        private int selectedIllus = 0;

        public List<MagicCardVariant> Variants
        {
            get
            {
                if (selectedCard != null && selectedCard.variants != null && selectedCard.variants.Count > 0)
                    return selectedCard.variants;
                else 
                    return new List<MagicCardVariant>();
            }
        }

        public int nbVariants
        {
            get
            {
                if (selectedCard != null && selectedCard.variants != null)
                    return selectedCard.variants.Count;
                else 
                    return 0;
            }
        }

        public BitmapImage GetImage
        {
            get
            {
                if (selectedCard != null && selectedCard.variants != null && selectedCard.variants.Count > 0 && selectedCard.variants[selectedIllus] != null)
                {
                    return selectedCard.RetrieveImage(selectedIllus);
                }
                return null;
            }
        }

        public Visibility Visible { get { return selectedCard == null ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        #region CTOR

        public CardInspector()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectCard += HandleCardSelected;
        }

        #endregion

        #region Buttons

        private void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            App.cardManager.GotCard_Add(selectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            App.cardManager.GotCard_Remove(SelectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            App.cardManager.AddCardToDeck(selectedCard, App.state.SelectedDeck);
        }

        #endregion

        private void SelectVariant(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VariantListBox.SelectedIndex >= 0 && VariantListBox.SelectedIndex < Variants.Count)
            {
                var variant = Variants[VariantListBox.SelectedIndex];
                if (variant != null)
                {
                    selectedIllus = VariantListBox.SelectedIndex;
                    OnPropertyChanged("GetImage");
                }
            }
        }
    
    }

}

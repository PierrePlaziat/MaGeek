using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class CardDetails : UserControl, INotifyPropertyChanged
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
                selectedCard = value;
                OnPropertyChanged();
                OnPropertyChanged("ImgUrl");
                OnPropertyChanged("Variants");
                OnPropertyChanged("CollectedQuantity");
            }
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

        public string ImgUrl
        {
            get
            {
                if (selectedCard != null && selectedCard.variants != null && selectedCard.variants.Count > 0 && selectedCard.variants[0] != null && selectedCard.variants[0].ImageUrl != null)
                    return selectedCard.variants[selectedIllus].ImageUrl;
                else 
                    return "";
            }
        }

        public delegate void CustomEventHandler(object sender, AddToDeckEventArgs args);
        public event CustomEventHandler RaiseCustomEvent;

        #endregion

        #region CTOR

        public CardDetails()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Buttons

        private void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            App.Collection_AddCard(selectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            App.Collection_RemoveCard(SelectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            OnRaiseCustomEvent(new AddToDeckEventArgs(SelectedCard, App.CurrentDeck));
        }

        protected virtual void OnRaiseCustomEvent(AddToDeckEventArgs e)
        {
            CustomEventHandler raiseEvent = RaiseCustomEvent;
            if (raiseEvent != null) raiseEvent(this, e);
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
                    OnPropertyChanged("ImgUrl");
                }
            }
        }
    
    }

}

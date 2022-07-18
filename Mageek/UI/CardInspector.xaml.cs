using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public List<CardTag> Tags
        {
            get
            {
                if(selectedCard==null) return null;
                return App.database.Tags.Where(x=>x.CardId==selectedCard.CardId).ToList();
            }
        }

        private MagicCard selectedCard;
        public MagicCard SelectedCard
        {
            get { return selectedCard; }
            set
            {
                selectedCard = value;
                OnPropertyChanged("Variants");
                OnPropertyChanged();
                AutoSelectVariant();
                OnPropertyChanged("CollectedQuantity");
                OnPropertyChanged("Visible");
            }
        }

        private void AutoSelectVariant()
        {
            VariantListBox.UnselectAll();
            if (selectedCard == null) return;
            if (!string.IsNullOrEmpty(selectedCard.FavouriteVariant))
            {
                SelectedVariant = selectedCard.Variants.Where(x => x.Id == selectedCard.FavouriteVariant).FirstOrDefault();
            }
            else
            {
                SelectedVariant = selectedCard.Variants.Where(x => !string.IsNullOrEmpty(x.ImageUrl)).FirstOrDefault();
            }
        }

        private MagicCardVariant selectedVariant;
        public MagicCardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { selectedVariant = value; OnPropertyChanged(); }
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


        public List<MagicCardVariant> Variants
        {
            get
            {
                if (selectedCard != null && selectedCard.Variants != null && selectedCard.Variants.Count > 0)
                    return selectedCard.Variants;
                else 
                    return new List<MagicCardVariant>();
            }
        }

        public int nbVariants
        {
            get
            {
                if (selectedCard != null && selectedCard.Variants != null)
                    return selectedCard.Variants.Count;
                else 
                    return 0;
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
            App.cardManager.AddCardToDeck(SelectedVariant, App.state.SelectedDeck);
        }

        #endregion
        private void SelectVariant(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedVariant = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;
        }


        private void SetFav(object sender, RoutedEventArgs e)
        {
            var cardvar = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;  
            App.cardManager.SetFav(cardvar.Card, cardvar.Id);


        }

        private void AddTag(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTag.Text))
            {
                App.database.Tags.Add(new CardTag(NewTag.Text, selectedCard));
                App.database.SaveChanges();
            }
        }

    }

}

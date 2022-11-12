using MaGeek.Data.Entities;
using MaGeek.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MaGeek.UI
{

    public partial class CardInspector : TemplatedUserControl
    {

        #region Attributes

        public List<CardTag> Tags
        {
            get
            {
                if(selectedCard==null) return null;
                return App.DB.Tags.Where(x=>x.CardId==selectedCard.CardId).ToList();
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
                OnPropertyChanged("Tags");
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
            if(selectedVariant != null) VariantListBox.SelectedItem = selectedVariant;
        }

        private MagicCardVariant selectedVariant;
        public MagicCardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { selectedVariant = value; OnPropertyChanged(); }
        }

        void HandleCardSelected(MagicCard Card)
        {
            if (!isPinned) SelectedCard = Card;
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
            App.STATE.CardSelectedEvent += HandleCardSelected;
        }

        #endregion

        #region Buttons

        private void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            App.CARDS.Utils.GotCard_Add(selectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            App.CARDS.Utils.GotCard_Remove(SelectedCard);
            OnPropertyChanged("CollectedQuantity");
        }

        private void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            App.CARDS.Utils.AddCardToDeck(SelectedVariant, App.STATE.SelectedDeck,1);
        }

        #endregion

        #region Variants

        private void SelectVariant(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VariantListBox.SelectedIndex < 0) return;
            SelectedVariant = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;
        }

        private void SetFav(object sender, RoutedEventArgs e)
        {
            var cardvar = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;  
            App.CARDS.Utils.SetFav(cardvar.Card, cardvar.Id);
        }

        private void LaunchCustomCardCreation(object sender, RoutedEventArgs e)
        {
            var window = new CreateCustomCard(selectedCard);
            window.Show();
        }

        #endregion

        #region Tags

        private void AddTag(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTag.Text))
            {
                App.DB.Tags.Add(new CardTag(NewTag.Text, selectedCard));
                App.DB.SaveChanges();
                OnPropertyChanged("Tags");
                NewTag.Text = "";
                sugestions.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            CardTag cardTag = (CardTag)((Button)sender).DataContext;
            App.DB.Tags.Remove(cardTag);
            App.DB.SaveChanges();
            OnPropertyChanged("Tags");
            sugestions.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void NewTag_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = GetExistingTags();
            string query = (sender as TextBox).Text;
            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }
            resultStack.Children.Clear();
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    addItem(obj);
                    found = true;
                }
            }
            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private List<string> GetExistingTags()
        {
            return App.CARDS.AllTags;
        }

        private void addItem(string text)
        {
            TextBlock block = new TextBlock();
            block.Text = text;
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;
            block.MouseLeftButtonUp += (sender, e) =>
            {
                NewTag.Text = (sender as TextBlock).Text;
            };
            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Gray;
            };
            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
            resultStack.Children.Add(block);
        }

        private void NewTag_LostFocus(object sender, RoutedEventArgs e)
        {
            sugestions.Visibility = Visibility.Collapsed;
        }

        #endregion

        private void UpdateCardVariants(object sender, RoutedEventArgs e)
        {
            App.CARDS.Importer.AddImportToQueue(
                new PendingImport
                {
                    mode = ImportMode.Update,
                    content = SelectedCard.CardId
                }
            );
        }

        bool isPinned = false;
        private void PinCard(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned) PinButton.Background = Brushes.Gray;
            else PinButton.Background = Brushes.Black;
        }
    }

}

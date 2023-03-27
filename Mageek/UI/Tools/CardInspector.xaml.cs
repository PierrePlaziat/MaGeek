using MaGeek.AppBusiness;
using MaGeek.AppData.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MaGeek.UI
{

    public partial class CardInspector : TemplatedUserControl
    {

        #region Attributes

        private MagicCard selectedCard;
        public MagicCard SelectedCard
        {
            get { return selectedCard; }
            set
            {
                selectedCard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsActive));
                Reload();
            }
        }

        public int CollectedQuantity
        {
            get
            {
                if (SelectedCard == null) return 0;
                return SelectedCard.Got;
            }
        }

        public List<MagicCardVariant> Variants { get; private set; }

        private MagicCardVariant selectedVariant;
        public MagicCardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { 
                selectedVariant = value; 
                OnPropertyChanged();
                AsyncReloadVariant();
            }
        }

        public int NbVariants { get; private set; }
        public string Price { get; private set; }
        public List<Legality> Legalities { get; private set; }
        public Brush PriceColor { get; private set; }
        public List<CardTag> Tags { get; private set; }

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive
        {
            get { return SelectedCard == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #endregion

        #region CTOR

        public CardInspector()
        {
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.CardSelectedEvent += HandleCardSelected;
        }

        void HandleCardSelected(MagicCard Card)
        {
            if (!isPinned) SelectedCard = Card;
        }

        #endregion

        #region Async Reload

        private void Reload()
        {
            DoAsyncReloadCard().ConfigureAwait(false);
        }

        private void AsyncReloadVariant()
        {
            DoAsyncReloadVariant().ConfigureAwait(false);
        }

        private async Task DoAsyncReloadCard()
        {
            IsLoading = Visibility.Visible; 
            await Task.Run(() => { Variants = GetVariants(); });
            await Task.Run(() => { NbVariants = GetNbVariants(); });
            await Task.Run(() => { Tags = GetTags(); });
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(Variants));
                OnPropertyChanged(nameof(NbVariants));
                OnPropertyChanged(nameof(CollectedQuantity));
                OnPropertyChanged(nameof(Tags));
            });
            AutoSelectVariant();
        }

        private async Task DoAsyncReloadVariant()
        {
            IsLoading = Visibility.Visible;
            await Task.Run(() => { Legalities = GetLegalities(); });
            await Task.Run(() => { Price = GetPrice(); });
            await Task.Run(() => { PriceColor = GetPriceColor(); });
            await Task.Run(() =>
            {
                OnPropertyChanged(nameof(Legalities));
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(PriceColor));
                IsLoading = Visibility.Collapsed;
            });
        }

        #endregion

        #region Data Retrieve

        private List<MagicCardVariant> GetVariants()
        {
            if (selectedCard != null && selectedCard.Variants != null && selectedCard.Variants.Count > 0)
                return selectedCard.Variants;
            else 
                return new List<MagicCardVariant>();
        }
        private int GetNbVariants()
        {
            if (selectedCard != null && selectedCard.Variants != null)
                return selectedCard.Variants.Count;
            else 
                return 0;
        }
        private string GetPrice()
        {
                if (SelectedVariant == null) return "/";
                return App.Biz.Utils.GetCardPrize(SelectedVariant).ToString(); 
        }
        private List<Legality> GetLegalities()
        {
            if (SelectedVariant == null) return new List<Legality>();
            return App.Biz.Utils.GetCardLegal(SelectedVariant);
        }
        private Brush GetPriceColor()
        {
            var p = App.Biz.Utils.GetCardPrize(SelectedVariant);
            if (p>=10) return Brushes.White;
            else if (p>=5) return Brushes.Orange;
            else if (p>=2) return Brushes.Yellow;
            else if (p>=1) return Brushes.Green;
            else if (p>=0.2) return Brushes.LightGray;
            else if (p>=0) return Brushes.DarkGray;
            else return Brushes.Black;
        }
        private List<CardTag> GetTags()
        {
            if(selectedCard==null) return null;
            return App.Biz.Utils.FindTagsForCard(selectedCard.CardId);
        }

        #endregion

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
            if (selectedVariant != null) VariantListBox.SelectedItem = selectedVariant;
        }

        #region Buttons

        private void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            MagicCardVariant variant = (MagicCardVariant) ((Button)sender).DataContext;
            App.Biz.Utils.GotCard_Add(variant);
            OnPropertyChanged(nameof(CollectedQuantity));
            OnPropertyChanged(nameof(Variants));
        }

        private void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            MagicCardVariant variant = (MagicCardVariant)((Button)sender).DataContext;
            App.Biz.Utils.GotCard_Remove(variant);
            OnPropertyChanged(nameof(CollectedQuantity));
            OnPropertyChanged(nameof(Variants));
        }

        private void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            App.Biz.Utils.AddCardToDeck(SelectedVariant, App.State.SelectedDeck,1);
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
            App.Biz.Utils.SetFav(cardvar.Card, cardvar.Id);
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
                App.Biz.Utils.TagCard(selectedCard,NewTag.Text);
                OnPropertyChanged("Tags");
                NewTag.Text = "";
                sugestions.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            CardTag cardTag = (CardTag)((Button)sender).DataContext;
            App.Biz.Utils.UnTagCard(cardTag);
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
                if (obj.Tag.ToLower().StartsWith(query.ToLower()))
                {
                    addItem(obj.Tag);
                    found = true;
                }
            }
            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }



        private List<CardTag> GetExistingTags()
        {
            return App.Biz.Utils.GetTagsDistinct();
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
            App.Biz.Importer.AddImportToQueue(
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

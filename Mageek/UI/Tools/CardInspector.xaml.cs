using MaGeek.AppBusiness;
using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        string loadMsg;
        public string LoadMsg
        {
            get { return loadMsg; }
            set { loadMsg = value; OnPropertyChanged(); }
        }

        private CardModel selectedCard;
        public CardModel SelectedCard
        {
            get { return selectedCard; }
            set
            {
                selectedCard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsActive));
                if (value != null) ReloadCard().ConfigureAwait(false);
                UpdateButton.Visibility = Visibility.Visible;
                AutoSelectVariant().ConfigureAwait(false);
            }
        }

        public List<CardVariant> Variants { get; private set; }

        private CardVariant selectedVariant;
        public CardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { 
                selectedVariant = value; 
                OnPropertyChanged();
                Dispatcher.Invoke(() => {
                    VariantListBox.SelectedItem = selectedVariant;
                });
            }
        }

        public List<CardLegality> Legalities { get; private set; }
        public List<CardRule> Rulings { get; private set; }
        public List<CardRelation> RelatedCards { get; private set; }
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

        public Visibility ShowRelateds
        {
            get { return RelatedCards == null || RelatedCards.Count>0 ? Visibility.Visible : Visibility.Collapsed; }
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

        void HandleCardSelected(CardModel Card)
        {
            if (!isPinned)
            {
                if (Card == null) SelectedCard = null;
                else
                {
                    IsLoading = Visibility.Visible;
                    Task.Run(() =>
                    {
                        LoadMsg = "Finding card";
                        SelectedCard = MageekCollection.FindCardById(Card.CardId).Result;
                    }).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region Async Reload

        private async Task ReloadCard()
        {
            try
            {
                IsLoading = Visibility.Visible; 
                await Task.Run(() => { 
                    AutoSelectVariant().ConfigureAwait(true);
                    LoadMsg = "Loading variants";
                    Variants = GetVariants();
                    LoadMsg = "Loading legalities";
                    Legalities = MageekApi.GetLegalities(SelectedCard).Result;
                    LoadMsg = "Loading rulings";
                    Rulings = MageekApi.GetRules(SelectedCard).Result;
                    LoadMsg = "Loading prices";
                    foreach (var v in SelectedCard.Variants)
                    {
                        MageekApi.RetrieveCardValues(v).ConfigureAwait(true);
                    }
                    LoadMsg = "Loading relateds";
                    RelatedCards = MageekApi.GetRelatedCards(SelectedCard).Result;
                    LoadMsg = "Loading tags";
                    Tags = GetTags().Result;
                    LoadMsg = "Updating";
                    OnPropertyChanged(nameof(Variants));
                    OnPropertyChanged(nameof(Legalities));
                    OnPropertyChanged(nameof(Tags));
                    OnPropertyChanged(nameof(Rulings));
                    OnPropertyChanged(nameof(RelatedCards));
                    OnPropertyChanged(nameof(ShowRelateds));
                });
                IsLoading = Visibility.Collapsed;
            }
            catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        #endregion

        #region Data Retrieve

        private List<CardVariant> GetVariants()
        {
            if (selectedCard != null && selectedCard.Variants != null && selectedCard.Variants.Count > 0)
                return selectedCard.Variants;
            else 
                return new List<CardVariant>();
        }

        private async Task<List<CardTag>> GetTags()
        {
            if(selectedCard==null) return null;
            return await MageekStats.FindTagsForCard(selectedCard.CardId);
        }

        #endregion

        private async Task AutoSelectVariant()
        {
            await Task.Run(() =>
            {
                if (selectedCard == null) return;
                if (!string.IsNullOrEmpty(selectedCard.FavouriteVariant))
                {
                    SelectedVariant = selectedCard.Variants.Where(x => x.Id == selectedCard.FavouriteVariant).FirstOrDefault();
                }
                else
                {
                    SelectedVariant = selectedCard.Variants/*.Where(x => !string.IsNullOrEmpty(x.ImageUrl_Front))*/.FirstOrDefault();
                }
            });
        }

        #region Buttons

        private async void AddCardToCollection(object sender, RoutedEventArgs e)
        {
            CardVariant variant = (CardVariant) ((Button)sender).DataContext;
            await MageekCollection.GotCard_Add(variant);
            HandleCardSelected(selectedCard);
        }

        private async void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            CardVariant variant = (CardVariant)((Button)sender).DataContext;
            await MageekCollection.GotCard_Remove(variant);
            HandleCardSelected(selectedCard);
        }

        private async void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            await MageekCollection.AddCardToDeck(SelectedVariant, App.State.SelectedDeck,1);
        }

        #endregion

        #region Variants

        private void SelectVariant(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VariantListBox.SelectedIndex < 0) return;
            SelectedVariant = VariantListBox.Items[VariantListBox.SelectedIndex] as CardVariant;
        }

        private async void SetFav(object sender, RoutedEventArgs e)
        {
            var cardvar = VariantListBox.Items[VariantListBox.SelectedIndex] as CardVariant;  
            await MageekCollection.SetFav(cardvar.Card, cardvar);
        }

        private void LaunchCustomCardCreation(object sender, RoutedEventArgs e)
        {
            var window = new CreateCustomCard(selectedCard);
            window.Show();
        }

        #endregion

        #region Tags

        private async void AddTag(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewTag.Text))
            {
                await MageekStats.TagCard(selectedCard,NewTag.Text);
                OnPropertyChanged(nameof(Tags));
                NewTag.Text = "";
                sugestions.Visibility = Visibility.Collapsed;
            }
        }

        private async void DeleteTag(object sender, RoutedEventArgs e)
        {
            CardTag cardTag = (CardTag)((Button)sender).DataContext;
            await MageekStats.UnTagCard(cardTag);
            OnPropertyChanged(nameof(Tags));
            sugestions.Visibility = Visibility.Collapsed;
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await MageekStats.GetTagsDistinct();
            string query = (sender as TextBox).Text;
            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
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
            UpdateButton.Visibility = Visibility.Hidden;
            App.Importer.AddImportToQueue(
                new PendingImport
                {
                    Mode = ImportMode.Update,
                    Content = SelectedCard.CardId
                }
            );
        }

        bool isPinned = false;
        private void PinCard(object sender, RoutedEventArgs e)
        {
            isPinned = !isPinned;
            if (isPinned) PinButton.Background = Brushes.Gray;
            else PinButton.Background = (Brush)(new BrushConverter().ConvertFrom("#555")); ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + selectedCard.CardId);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CardRelation rel = (CardRelation)((Button)sender).DataContext;
            CardModel relatedCard;
            using (var DB = App.DB.GetNewContext())
            {
                relatedCard = DB.CardModels.Where(x => x.CardId == rel.Card2Id)
                    .ToList()
                    .FirstOrDefault();
            }
            if (relatedCard != null) App.Events.RaiseCardSelected(relatedCard);
        }
    }

}

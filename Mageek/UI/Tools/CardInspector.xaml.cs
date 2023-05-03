﻿using MaGeek.AppBusiness;
using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using System;
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

        private MagicCard selectedCard;
        public MagicCard SelectedCard
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

        public List<MagicCardVariant> Variants { get; private set; }

        private MagicCardVariant selectedVariant;
        public MagicCardVariant SelectedVariant
        {
            get { return selectedVariant; }
            set { 
                selectedVariant = value; 
                OnPropertyChanged();
                Dispatcher.Invoke(() => {
                    VariantListBox.SelectedItem = selectedVariant;
                });
                ReloadCardVariant().ConfigureAwait(false);
            }
        }

        public List<Legality> Legalities { get; private set; }
        public List<Rule> Rulings { get; private set; }
        public List<CardCardRelation> RelatedCards { get; private set; }
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

        void HandleCardSelected(MagicCard Card)
        {
            if (!isPinned)
            {
                if (Card == null) SelectedCard = null;
                else SelectedCard = MageekUtils.FindCardById(Card.CardId).Result;
            }
        }

        #endregion

        #region Async Reload

        private async Task ReloadCard()
        {
            try
            {
                IsLoading = Visibility.Visible; 
                await AutoSelectVariant();
                await Task.Run(() => { 
                    Variants = GetVariants();
                });

                Legalities = await MageekApi.GetLegalities(SelectedCard);
                Rulings = await MageekApi.GetRules(SelectedCard);
                foreach (var v in SelectedCard.Variants)
                {
                    await MageekApi.RetrieveCardValues(v);
                }
                RelatedCards = await MageekApi.GetRelatedCards(SelectedCard);
                Tags = await GetTags();
                await OnPropertyChangedAsync(nameof(Variants));
                await OnPropertyChangedAsync(nameof(Tags));
                await OnPropertyChangedAsync(nameof(Rulings));
                await OnPropertyChangedAsync(nameof(RelatedCards));
                await OnPropertyChangedAsync(nameof(ShowRelateds));
                await Task.Run(() =>
                {
                    IsLoading = Visibility.Collapsed;
                });
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        private async Task ReloadCardVariant()
        {
            try
            {
                await Task.Run(() =>
                {
                    OnPropertyChanged(nameof(Legalities));
                });
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
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

        private async Task<List<CardTag>> GetTags()
        {
            if(selectedCard==null) return null;
            return await MageekUtils.FindTagsForCard(selectedCard.CardId);
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
            MagicCardVariant variant = (MagicCardVariant) ((Button)sender).DataContext;
            await MageekUtils.GotCard_Add(variant);
            HandleCardSelected(selectedCard);
        }

        private async void SubstractCardFromCollection(object sender, RoutedEventArgs e)
        {
            MagicCardVariant variant = (MagicCardVariant)((Button)sender).DataContext;
            await MageekUtils.GotCard_Remove(variant);
            HandleCardSelected(selectedCard);
        }

        private async void AddToCurrentDeck(object sender, RoutedEventArgs e)
        {
            await MageekUtils.AddCardToDeck(SelectedVariant, App.State.SelectedDeck,1);
        }

        #endregion

        #region Variants

        private void SelectVariant(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VariantListBox.SelectedIndex < 0) return;
            SelectedVariant = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;
        }

        private async void SetFav(object sender, RoutedEventArgs e)
        {
            var cardvar = VariantListBox.Items[VariantListBox.SelectedIndex] as MagicCardVariant;  
            await MageekUtils.SetFav(cardvar.Card, cardvar);
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
                await MageekUtils.TagCard(selectedCard,NewTag.Text);
                OnPropertyChanged("Tags");
                NewTag.Text = "";
                sugestions.Visibility = Visibility.Collapsed;
            }
        }

        private async void DeleteTag(object sender, RoutedEventArgs e)
        {
            CardTag cardTag = (CardTag)((Button)sender).DataContext;
            await MageekUtils.UnTagCard(cardTag);
            OnPropertyChanged("Tags");
            sugestions.Visibility = Visibility.Collapsed;
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await MageekUtils.GetTagsDistinct();
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
            Process.Start(new ProcessStartInfo("https://www.cardmarket.com/en/Magic/Products/Search?searchString=" + selectedCard.CardId) { UseShellExecute = true });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CardCardRelation rel = (CardCardRelation)((Button)sender).DataContext;
            MagicCard relatedCard;
            using (var DB = App.DB.GetNewContext())
            {
                relatedCard = DB.Cards.Where(x => x.CardId == rel.Card2Id)
                    .ToList()
                    .FirstOrDefault();
            }
            if (relatedCard != null) App.Events.RaiseCardSelected(relatedCard);
        }
    }

}

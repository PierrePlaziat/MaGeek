using CommunityToolkit.Mvvm.ComponentModel;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.ViewModels
{
    public partial class CardSearcherViewModel : BaseViewModel
    {

        private SettingService config;

        public CardSearcherViewModel(SettingService config)
        {
            this.config = config;
        }

        [ObservableProperty] private bool colorIsOr;
        [ObservableProperty] private List<string> historic;
        [ObservableProperty] private List<Cards> cardList;
        [ObservableProperty] private List<Tag> availableTags;
        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private bool showAdvanced = false;
        [ObservableProperty] private bool showNormal = true;
        [ObservableProperty] private string filterName = "";
        [ObservableProperty] private string filterType = "";
        [ObservableProperty] string filterKeyword = "";
        [ObservableProperty] private string filterText = "";
        [ObservableProperty] private MtgColorFilter filterColor = MtgColorFilter._;
        [ObservableProperty] private Tag filterTag = null;
        [ObservableProperty] private bool onlyGot = false;

        #region DATA LOAD

        private async Task ReloadData()
        {
            IsLoading = true;

            if (!string.IsNullOrEmpty(FilterName) && !Historic.Contains(FilterName))
            {
                Historic.Add(FilterName);
                if (Historic.Count > 30) Historic.RemoveAt(0);
                OnPropertyChanged(nameof(Historic));
            }

            if (ShowAdvanced == false)
            {
                CardList = await MageekService.MageekService.NormalSearch(
                    config.Settings[AppSetting.ForeignLanguage],
                    FilterName
                );
            }
            else
            {
                var lang = config.Settings[AppSetting.ForeignLanguage];
                var color = FilterColor.ToString();
                string tagz = "";// FilterTag.TagContent;
                CardList = await MageekService.MageekService.AdvancedSearch(
                    lang,
                    FilterName,
                    FilterType,
                    FilterKeyword,
                    FilterText,
                    color,
                    tagz,
                    OnlyGot,
                    ColorIsOr
                );
            }
            OnPropertyChanged(nameof(CardList));
            await Task.Delay(50);
            IsLoading = false;
        }

        #endregion

        #region UI Link

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            ReloadData().ConfigureAwait(false);
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            ShowAdvanced = false;
            ShowNormal = true;
            FilterName = "";
            FilterType = "";
            FilterKeyword = "";
            FilterText = "";
            FilterColor = MtgColorFilter._;
            FilterTag = null;
            OnlyGot = false;
        }

        //private void FilterName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == System.Windows.Input.Key.Enter)
        //    {
        //        e.Handled = true;
        //        var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
        //        binding.UpdateSource();
        //        ReloadData().ConfigureAwait(false);
        //    }
        //}

        private void Button_AdvancedSearcher(object sender, RoutedEventArgs e)
        {
            if (ShowAdvanced == false)
            {
                ShowAdvanced = true;
                ShowNormal = false;
            }
            else if (ShowAdvanced == true)
            {
                ShowAdvanced = false    ;
                ShowNormal = true;
            }
        }

        //private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (CardGrid.SelectedItem is Cards card) App.Events.RaiseCardSelected(card.Uuid);
        //}

        //private async void AddToDeck(object sender, RoutedEventArgs e)
        //{
        //    foreach (Cards c in CardGrid.SelectedItems)
        //    {
        //        await MageekService.MageekService.AddCardToDeck(c.Uuid, App.State.SelectedDeck, 1);
        //    }
        //    App.Events.RaiseUpdateDeck();
        //}

        private void FilterTag_DropDownOpened(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableTags));
        }

        //private void FillColorFilterCombo()
        //{
        //    ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        //}

        #endregion

        //private void ContextMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    FilterName = ((MenuItem)e.OriginalSource).Header.ToString();
        //}
    }


}

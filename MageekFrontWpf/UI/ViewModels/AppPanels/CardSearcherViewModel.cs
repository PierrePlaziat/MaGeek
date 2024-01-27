using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
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

        public async Task ReloadData()
        {
            IsLoading = true;

            await Task.Delay(100);

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

        [RelayCommand]
        private void Search()
        {
            ReloadData().ConfigureAwait(false);
        }

        [RelayCommand]
        private void ResetFilters()
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

        [RelayCommand]
        private void AdvancedSearch()
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

        //private async void AddToDeck(object sender, RoutedEventArgs e)
        //{
        //    foreach (Cards c in CardGrid.SelectedItems)
        //    {
        //        await MageekService.MageekService.AddCardToDeck(c.Uuid, App.State.SelectedDeck, 1);
        //    }
        //    App.Events.RaiseUpdateDeck();
        //}

    }


}

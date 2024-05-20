using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekFrontWpf.Framework.AppValues;
using CommunityToolkit.Mvvm.Messaging;
using MageekCore.Service;
using PlaziatTools;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{
    public partial class CardSearcherViewModel : 
        ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private SettingService config;
        private IMageekService mageek;

        public CardSearcherViewModel(
            SettingService config,
            IMageekService mageek
        ){
            this.mageek = mageek;
            this.config = config;
            WeakReferenceMessenger.Default.RegisterAll(this);
            Logger.Log("!");
        }

        [ObservableProperty] private bool colorIsOr;
        [ObservableProperty] private List<string> historic = new();
        [ObservableProperty] private List<SearchedCards> cardList;
        [ObservableProperty] private List<Tag> availableTags;
        [ObservableProperty] private bool isLoading = false;
        [ObservableProperty] private bool advancedMode = false;
        [ObservableProperty] private string filterName = "";
        [ObservableProperty] private string filterType = "";
        [ObservableProperty] string filterKeyword = "";
        [ObservableProperty] private string filterText = "";
        [ObservableProperty] private MtgColorFilter filterColor = MtgColorFilter._;
        [ObservableProperty] private Tag filterTag = null;
        [ObservableProperty] private bool onlyGot = false;
        [ObservableProperty] private int currentPage = 0;
        [ObservableProperty] private int nbResulsts = 200;

        public void Receive(LaunchAppMessage message)
        {
            //FilterName = "Edgar";
            //DoSearch().ConfigureAwait(false);
        }

        [RelayCommand] private async Task Search()
        {
            await DoSearch();
        }
        
        [RelayCommand] private void AdvancedSearch()
        {
            AdvancedMode = !AdvancedMode;
        }

        [RelayCommand] private void ResetFilters()
        {
            AdvancedMode = false;
            FilterName = "";
            FilterType = "";
            FilterKeyword = "";
            FilterText = "";
            FilterColor = MtgColorFilter._;
            FilterTag = null;
            OnlyGot = false;
        }

        public async Task DoSearch()
        {
            IsLoading = true;
            await Task.Run(async () => {
                FillHistoric();
                if (!AdvancedMode)
                {
                    CardList = await mageek.Cards_Search(
                        FilterName, config.Settings[Setting.Translations],
                        CurrentPage, NbResulsts
                    );
                }
                if (AdvancedMode)
                {
                    CardList = await mageek.Cards_Search(
                        FilterName, config.Settings[Setting.Translations],
                        CurrentPage, NbResulsts,
                        FilterType, FilterKeyword, FilterText,
                        FilterColor.ToString(),
                        "", // FilterTag.TagContent;
                        OnlyGot, ColorIsOr
                    );
                }
            });
            IsLoading = false;
        }

        private void FillHistoric()
        {
            if (!string.IsNullOrEmpty(FilterName) && !Historic.Contains(FilterName))
            {
                Historic.Add(FilterName);
                if (Historic.Count > 30) Historic.RemoveAt(0);
                OnPropertyChanged(nameof(Historic));
            }
        }

    }

}

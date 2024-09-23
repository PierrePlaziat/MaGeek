using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using PlaziatTools;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;
using System;

namespace MageekDesktopClient.UI.ViewModels.AppPanels
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
            ResetFilters();
            Logger.Log("Done");
        }

        [ObservableProperty] private List<SearchedCards> cardList = new();
        [ObservableProperty] private List<string> historic = new();
        [ObservableProperty] private int currentPage = 0;
        [ObservableProperty] private int nbResulsts = 200;
        [ObservableProperty] private bool advancedMode;
        [ObservableProperty] private bool filterOnlyGot;
        [ObservableProperty] private string filterName;
        [ObservableProperty] private string filterType;
        [ObservableProperty] private string filterKeyword;
        [ObservableProperty] private string filterText;
        [ObservableProperty] private MtgColorFilter filterColor;
        [ObservableProperty] private bool filterColorTrueOrFalseAnd;
        [ObservableProperty] private Tag filterTag;
        [ObservableProperty] private List<Tag> filterTagsAvailable;
        [ObservableProperty] private bool isLoading = false;

        public void Receive(LaunchAppMessage message) 
        {
            //FilterName = "Edgar Markov";
            //Search().ConfigureAwait(false);
        }

        [RelayCommand] public async Task Search()
        {
            IsLoading = true;
            FillHistoric();
            if (!AdvancedMode) await SearchNormal();
            if (AdvancedMode) await SearchAdvanced();
            Logger.Log("Done");
            IsLoading = false;
        }

        private void FillHistoric()
        {
            try
            { 
                if (!string.IsNullOrEmpty(FilterName) && !Historic.Contains(FilterName))
                {
                    Historic.Add(FilterName);
                    if (Historic.Count > 30) Historic.RemoveAt(0);
                    OnPropertyChanged(nameof(Historic));
                }
            }
            catch (Exception e) { Logger.Log(e); }
        }

        private async Task SearchNormal()
        {
            try
            { 
                if (string.IsNullOrEmpty(FilterName)) return;
                CardList = await mageek.Cards_Search(
                    FilterName,
                    "French",
                    CurrentPage,
                    NbResulsts
                );
            }
            catch (Exception e) { Logger.Log(e); }
        }
        private async Task SearchAdvanced()
        {
            try
            { 
                CardList = await mageek.Cards_Search(
                    FilterName,
                    config.Settings[Setting.Translations.ToString()],
                    CurrentPage,
                    NbResulsts,
                    FilterType, FilterKeyword, FilterText,
                    FilterColor.ToString(),
                    "",
                    // FilterTag.TagContent;
                    FilterOnlyGot,
                    FilterColorTrueOrFalseAnd
                );
            }
            catch (Exception e) { Logger.Log(e); }
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
            FilterOnlyGot = false;
            FilterColorTrueOrFalseAnd = false;
        }

    }

}

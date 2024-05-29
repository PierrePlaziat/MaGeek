using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class CollecEstimationViewModel : ObservableViewModel
    {

        private IMageekService mageek;
        private SettingService config;

        public CollecEstimationViewModel(
            SettingService config,
            IMageekService mageek
        )
        {
            this.mageek = mageek;
            this.config = config;
        }

        [ObservableProperty] private int totalGot = 0;
        [ObservableProperty] private int totalDiffGot = 0;
        [ObservableProperty] private int totalDiffExist = 0;
        [ObservableProperty] private float autoEstimation = 0;
        [ObservableProperty] private float finalEstimation = 0;
        [ObservableProperty] private int missingCount = 0;
        [ObservableProperty] private bool isLoading = true;
        [ObservableProperty] private List<string> missingList = new();

        [RelayCommand]
        private async Task AddManualEstimation()
        {
            throw new NotImplementedException();
        }

        private async Task AutoEstimate()
        {
            //Logger.Log("AutoEstimate");
            //try
            //{
            //    IsLoading = true;
            //    await Task.Run(async () => {
            //        TotalDiffExist = await mageek.GetTotal_ExistingArchetypes();
            //        TotalDiffGot = await mageek.GetTotal_CollectedArchetype();
            //        TotalGot = await mageek.GetTotal_CollectedDiff();
            //        var est = await mageek.AutoEstimateCollection(config.Settings[Setting.ForeignLanguage]);
            //        AutoEstimation = est.Item1;
            //        MissingList = est.Item2;
            //        OnPropertyChanged(nameof(TotalDiffExist));
            //        OnPropertyChanged(nameof(TotalDiffGot));
            //        OnPropertyChanged(nameof(TotalGot));
            //        OnPropertyChanged(nameof(AutoEstimation));
            //        OnPropertyChanged(nameof(MissingList));
            //        OnPropertyChanged(nameof(MissingCount));
            //    });
            //    IsLoading = false;
            //}
            //catch (Exception e)
            //{
            //    Logger.Log(e);
            //}
        }

        //public async Task<Tuple<float, List<string>>> AutoEstimateCollection(string currency)
        //{
        //    float total = 0;
        //    List<string> missingList = new();
        //    try
        //    {
        //        using CollectionDbContext DB = await collec.GetContext();
        //        var allGot = await DB.CollectedCard.Where(x => x.Collected > 0).ToListAsync();
        //        foreach (CollectedCard collectedCard in allGot)
        //        {
        //            var price = await Cards_GetPrice(collectedCard.CardUuid);
        //            if (price != null)
        //            {
        //                //if (currency == "Eur") total += price.GetLastPriceEur;
        //                //if (currency == "Usd") total += price.GetLastPriceUsd;
        //            }
        //            else missingList.Add(collectedCard.CardUuid);
        //        }
        //    }
        //    catch (Exception e) { Logger.Log(e); }
        //    return new Tuple<float, List<string>>(total, missingList);
        //}

    }

}

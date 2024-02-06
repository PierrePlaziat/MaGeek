using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class CollectionEstimationViewModel : BaseViewModel
    {

        private MageekService.MageekService mageek;
        private SettingService config;

        public CollectionEstimationViewModel(
            SettingService config,
            MageekService.MageekService mageek
        )
        {
            this.mageek = mageek;
            this.config = config;
            DelayLoad().ConfigureAwait(false);
        }

        [ObservableProperty] private int totalGot = 0;
        [ObservableProperty] private int totalDiffGot = 0;
        [ObservableProperty] private int totalDiffExist = 0;
        [ObservableProperty] private Decimal autoEstimation = 0;
        [ObservableProperty] private float finalEstimation = 0;
        [ObservableProperty] private int missingCount = 0;
        [ObservableProperty] private bool isLoading = true;
        [ObservableProperty] private List<string> missingList = new();

        [RelayCommand]
        private static void DoAddManualEstimation()
        {
            throw new NotImplementedException();
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            await AutoEstimate();
        }

        private async Task AutoEstimate()
        {
            try
            {
                IsLoading = true;
                await Task.Run(async () => {
                    TotalDiffExist = await mageek.GetTotal_ExistingArchetypes();
                    TotalDiffGot = await mageek.GetTotal_CollectedArchetype();
                    TotalGot = await mageek.GetTotal_CollectedDiff();
                    var est = await mageek.AutoEstimatePrices(config.Settings[AppSetting.ForeignLanguage]);
                    AutoEstimation = est.Item1;
                    MissingList = est.Item2;
                    OnPropertyChanged(nameof(TotalDiffExist));
                    OnPropertyChanged(nameof(TotalDiffGot));
                    OnPropertyChanged(nameof(TotalGot));
                    OnPropertyChanged(nameof(AutoEstimation));
                    OnPropertyChanged(nameof(missingList));
                    OnPropertyChanged(nameof(MissingCount));
                });
                IsLoading = false;
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

    }

}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MageekFrontWpf.ViewModels
{

    public partial class CollectionEstimationViewModel : BaseViewModel
    {

        private SettingService config;
        public CollectionEstimationViewModel(
            SettingService config
        ){
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
                    TotalDiffExist = await MageekService.MageekService.GetTotal_ExistingArchetypes();
                    TotalDiffGot = await MageekService.MageekService.GetTotal_CollectedArchetype();
                    TotalGot = await MageekService.MageekService.GetTotal_CollectedDiff();
                    var est = await MageekService.MageekService.AutoEstimatePrices(config.Settings[AppSetting.ForeignLanguage]);
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

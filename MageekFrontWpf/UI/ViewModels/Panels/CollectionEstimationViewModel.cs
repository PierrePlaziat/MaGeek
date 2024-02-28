using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using PlaziatTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class CollecEstimationViewModel : BaseViewModel
    {

        private MageekCore.MageekService mageek;
        private SettingService config;
        private ILogger<CollecEstimationViewModel> logger;

        public CollecEstimationViewModel(
            SettingService config,
            MageekCore.MageekService mageek,
            ILogger<CollecEstimationViewModel> logger
        )
        {
            this.logger = logger;
            this.mageek = mageek;
            this.config = config;
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
        private async Task AddManualEstimation()
        {
            throw new NotImplementedException();
        }

        private async Task AutoEstimate()
        {
            Logger.Log("AutoEstimate");
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
                    OnPropertyChanged(nameof(MissingList));
                    OnPropertyChanged(nameof(MissingCount));
                });
                IsLoading = false;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

    }

}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data;
using System.Threading.Tasks;
using System;
using PlaziatTools;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class WelcomeWindowViewModel : BaseViewModel
    {

        private readonly WindowsService winManager;
        private readonly MageekCore.MageekService mageek;

        public WelcomeWindowViewModel(
            WindowsService winManager,
            MageekCore.MageekService mageek
        ){
            this.mageek = mageek;
            this.winManager = winManager;
        }

        [ObservableProperty] bool updateAvailable = false;
        [ObservableProperty] bool canLaunch = false;
        [ObservableProperty] bool isLoading = false;
        [ObservableProperty] string message = "Welcome";

        public async Task Init()
        {
            IsLoading = true;
            await Task.Delay(100);
            Message = "Init...";
            var retour = await mageek.Initialize();
            switch (retour)
            {
                case MageekInitReturn.Error:
                    CanLaunch = false;
                    UpdateAvailable = false;
                    Message = "Error";
                    break;
                case MageekInitReturn.MtgUpToDate:
                    CanLaunch = true;
                    UpdateAvailable = false;
                    Message = "Up to date";
                    Launch();
                    break;
                case MageekInitReturn.MtgOutdated:
                    CanLaunch = true;
                    UpdateAvailable = true;
                    Message = "Update available";
                    break;
            }
            IsLoading = false;
        }

        [RelayCommand]
        public async Task Update()
        {
            IsLoading = true;
            CanLaunch = false;
            UpdateAvailable = false;
            Message = "Updating...";
            await Task.Delay(100);
            var retour = await mageek.UpdateMtg();
            switch (retour)
            {
                case MageekUpdateReturn.Success:
                    CanLaunch = true;
                    UpdateAvailable = false;
                    Message = "Updated";
                    break;
                case MageekUpdateReturn.ErrorDownloading:
                    CanLaunch = true;
                    UpdateAvailable = true;
                    Message = "Update failed";
                    break;
                case MageekUpdateReturn.ErrorFetching:
                    CanLaunch = false;
                    UpdateAvailable = false;
                    Message = "/!\\ Fatal Error /!\\"; //TODO backup system
                    break;
            }
            IsLoading = false;
        }

        [RelayCommand]
        public void Launch()
        {
            IsLoading = true;
            winManager.CloseWindow(AppWindowEnum.Welcome);
            winManager.OpenWindow(AppWindowEnum.Main);
            try
            {
                winManager.LoadLayout("Default");
            }
            catch (Exception e) { Logger.Log(e); }
        }

    }

}

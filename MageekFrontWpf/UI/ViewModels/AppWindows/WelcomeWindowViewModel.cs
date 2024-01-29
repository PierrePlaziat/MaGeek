using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class WelcomeWindowViewModel : BaseViewModel
    {

        private readonly WindowsService winManager;
        private readonly MageekService.MageekService mageek;

        public WelcomeWindowViewModel(
            WindowsService winManager,
            MageekService.MageekService mageek
        ){
            this.mageek = mageek;
            this.winManager = winManager;
            this.dialog = dialog;
        }

        [ObservableProperty] bool updateAvailable = false;
        [ObservableProperty] bool canLaunch = false;
        [ObservableProperty] bool isLoading = false;
        [ObservableProperty] string message = "Welcome";

        public async Task Init()
        {
            IsLoading = true;
            dialog.Notif("Test: ", "test message");
            await Task.Delay(100);
            Message = "Init...";
            var retour = await mageek.InitializeService();
            switch (retour)
            {
                case MageekService.MageekInitReturn.Error:
                    CanLaunch = false;
                    UpdateAvailable = false;
                    Message = "Error";
                    break;
                case MageekService.MageekInitReturn.MtgUpToDate:
                    CanLaunch = true;
                    UpdateAvailable = false;
                    Message = "Up to date";
                    break;
                case MageekService.MageekInitReturn.MtgOutdated:
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
                case MageekService.MageekUpdateReturn.Success:
                    CanLaunch = true;
                    UpdateAvailable = false;
                    Message = "Updated";
                    break;
                case MageekService.MageekUpdateReturn.ErrorDownloading:
                    CanLaunch = true;
                    UpdateAvailable = true;
                    Message = "Update failed";
                    break;
                case MageekService.MageekUpdateReturn.ErrorFetching:
                    CanLaunch = false;
                    UpdateAvailable = false;
                    Message = "/!\\ Fatal Error /!\\"; // todo backup system
                    break;
            }
            IsLoading = false;
        }

        [RelayCommand]
        public async Task Launch()
        {
            IsLoading = true;
            winManager.CloseWindow(AppWindowEnum.Welcome);
            winManager.OpenWindow(AppWindowEnum.Main);
        }

    }

}

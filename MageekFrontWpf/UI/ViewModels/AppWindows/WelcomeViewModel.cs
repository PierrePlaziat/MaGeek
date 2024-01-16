using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MageekFrontWpf.UI.ViewModels
{

    public partial class WelcomeWindowViewModel : BaseViewModel
    {

        private readonly WindowsManager winManager;

        public WelcomeWindowViewModel(WindowsManager winManager)
        {
            this.winManager = winManager;
            UpdateCommand = new AsyncRelayCommand(Update);
            LaunchCommand = new RelayCommand(Launch);
        }

        [ObservableProperty] bool updateAvailable = false;
        [ObservableProperty] bool canLaunch = false;
        [ObservableProperty] bool isLoading = false;
        [ObservableProperty] string message = "Welcome";

        public ICommand LaunchCommand { get; }
        public ICommand UpdateCommand { get; }

        public async Task Init()
        {
            IsLoading = true;
            await Task.Delay(100);
            Message = "Init...";
            var retour = await MageekService.MageekService.InitializeService();
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

        public async Task Update()
        {
            IsLoading = true;
            await Task.Delay(100);
            Message = "Updating...";
            var retour = await MageekService.MageekService.UpdateMtg();
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

        public void Launch()
        {
            IsLoading = true;
            winManager.CloseWindow(AppWindowEnum.Welcome);
            winManager.OpenWindow(AppWindowEnum.Main);
        }

    }

}

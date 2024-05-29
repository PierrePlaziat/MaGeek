using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using MageekCore.Data;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class WelcomeWindowViewModel : ObservableViewModel
    {

        private readonly IMageekService mageek;
        private readonly WindowsService winManager;
        private readonly DialogService dialogs;

        public WelcomeWindowViewModel(
            IMageekService mageek,
            WindowsService winManager,
            DialogService dialogs
        )
        {
            this.mageek = mageek;
            this.winManager = winManager;
            this.dialogs = dialogs;
            Message = "Welcome";
        }

        [ObservableProperty] string input_address = "http://192.168.1.10:55666/";
        [ObservableProperty] string input_user;
        [ObservableProperty] string input_pass;
        [ObservableProperty] string message;
        [ObservableProperty] bool isLoading;

        [RelayCommand]
        public async Task Connect()
        {
            IsLoading = true;
            Message = "Connecting";
            var retour = await mageek.Client_Connect(Input_address);
            if (retour == MageekConnectReturn.Success)
            {
                Message = "Connected";
                App.Launch();
            }
            else Message = "Couldnt connect";
            IsLoading = false;
        }

    }

}

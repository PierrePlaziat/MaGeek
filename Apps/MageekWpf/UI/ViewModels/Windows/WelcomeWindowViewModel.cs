using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using MageekCore.Services;

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
                winManager.LaunchApp();
            }
            else Message = "Couldnt connect";
            IsLoading = false;
        }

    }

}

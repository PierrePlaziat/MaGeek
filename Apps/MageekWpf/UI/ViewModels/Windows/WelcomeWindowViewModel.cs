using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using MageekCore.Data;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatTools;

namespace MageekDesktop.UI.ViewModels.AppWindows
{

    public partial class WelcomeWindowViewModel : ObservableViewModel
    {

        private readonly IMageekService mageek;
        private readonly WindowsService winManager;
        private readonly DialogService dialogs;

        private string registeredServer;
        private string registeredUser;

        public WelcomeWindowViewModel(
            IMageekService mageek,
            WindowsService winManager,
            DialogService dialogs
        ){
            this.mageek = mageek;
            this.winManager = winManager;
            this.dialogs = dialogs;
            MageekCore.Data.Paths.InitClient();
            RetrieveRegisteredCredentials();
            Message = "Welcome";
        }

        [ObservableProperty] string input_address;
        [ObservableProperty] string input_user;
        [ObservableProperty] string input_pass;
        [ObservableProperty] string message;
        [ObservableProperty] bool isLoading;

        private void RetrieveRegisteredCredentials()
        {
            IsLoading = true;
            Input_address = "https://127.0.0.1:5000/"; // Unraid:"http://192.168.1.10:55666/";
            Input_user = "Pierre";
            Input_pass = "Pierre0!";
            IsLoading = false;
        }

        [RelayCommand]
        public async Task Connect()
        {
            IsLoading = true;
            Message = "Connecting";
            var success = await mageek.Client_Connect(
                Input_user, 
                Input_pass, 
                Input_address
            );
            if (success != MageekConnectReturn.Success)
            {
                Message = "Couldnt connect";
                IsLoading = false;
                return;
            }
            Message = "Authenticating";
            success = await mageek.Client_Authentify(
                Input_user,
                Input_pass
            );
            if (success == MageekConnectReturn.Success)
            {
                Message = "Launching";
                Logger.Log("Launching for user: " + Input_user);
                App.OnConnected(Input_user);
            }
        }
        
        [RelayCommand]
        public async Task Register()
        {
            IsLoading = true;
            Message = "Connecting";
            var success = await mageek.Client_Connect(
                Input_user,
                Input_pass,
                Input_address
            );
            if (success != MageekConnectReturn.Success)
            {
                Message = "Couldnt connect";
                IsLoading = false;
                return;
            }
            Message = "Registering";
            var retour = await mageek.Client_Register(
                Input_user, 
                Input_pass
            );
            if (retour != MageekConnectReturn.Success)
            {
                Message = "Couldnt register";
                IsLoading = false;
                return;
            }
            Message = "Registered";
            await Connect();
        }

    }

}

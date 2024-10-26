using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekCore.Data;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatTools;
using System;
using System.IO;

namespace MageekDesktopClient.UI.ViewModels.AppWindows
{

    public partial class WelcomeWindowViewModel : ObservableViewModel
    {

        readonly IMageekService mageek;
        [ObservableProperty] string inputAddress;
        [ObservableProperty] string inputUser;
        [ObservableProperty] string inputPass;
        [ObservableProperty] string message;
        [ObservableProperty] bool isLoading;

        public WelcomeWindowViewModel(
            IMageekService mageek
        ){
            Message = "Init";
            this.mageek = mageek;
            IsLoading = true;
            MageekCore.Data.Paths.InitClient();
            LoadCredentials();
            IsLoading = false;
            Message = "Welcome";
        }

        [RelayCommand]
        public async Task Connect()
        {
            IsLoading = true;
            Message = "Connecting";
            var success = await mageek.Client_Connect(
                InputUser, 
                InputPass, 
                InputAddress
            );
            if (success != MageekConnectReturn.Success)
            {
                Message = "Couldnt connect";
                IsLoading = false;
                return;
            }
            Message = "Authenticating";
            success = await mageek.Client_Authentify(
                InputUser,
                InputPass
            );
            if (success == MageekConnectReturn.Success)
            {
                SaveCredentials();
                Message = "Launching";
                Logger.Log("Launching for user: " + InputUser);
                App.OnConnected(InputUser);
            }
            else
            {
                Message = "Failed";
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Register()
        {
            IsLoading = true;
            Message = "Connecting";
            var success = await mageek.Client_Connect(
                InputUser,
                InputPass,
                InputAddress
            );
            if (success != MageekConnectReturn.Success)
            {
                Message = "Couldnt connect";
                IsLoading = false;
                return;
            }
            Message = "Registering";
            var retour = await mageek.Client_Register(
                InputUser, 
                InputPass
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

        private void SaveCredentials()
        {
            Message = "Registering credentials";
            string data = $"{InputAddress},{InputUser},{InputPass}";
            try
            {
                File.WriteAllText(MageekCore.Data.Paths.File_RegCred, data);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void LoadCredentials()
        {
            Message = "Retrieving credentials";
            try
            {
                string data = File.ReadAllText(MageekCore.Data.Paths.File_RegCred);
                var parts = data.Split(',');
                if (parts.Length == 3)
                {
                    InputAddress = parts[0];
                    InputUser = parts[1];
                    InputPass = parts[2];
                }
                else
                {
                    Logger.Log("Invalid data", LogLevels.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

    }

}

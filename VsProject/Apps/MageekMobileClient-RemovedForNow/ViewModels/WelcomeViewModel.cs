using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekClient.Services;
using MageekCore.Services;
using MageekMobile.Views;

namespace MageekMobile.ViewModels
{

    public partial class WelcomeViewModel : ViewModel
    {

        [ObservableProperty] string inputAddress = "http://192.168.1.10:55666/";
        [ObservableProperty] string inputUser= "";
        [ObservableProperty] string inputPassword = "";
        [ObservableProperty] string message = "";

        private INavigationService navigation;
        private IMageekService client;

        public WelcomeViewModel(
            INavigationService navigation,
            IMageekService client)
        {
            this.client = client;
            this.navigation = navigation;
        }

        [RelayCommand]
        async Task Skip()
        {
            await navigation.NavigateAsync(nameof(GameView));
        }
        
        [RelayCommand]
        async Task Connect()
        {
            Message = "Connecting";
            IsBusy = true;
            var retour = await client.Client_Connect(InputAddress);
            if (retour==MageekCore.Data.MageekConnectReturn.Success)
            {
                await navigation.NavigateAsync(nameof(CollecView));
            }
            else
            {
                Message = "Couldnt connect";
                IsBusy = false;
            }
        }

    }

}

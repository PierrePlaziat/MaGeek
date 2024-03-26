using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekMaui.Views;
using Microsoft.Extensions.Logging;

namespace MageekMaui.ViewModels
{

    public partial class WelcomeViewModel : ViewModel
    {

        [ObservableProperty] string inputAddress = "";
        [ObservableProperty] string inputPassword = "";
        [ObservableProperty] string message = "";

        private INavigationService navigation;
        private MageekClient client;

        public WelcomeViewModel(
            INavigationService navigation,
            MageekClient client)
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

using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MageekFrontWpf.ViewModels
{

    public class WelcomeViewModel : BaseViewModel
    {

        #region Construction

        private readonly WinManager winManager;

        private bool updateAvailable = false;
        public bool UpdateAvailable
        {
            get { return updateAvailable; }
            set { updateAvailable = value; OnPropertyChanged(); }
        }
        public ICommand UpdateCommand { get; }

        private bool canLaunch = false;
        public bool CanLaunch
        {
            get { return canLaunch; }
            set { canLaunch = value; OnPropertyChanged(); }
        }
        public ICommand LaunchCommand { get; }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        string message;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        public WelcomeViewModel(WinManager winManager)
        {
            this.winManager = winManager;
            UpdateCommand = new AsyncRelayCommand(Update);
            LaunchCommand = new AsyncRelayCommand(Launch);
        }

        #endregion

        #region Usage

        public async Task Init()
        {
            IsLoading = true;
            await Task.Delay(100);
            Message = "Init...";
            var retour = await MageekService.MageekService.InitializeService();
            switch(retour)
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
        
        public async Task Launch()
        {
            IsLoading = true;
            await Task.Delay(100);
            winManager.CloseWindow(this);
            winManager.LaunchMainWin();
        }

        #endregion

    }

}

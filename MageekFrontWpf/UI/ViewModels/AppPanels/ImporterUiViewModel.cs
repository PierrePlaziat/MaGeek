using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using System.Timers;
using System.Windows;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class ImporterUiViewModel : BaseViewModel
    {

        Timer loopTimer;
        private CollectionImporter importer;

        public ImporterUiViewModel() {}

        public ImporterUiViewModel(CollectionImporter Importer)
        {
            importer = Importer;
            loopTimer = new Timer(1000);
            loopTimer.AutoReset = true;
            loopTimer.Elapsed += LoopTimer;
            loopTimer.Start();
        }

        [ObservableProperty] int importCount = 0;
        [ObservableProperty] int currentPercent = 0;
        [ObservableProperty] string state = "";
        [ObservableProperty] bool queueFilled = false;
        [ObservableProperty] string infoText = "";

        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            ImportCount = importer.PendingCount;
            CurrentPercent = importer.WorkerProgress;
            State = importer.Message;
            QueueFilled = importer.PendingCount > 0;
            InfoText = importer.InfoText;
        }

        [RelayCommand]
        private void ButtonPlayCommand()
        {
            importer.Play();
        }

        [RelayCommand]
        private void ButtonPauseCommand()
        {
            importer.Pause();
        }

        [RelayCommand]
        private void ButtonCancelCommand() 
        {
            importer.CancelAll();
        }

    }
}

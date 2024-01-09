using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using System.Timers;
using System.Windows;

namespace MageekFrontWpf.ViewModels
{
    public class ImporterUiViewModel : BaseViewModel
    {

        Timer loopTimer;
        private CollectionImporter importer;

        public Visibility HasImportInQueue
        {
            get { return importer.PendingCount > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }


        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            UpdateImporterInfos();
            OnPropertyChanged(nameof(HasImportInQueue));
        }
        private void ConfigureTimer()
        {
            loopTimer = new Timer(1000);
            loopTimer.AutoReset = true;
            loopTimer.Elapsed += LoopTimer;
            loopTimer.Start();
        }

        public string InfoText { get { return importer.InfoText; } }

        int importCount = 0;
        public int ImportCount
        {
            get { return importCount; }
            set { importCount = value; OnPropertyChanged(); }
        }

        int currentPercent = 0;
        public int CurrentPercent
        {
            get { return currentPercent; }
            set { currentPercent = value; OnPropertyChanged(); }
        }

        string state = "";

        public string State
        {
            get { return state; }
            set { state = value; OnPropertyChanged(); }
        }

        public ImporterUiViewModel(CollectionImporter Importer)
        {
            importer = Importer;
            ConfigureTimer();
        }
        private void UpdateImporterInfos()
        {
            ImportCount = importer.PendingCount;
            CurrentPercent = importer.WorkerProgress;
            State = importer.Message;
            OnPropertyChanged(nameof(InfoText));
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            importer.Play();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            importer.Pause();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            importer.CancelAll();
        }

    }
}

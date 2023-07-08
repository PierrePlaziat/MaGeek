using System.Timers;
using System.Windows;

namespace MaGeek.UI.Menus
{

    public partial class ImporterUi : TemplatedUserControl
    {

        Timer loopTimer;

        public Visibility HasImportInQueue
        {
            get { return App.Importer.PendingCount>0? Visibility.Visible : Visibility.Collapsed; }
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

        public string InfoText { get { return App.Importer.InfoText; } }

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

        public ImporterUi()
        {
            InitializeComponent();
            ConfigureTimer();
            DataContext = this;
        }
        private void UpdateImporterInfos()
        {
            ImportCount = App.Importer.PendingCount;
            CurrentPercent = App.Importer.WorkerProgress;
            State = App.Importer.Message;
            OnPropertyChanged(nameof(InfoText));
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            App.Importer.Play();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            App.Importer.Pause();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Importer.CancelAll();
        }

    }
}

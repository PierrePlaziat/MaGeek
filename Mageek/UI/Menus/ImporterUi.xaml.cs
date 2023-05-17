using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaGeek.UI.Menus
{
    /// <summary>
    /// Logique d'interaction pour ImporterUi.xaml
    /// </summary>
    public partial class ImporterUi : UserControl, INotifyPropertyChanged
    {

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        Timer loopTimer;

        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            UpdateImporterInfos();
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

using MaGeek.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace MaGeek.UI.CustomControls
{

    public partial class StateBar : UserControl, INotifyPropertyChanged
    {

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        Timer loopTimer;

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

        public string InfoText { get { return App.CARDS.Importer.InfoText; } }

        public StateBar()
        {
            InitializeComponent();
            ConfigureTimer();
            DataContext = this;
        }

        private void ConfigureTimer()
        {
            loopTimer = new Timer(1000);
            loopTimer.AutoReset = true;
            loopTimer.Elapsed += LoopTimer;
            loopTimer.Start();
        }

        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            ImportCount = App.CARDS.Importer.PendingCount;
            CurrentPercent = App.CARDS.Importer.WorkerProgress;
            State = App.CARDS.Importer.Message;
            OnPropertyChanged("InfoText");
        }

    }

}

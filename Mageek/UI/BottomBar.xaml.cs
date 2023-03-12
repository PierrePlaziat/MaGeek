using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        public string SelectedString
        {
            get {
                string s = "";
                if (App.State.SelectedDeck != null) s += "Selected Deck : " + App.State.SelectedDeck.Title;
                s += " | ";
                if (App.State.SelectedCard != null) s += "Selected Card : " + App.State.SelectedCard.CardId;
                return s;
            }
        }

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

        public string InfoText { get { return App.Biz.Importer.InfoText; } }

        public StateBar()
        {
            InitializeComponent();
            ConfigureTimer();
            DataContext = this;

            //App.CARDS.Importer.Play();

            App.Events.CardSelectedEvent += STATE_CardSelectedEvent;
            App.Events.SelectDeckEvent += STATE_SelectDeckEvent; ;
        }

        private void STATE_SelectDeckEvent(Data.Entities.MagicDeck deck)
        {
            OnPropertyChanged("SelectedString");
        }

        private void STATE_CardSelectedEvent(Data.Entities.MagicCard Card)
        {
            OnPropertyChanged("SelectedString");
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
            ImportCount = App.Biz.Importer.PendingCount;
            CurrentPercent = App.Biz.Importer.WorkerProgress;
            State = App.Biz.Importer.Message;
            OnPropertyChanged("InfoText");
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            App.Biz.Importer.Play();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            App.Biz.Importer.Pause();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Biz.Importer.CancelAll();
        }

    }

}

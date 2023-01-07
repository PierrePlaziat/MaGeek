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
                if (App.STATE.SelectedDeck != null) s += "Selected Deck : " + App.STATE.SelectedDeck.Title;
                s += " | ";
                if (App.STATE.SelectedCard != null) s += "Selected Card : " + App.STATE.SelectedCard.CardId;
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

        public string InfoText { get { return App.CARDS.Importer.InfoText; } }

        public StateBar()
        {
            InitializeComponent();
            ConfigureTimer();
            DataContext = this;

            //App.CARDS.Importer.Play();

            App.STATE.CardSelectedEvent += STATE_CardSelectedEvent;
            App.STATE.SelectDeckEvent += STATE_SelectDeckEvent; ;
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
            ImportCount = App.CARDS.Importer.PendingCount;
            CurrentPercent = App.CARDS.Importer.WorkerProgress;
            State = App.CARDS.Importer.Message;
            OnPropertyChanged("InfoText");
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            App.CARDS.Importer.Play();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            App.CARDS.Importer.Pause();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            App.CARDS.Importer.CancelAll();
        }

    }

}

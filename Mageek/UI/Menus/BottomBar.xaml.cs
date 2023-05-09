using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using MaGeek.AppData.Entities;

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

        public string SelectedDeckString
        {
            get {
                string s = "";
                if (App.State.SelectedDeck != null) s = App.State.SelectedDeck.Title;
                return s;
            }
        }
        
        public string SelectedCardString
        {
            get {
                string s = "";
                if (App.State.SelectedCard != null) s = App.State.SelectedCard.CardId;
                return s;
            }
        }
        
        public string Msg
        {
            get {
                string s = "";
                s += App.State.OutputMessages.LastOrDefault();
                return s;
            }
        }
        
        public string Msgs
        {
            get {
                string s = "";
                foreach (var ss in App.State.OutputMessages) s += "> " + ss + "\n";
                if (App.State.OutputMessages.Count > 1) s = s.Remove(s.Length - 2);
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

        public string InfoText { get { return App.Importer.InfoText; } }

        public StateBar()
        {
            InitializeComponent();
            ConfigureTimer();
            DataContext = this;
            App.Events.CardSelectedEvent += STATE_CardSelectedEvent;
            App.Events.SelectDeckEvent += STATE_SelectDeckEvent; ;
        }

        private void STATE_SelectDeckEvent(Deck deck)
        {
            OnPropertyChanged(nameof(SelectedDeckString));
        }

        private void STATE_CardSelectedEvent(CardModel Card)
        {
            OnPropertyChanged(nameof(SelectedCardString));
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
            UpdateImporterInfos();
            UpdateMsgs();
        }

        private void UpdateMsgs()
        {
            OnPropertyChanged(nameof(Msg));
            OnPropertyChanged(nameof(Msgs));
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

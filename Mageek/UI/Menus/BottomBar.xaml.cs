using System.Linq;
using System.Timers;
using MaGeek.AppBusiness.Entities;

namespace MaGeek.UI.Menus
{

    public partial class StateBar : TemplatedUserControl
    {

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
                s += AppLogger.OutputMessages.LastOrDefault();
                return s;
            }
        }
        
        public string Msgs
        {
            get {
                string s = "";
                foreach (var ss in AppLogger.OutputMessages) s += "> " + ss + "\n";
                s = s.Remove(s.Length - 1);
                return s;
            }
        }

        Timer loopTimer;

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
            UpdateMsgs();
        }

        private void UpdateMsgs()
        {
            OnPropertyChanged(nameof(Msg));
            OnPropertyChanged(nameof(Msgs));
        }

    }

}

using MaGeek.Framework.Utils;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MaGeek.UI
{
    /// <summary>
    /// Logique d'interaction pour OutputMsg.xaml
    /// </summary>
    public partial class OutputMsg : TemplatedUserControl
    {

        public string Msg
        {
            get
            {
                string s = "";
                try
                {
                    s += Log.Messages.LastOrDefault();
                }
                catch { }
                
                return s;
            }
        }

        public string Msgs
        {
            get
            {
                string s = "";
                foreach (var ss in Log.Messages) s += "> " + ss + "\n";
                if (s.Length>0) s = s.Remove(s.Length - 1);
                return s;
            }
        }

        Timer loopTimer;

        public OutputMsg()
        {
            DataContext = this;
            InitializeComponent();
            ConfigureTimer();
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

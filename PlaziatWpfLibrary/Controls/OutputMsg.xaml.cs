using System.Timers;
using PlaziatWpf.Mvvm;
using Timer = System.Timers.Timer;

namespace PlaziatWpf.Controls
{
    /// <summary>
    /// Logique d'interaction pour OutputMsg.xaml
    /// </summary>
    public partial class OutputMsg : BaseUserControl
    {

        public string Msg
        {
            get
            {
                string s = "";
                try
                {
                    //s += Log.Messages.LastOrDefault();
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
                //foreach (var ss in Log.Messages) s += "> " + ss + "\n";
                //if (s.Length>0) s = s.Remove(s.Length - 1);
                return s;
            }
        }

        private readonly Timer loopTimer;

        public OutputMsg()
        {
            DataContext = this;
            InitializeComponent();
            // ConfigureTimer
            loopTimer = new Timer(1000)
            { 
                Interval = 1000, 
                AutoReset = true
            };
            loopTimer.Elapsed += LoopTimer;
            loopTimer.Start();
        }

        private void LoopTimer(object? sender, ElapsedEventArgs e)
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

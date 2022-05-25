using MaGeek.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class SelectedDeck : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        public MagicDeck CurrentDeck { 
            get { 
                return App.CurrentDeck; 
            } 
            set { 
                App.CurrentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged("DevotionB");
                OnPropertyChanged("DevotionB");
                OnPropertyChanged("DevotionW");
                OnPropertyChanged("DevotionU");
                OnPropertyChanged("DevotionG");
                OnPropertyChanged("DevotionR");
            } 
        }

        public ObservableCollection<MagicCard> Cards
        {
            get {
                if (App.CurrentDeck == null) return null;
                return App.CurrentDeck.Cards as ObservableCollection<MagicCard>; 
            }
        }

        #region Devotion

        public int DevotionB
        {
            get
            {
                if (App.CurrentDeck == null || App.CurrentDeck.Cards == null) return 0;
                int devotion = 0;
                foreach (var c in App.CurrentDeck.Cards) devotion += c.DevotionB;
                return devotion; 
            }
        }
        public int DevotionW
        {
            get
            {
                if (App.CurrentDeck == null || App.CurrentDeck.Cards == null) return 0;
                int devotion = 0;
                foreach (var c in App.CurrentDeck.Cards) devotion += c.DevotionW;
                return devotion;
            }
        }
        public int DevotionU
        {
            get
            {
                if (App.CurrentDeck == null || App.CurrentDeck.Cards == null) return 0;
                int devotion = 0;
                foreach (var c in App.CurrentDeck.Cards) devotion += c.DevotionU;
                return devotion;
            }
        }
        public int DevotionG
        {
            get
            {
                if (App.CurrentDeck == null || App.CurrentDeck.Cards == null) return 0;
                int devotion = 0;
                foreach (var c in App.CurrentDeck.Cards) devotion += c.DevotionG;
                return devotion;
            }
        }
        public int DevotionR
        {
            get
            {
                if (App.CurrentDeck == null || App.CurrentDeck.Cards == null) return 0;
                int devotion = 0;
                foreach (var c in App.CurrentDeck.Cards) devotion += c.DevotionR;
                return devotion;
            }
        }

        #endregion

        #endregion

        #region CTOR

        public SelectedDeck()
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion



    }

}

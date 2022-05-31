using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckTable : UserControl, INotifyPropertyChanged
    {


        #region Attributes

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck { 
            get { return currentDeck; }
            set { currentDeck = value; OnPropertyChanged(); }
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
        }

        public ObservableCollection<MagicCard> cards;
        public ObservableCollection<MagicCard> Cards
        {
            get { return cards; }
            set { cards = value; OnPropertyChanged(); }
        }

        #endregion

        #region CTOR

        public DeckTable()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
        }

        #endregion

    }

}

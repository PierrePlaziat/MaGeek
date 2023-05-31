using MaGeek.Entities;

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

        public StateBar()
        {
            InitializeComponent();
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

    }

}

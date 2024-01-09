using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.BaseMvvm;

namespace MageekFrontWpf.ViewModels
{
    public class StateBarViewModel : BaseViewModel
    {
        private AppEvents events;
        private AppState state;

        public StateBarViewModel(AppEvents events,AppState state)
        {
            this.events = events;
            this.state = state;
            events.CardSelectedEvent += STATE_CardSelectedEvent;
            events.SelectDeckEvent += STATE_SelectDeckEvent; ;
        }

        public string SelectedDeckString
        {
            get
            {
                string s = "";
                if (state.SelectedDeck != null) s = state.SelectedDeck.Title;
                return s;
            }
        }

        public string SelectedCardString
        {
            get
            {
                string s = "";
                if (state.SelectedCard != null) s = state.SelectedCard.ArchetypeId;
                return s;
            }
        }

        private void STATE_SelectDeckEvent(string deckId)
        {
            OnPropertyChanged(nameof(SelectedDeckString));
        }

        private void STATE_CardSelectedEvent(string CardUuid)
        {
            OnPropertyChanged(nameof(SelectedCardString));
        }

    }
}
using MaGeek.Data.Entities;

namespace MaGeek.Events
{

    public class AddToDeckEventArgs
    {

        public MagicDeck Deck { get; set; }
        public MagicCard Card { get; set; }

        public AddToDeckEventArgs(MagicCard card, MagicDeck deck)
        {
            Deck = deck;
            Card = card;
        }

    }

}

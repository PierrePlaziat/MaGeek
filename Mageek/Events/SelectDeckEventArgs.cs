using MaGeek.Data.Entities;

namespace MaGeek.Events
{

    public class SelectDeckEventArgs
    {

        public MagicDeck Deck { get; set; }

        public SelectDeckEventArgs(MagicDeck deck)
        {
            Deck = deck;
        }

    }

}

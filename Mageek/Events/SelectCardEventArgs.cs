using MaGeek.Data.Entities;

namespace MaGeek.Events
{

    public class SelectCardEventArgs
    {

        public MagicCard Card { get; set; }

        public SelectCardEventArgs(MagicCard card)
        {
            Card = card;
        }

    }

}

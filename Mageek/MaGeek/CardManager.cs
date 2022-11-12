using MaGeek.Data.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MaGeek
{

    public class CardManager
    {

        public ImportManager Importer { get; } = new();

        public MageekUtils Utils { get; } = new();

        public ObservableCollection<MagicCard> AllCards
        {
            get
            {
                App.DB.cards.Load();
                return App.DB.cards.Local.ToObservableCollection();
            }
        }
        public ObservableCollection<MagicDeck> AllDecks
        {
            get
            {
                App.DB.decks.Load();
                return App.DB.decks.Local.ToObservableCollection();
            }
        }

        public List<string> AllTags
        {
            get
            { 
                List<string> tags = new List<string>();
                tags.Add("");

                foreach (var tag in App.DB.Tags.GroupBy(x=>x.Tag).Select(x=>x.First()))
                    tags.Add(tag.Tag);
                return tags;
            }
        }

    }

}

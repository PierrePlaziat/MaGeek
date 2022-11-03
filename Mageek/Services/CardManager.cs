using MaGeek.Data.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MaGeek.Data
{

    public class MageekManager
    {

        public MageekImporter Importer { get; } = new();

        public MageekUtils Utils { get; } = new();

        public ObservableCollection<MagicCard> AllCards
        {
            get
            {
                App.Database.cards.Load();
                return App.Database.cards.Local.ToObservableCollection();
            }
        }
        public ObservableCollection<MagicDeck> AllDecks
        {
            get
            {
                App.Database.decks.Load();
                return App.Database.decks.Local.ToObservableCollection();
            }
        }

        public List<string> AllTags
        {
            get
            { 
                List<string> tags = new List<string>();
                tags.Add("");

                foreach (var tag in App.Database.Tags.GroupBy(x=>x.Tag).Select(x=>x.First()))
                    tags.Add(tag.Tag);
                return tags;
            }
        }

    }

}

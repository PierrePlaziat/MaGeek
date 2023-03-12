using MaGeek.Data.Entities;
using MaGeek.Entities;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Business Relative Work
    /// </summary>
    public class AppBiz
    {

        private CardDatabase db;
        public CardImporter Importer { get; }
        public MageekUtils Utils { get; }

        public ObservableCollection<MagicCard> AllCards
        {
            get
            {
                db.cards.Load();
                return db.cards.Local.ToObservableCollection();
            }
        }
        public ObservableCollection<MagicDeck> AllDecks
        {
            get
            {
                db.decks.Load();
                return db.decks.Local.ToObservableCollection();
            }
        }
        public List<string> AllTags
        {
            get
            {
                List<string> tags = new List<string>();
                tags.Add("");

                foreach (var tag in db.Tags.GroupBy(x => x.Tag).Select(x => x.First()))
                    tags.Add(tag.Tag);
                return tags;
            }
        }

        public AppBiz()
        {
            db = new CardDatabase();
            db.InitDb();
            Importer = new CardImporter(db);
            Utils = new MageekUtils(db);
        }

    }

}

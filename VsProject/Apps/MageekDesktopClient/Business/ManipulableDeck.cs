using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using MageekCore.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MageekCore.Data.Collection.Entities;
using MageekCore.Services;
using MageekDesktopClient.DeckTools;
using MageekDesktopClient.Framework;
using PlaziatWpf.Services;
using PlaziatTools;

namespace MageekDesktopClient.MageekTools.DeckTools
{

    public partial class ManipulableDeck : ObservableObject
    {

        //TODO only do that on demand, change the design
        //public string StandardOk { get { return manipulator.DeckValidity(Entries, "Standard").Result; } }
        //public string CommanderOk { get { return manipulator.DeckValidity(Entries, "Commander").Result; } }
        //public int OwnedRatio { get { return manipulator.GetRatio(Entries).Result; } }

        #region Construction

        private IMageekService mageek;
        private SessionBag session;
        private DeckManipulator manipulator;

        public ManipulableDeck(IMageekService mageek, SessionBag session, DeckManipulator manipulator)
        {
            this.mageek = mageek;
            this.session = session;
            this.manipulator = manipulator;
        }

        #endregion

        [ObservableProperty] ObservableCollection<ManipulableDeckEntry> entries;
        [ObservableProperty] Deck header;
        [ObservableProperty] PathFigure curve;

        public async Task OpenDeck(Deck deck)
        {
            Logger.Log("Oppening existing deck...");
            Entries = new ObservableCollection<ManipulableDeckEntry>(await manipulator.GetEntriesFromDeck(deck.DeckId));
            Header = deck;
            DeckChanged();
            Logger.Log("...done");
        }

        public async Task OpenDeck(List<DeckCard> importLines)
        {
            Logger.Log("Oppening importation...");
            Entries = new ObservableCollection<ManipulableDeckEntry>(await manipulator.GetEntriesFromImport(importLines)); 
            Header = new Deck()
            {
                Title = "Importation ",
                Description = "Imported at : "+DateTime.Now,
            };
            DeckChanged();
            Logger.Log("...done");
        }

        public async Task OpenDeck(Preco preco)
        {
            Logger.Log("Oppening preco...");
            Entries = new ObservableCollection<ManipulableDeckEntry>(await manipulator.GetEntriesFromPreco(preco));
            Header = manipulator.GetHeaderFromPreco(preco);
            DeckChanged();
            Logger.Log("...done");
        }

        public async Task AddCard(string uuid)
        {
            List<string> newVariants = await mageek.Cards_UuidsForGivenCardUuid(uuid);
            ManipulableDeckEntry previousEntry = Entries
                .Where(x => newVariants.Contains(x.Line.CardUuid)) // Checks if the any of the variants of the card is in the deck
                .FirstOrDefault();
            if (previousEntry != null) MoreOf(previousEntry);
            else
            {
                var entry = new ManipulableDeckEntry()
                {
                    Card = await mageek.Cards_GetData(uuid),
                    Line = new DeckCard()
                    {
                        CardUuid = uuid,
                        Quantity = 1,
                        RelationType = 0,
                    }
                };
                Entries.Add(entry);
                DeckChanged();
            }
        }

        public void LessOf(ManipulableDeckEntry entry)
        {
            if (entry.Line.Quantity <= 0)
            {
                Entries.Remove(entry);
                return;
            }
            entry.Line.Quantity--;
            DeckChanged();
        }

        public void MoreOf(ManipulableDeckEntry entry)
        {
            entry.Line.Quantity++;
            DeckChanged();
        }

        private void DeckChanged()
        {
            Header.DeckColors = manipulator.GetColors(Entries);
            Header.CardCount = manipulator.CountQuantity(Entries);
            Curve = manipulator.GetManaCurve(this);
        }

        public async Task SaveDeck()
        {
            //DeckChanged(); //TODO Safety measure, probably not needed, check perf before decide
            Logger.Log("Saving...");
            await mageek.Decks_Save(session.UserName, Header, manipulator.GetSavableCardList(Entries));
            Logger.Log("...done");
            WeakReferenceMessenger.Default.Send(new UpdateDeckListMessage(Header.DeckId));
        }

        #region Accessors

        public IEnumerable<ManipulableDeckEntry> entries_Deck { get { return Entries.Where(x => x.Line.RelationType == 0).OrderBy(x=>x.Card.FaceConvertedManaCost); } }
        public IEnumerable<ManipulableDeckEntry> entries_Commanders { get { return Entries.Where(x => x.Line.RelationType == 1); } }
        public IEnumerable<ManipulableDeckEntry> entries_Side { get { return Entries.Where(x => x.Line.RelationType == 2); } }

        public IEnumerable<ManipulableDeckEntry> deck_Creatures { get { return entries_Deck.Where(x => x.Card.Type.Contains("Creature")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Instants { get { return entries_Deck.Where(x => x.Card.Type.Contains("Instant")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Sorceries { get { return entries_Deck.Where(x => x.Card.Type.Contains("Sorcery")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Enchantments { get { return entries_Deck.Where(x => x.Card.Type.Contains("Enchantment")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Artifacts { get { return entries_Deck.Where(x => x.Card.Type.Contains("Artifact")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Lands { get { return entries_Deck.Where(x => x.Card.Type.Contains("Land")); } }
        public IEnumerable<ManipulableDeckEntry> deck_BasicLands { get { return deck_Lands.Where(x => x.Card.Type.Contains("Basic")); } }
        public IEnumerable<ManipulableDeckEntry> deck_SpecialLands { get { return deck_Lands.Where(x => !x.Card.Type.Contains("Basic")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Other
        {
            get
            {
                return entries_Deck.Where(x =>
                    !x.Card.Type.Contains("Creature") &&
                    !x.Card.Type.Contains("Instant") &&
                    !x.Card.Type.Contains("Sorcery") &&
                    !x.Card.Type.Contains("Enchantment") &&
                    !x.Card.Type.Contains("Artifact") &&
                    !x.Card.Type.Contains("Land"));
            }
        }

        public IEnumerable<ManipulableDeckEntry> deck_Cmc0 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 0); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc1 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 1); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc2 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 2); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc3 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 3); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc4 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 4); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc5 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 5); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc6 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 6); } }
        public IEnumerable<ManipulableDeckEntry> deck_Cmc7 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 7); } }
        public IEnumerable<ManipulableDeckEntry> deck_CmcBig { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost >= 8); } }

        public bool HasCmc0 { get { return deck_Cmc0.Any(); } }
        public bool HasCmc1 { get { return deck_Cmc1.Any(); } }
        public bool HasCmc2 { get { return deck_Cmc2.Any(); } }
        public bool HasCmc3 { get { return deck_Cmc3.Any(); } }
        public bool HasCmc4 { get { return deck_Cmc4.Any(); } }
        public bool HasCmc5 { get { return deck_Cmc5.Any(); } }
        public bool HasCmc6 { get { return deck_Cmc6.Any(); } }
        public bool HasCmc7 { get { return deck_Cmc7.Any(); } }
        public bool HasCmcBig { get { return deck_CmcBig.Any(); } }

        public bool HasCommandant { get { return entries_Commanders.Any(); } }
        public bool HasSide { get { return entries_Side.Any(); } }
        public bool HasLands { get { return deck_Lands.Any(); } }
        public bool HasCreatures { get { return deck_Creatures.Any(); } }
        public bool HasInstants { get { return deck_Instants.Any(); } }
        public bool HasSorceries { get { return deck_Sorceries.Any(); } }
        public bool HasEnchantments { get { return deck_Enchantments.Any(); } }
        public bool HasArtifacts { get { return deck_Artifacts.Any(); } }
        public bool HasOther { get { return deck_Other.Any(); } }

        public int count_All { get { return manipulator.CountQuantity(Entries); } }
        public int count_Creature { get { return manipulator.CountQuantity(deck_Creatures); } }
        public int count_instant { get { return manipulator.CountQuantity(deck_Instants); } }
        public int count_Sorcery { get { return manipulator.CountQuantity(deck_Sorceries); } }
        public int count_Enchantment { get { return manipulator.CountQuantity(deck_Enchantments); } }
        public int count_Artifact { get { return manipulator.CountQuantity(deck_Artifacts); } }
        public int count_Land { get { return manipulator.CountQuantity(deck_Lands); } }
        public int count_BasicLand { get { return manipulator.CountQuantity(deck_BasicLands); } }
        public int count_specialLand { get { return manipulator.CountQuantity(deck_SpecialLands); } }
        public int count_Other { get { return manipulator.CountQuantity(deck_Other); } }

        public int DevotionB { get { return manipulator.CountDevotion(entries_Deck, 'B'); } }
        public int DevotionW { get { return manipulator.CountDevotion(entries_Deck, 'W'); } }
        public int DevotionU { get { return manipulator.CountDevotion(entries_Deck, 'U'); } }
        public int DevotionR { get { return manipulator.CountDevotion(entries_Deck, 'R'); } }
        public int DevotionG { get { return manipulator.CountDevotion(entries_Deck, 'G'); } }

        #endregion

    }

}

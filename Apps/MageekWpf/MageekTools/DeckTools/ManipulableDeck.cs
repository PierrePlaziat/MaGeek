using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using MageekCore.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MageekFrontWpf.Framework.AppValues;
using MageekCore.Data.Collection.Entities;
using MageekCore.Services;

namespace MageekFrontWpf.MageekTools.DeckTools
{

    public partial class ManipulableDeck : ObservableObject
    {

        IMageekService mageek;
        DeckManipulator deckManip;

        public ManipulableDeck(IMageekService mageek, DeckManipulator deckManip)
        {
            this.mageek = mageek;
            this.deckManip = deckManip;
        }

        [ObservableProperty] Deck header;
        [ObservableProperty] ObservableCollection<ManipulableDeckEntry> entries;

        public async Task OpenDeck(Deck deck)
        {
            Header = deck;
            Entries = new ObservableCollection<ManipulableDeckEntry>(await deckManip.GetEntriesFromDeck(deck.DeckId));
        }
        
        public async Task OpenDeck(List<DeckCard> importLines)
        {
            Header = new Deck()
            {
                Title = "Importation ",
                Description = "Imported at : "+DateTime.Now,
            };
            Entries = new ObservableCollection<ManipulableDeckEntry>(await deckManip.GetEntriesFromImport(importLines));
            Header.DeckColors = deckManip.GetColors(Entries);
            Header.CardCount = deckManip.CountQuantity(Entries);
        }

        public async Task OpenDeck(Preco preco)
        {
            Header = deckManip.GetHeaderFromPreco(preco);
            Entries = new ObservableCollection<ManipulableDeckEntry>(await deckManip.GetEntriesFromPreco(preco));
            Header.DeckColors = deckManip.GetColors(Entries);
            Header.CardCount = deckManip.CountQuantity(Entries);
        }

        public async Task SaveDeck()
        {
            Header.DeckColors = DeckColors;
            Header.CardCount = count_All;
            await mageek.Decks_Save(Header, deckManip.GetSavableCardList(Entries));
            WeakReferenceMessenger.Default.Send(new UpdateDeckListMessage(Header.DeckId));
        }

        public async Task AddCard(string uuid)
        {
            var newCard = await mageek.Cards_GetData(uuid);

            List<string> newVariants = await mageek.Cards_UuidsForGivenCardUuid(uuid);

            ManipulableDeckEntry previousEntry = Entries
                .Where(x => newVariants.Contains(x.Line.CardUuid))
                .FirstOrDefault();
            if (previousEntry != null)
            {
                MoreOf(previousEntry);
            }
            else
            {
                var v = new ManipulableDeckEntry()
                {
                    Card = await mageek.Cards_GetData(uuid),
                    Line = new DeckCard()
                    {
                        CardUuid = uuid,
                        Quantity = 1,
                        RelationType = 0,
                    }
                };
                Entries.Add(v);
                Header.CardCount++;
            }
        }

        public void LessOf(ManipulableDeckEntry entry)
        {
            entry.Line.Quantity--;
            if (entry.Line.Quantity < 0)
            {
                entry.Line.Quantity = 0;
            }
            else
            {
                Header.CardCount--;
            }
        }

        public void MoreOf(ManipulableDeckEntry entry)
        {
            entry.Line.Quantity++;
            Header.CardCount++;
        }

        #region Accessors

        public string DeckColors { get { return deckManip.GetColors(Entries); } }

        public IEnumerable<ManipulableDeckEntry> entries_Deck { get { return Entries.Where(x=> x.Line.RelationType == 0); } }
        public IEnumerable<ManipulableDeckEntry> entries_Commanders { get { return Entries.Where(x=> x.Line.RelationType == 1); } }
        public IEnumerable<ManipulableDeckEntry> entries_Side { get { return Entries.Where(x=> x.Line.RelationType == 2); } }

        public IEnumerable<ManipulableDeckEntry> deck_Creatures { get { return entries_Deck.Where(x => x.Card.Type.Contains("Creature")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Instants { get { return entries_Deck.Where(x => x.Card.Type.Contains("Instant")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Sorceries { get { return entries_Deck.Where(x => x.Card.Type.Contains("Sorcery")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Enchantments { get { return entries_Deck.Where(x => x.Card.Type.Contains("Enchantment")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Artifacts { get { return entries_Deck.Where(x => x.Card.Type.Contains("Artifact")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Lands { get { return entries_Deck.Where(x => x.Card.Type.Contains("Land")); } }
        public IEnumerable<ManipulableDeckEntry> deck_BasicLands { get { return deck_Lands.Where(x => x.Card.Type.Contains("Basic")); } }
        public IEnumerable<ManipulableDeckEntry> deck_SpecialLands { get { return deck_Lands.Where(x => ! x.Card.Type.Contains("Basic")); } }
        public IEnumerable<ManipulableDeckEntry> deck_Other 
        {
            get {
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
        public bool HasCreatures{ get { return deck_Creatures.Any(); } }
        public bool HasInstants{ get { return deck_Instants.Any(); } }
        public bool HasSorceries{ get { return deck_Sorceries.Any(); } }
        public bool HasEnchantments{ get { return deck_Enchantments.Any(); } }
        public bool HasArtifacts{ get { return deck_Artifacts.Any(); } }
        public bool HasOther{ get { return deck_Other.Any(); } }

        public int count_All{ get { return deckManip.CountQuantity(Entries); } }
        public int count_Creature { get { return deckManip.CountQuantity(deck_Creatures); } }
        public int count_instant { get { return deckManip.CountQuantity(deck_Instants); } }
        public int count_Sorcery { get { return deckManip.CountQuantity(deck_Sorceries); } }
        public int count_Enchantment { get { return deckManip.CountQuantity(deck_Enchantments); } }
        public int count_Artifact { get { return deckManip.CountQuantity(deck_Artifacts); } }
        public int count_Land { get { return deckManip.CountQuantity(deck_Lands); } }
        public int count_BasicLand { get { return deckManip.CountQuantity(deck_BasicLands); } }
        public int count_specialLand { get { return deckManip.CountQuantity(deck_SpecialLands); } }
        public int count_Other { get { return deckManip.CountQuantity(deck_Other); } }

        public int DevotionB { get { return deckManip.CountDevotion(entries_Deck, 'B'); } }
        public int DevotionW { get { return deckManip.CountDevotion(entries_Deck, 'W'); } }
        public int DevotionU { get { return deckManip.CountDevotion(entries_Deck, 'U'); } }
        public int DevotionR { get { return deckManip.CountDevotion(entries_Deck, 'R'); } }
        public int DevotionG { get { return deckManip.CountDevotion(entries_Deck, 'G'); } }

        public string StandardOk { get { return deckManip.DeckValidity(Entries, "Standard").Result; } }
        public string CommanderOk { get { return deckManip.DeckValidity(Entries, "Commander").Result; } }
        public int OwnedRatio { get { return deckManip.GetRatio(Entries).Result; } }

        public PointCollection CurvePoints { get { return deckManip.GetManaCurve(this); } }

        #endregion

    }

}

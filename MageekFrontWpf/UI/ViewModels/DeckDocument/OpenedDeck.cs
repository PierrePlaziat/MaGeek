using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using System.Collections.Generic;
using MageekCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using MageekCore.Tools;
using MageekCore.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;

namespace MageekFrontWpf.UI.ViewModels
{

    public partial class OpenedDeckEntry : ObservableObject
    {
        [ObservableProperty] DeckCard line;
        [ObservableProperty] Cards card;
    }

    public partial class OpenedDeck : ObservableObject
    {

        #region Construct

        private MageekService mageek;

        [ObservableProperty] Deck header;
        [ObservableProperty] ObservableCollection<OpenedDeckEntry> entries;

        public OpenedDeck(MageekService mageek)
        {
            this.mageek = mageek;
        }

        public async Task Initialize(Deck deck)
        {
            Header = deck;
            Entries = new ObservableCollection<OpenedDeckEntry>(await EntriesFromCollec(deck.DeckId));
        }

        public async Task Initialize(Preco preco)
        {
            Header = await DeckFromPreco(preco);
            Entries = new ObservableCollection<OpenedDeckEntry>(await EntriesFromPreco(preco));

            //deck.DeckColors = ; //TODO 
            //deck.CardCount = ; //TODO 
        }

        private async Task<List<OpenedDeckEntry>> EntriesFromCollec(string deckId)
        {
            List<OpenedDeckEntry> newEntries = new();
            var content = await mageek.GetDeckContent(deckId);
            foreach (var line in content)
            {
                Cards card = await mageek.FindCard_Data(line.CardUuid);
                OpenedDeckEntry entry = new OpenedDeckEntry()
                {
                    Line = line,
                    Card = card,
                };
                newEntries.Add(entry);
            }
            return newEntries;
        }

        private async Task<Deck> DeckFromPreco(Preco preco)
        {
            Deck deck = new();
            deck.Title = preco.Title;
            deck.Description = string.Concat(preco.ReleaseDate," - ",preco.Kind);
            deck.DeckId = string.Concat("[",preco.Code,"] ",preco.Title);
            return deck;
        }

        private async Task<List<OpenedDeckEntry>> EntriesFromPreco(Preco preco) 
        {
            List<OpenedDeckEntry> list = new();
            foreach (Tuple<string, int> line in preco.CommanderCardUuids)
            {
                list.Add(new OpenedDeckEntry()
                {
                    Card = await mageek.FindCard_Data(line.Item1),
                    Line = new DeckCard()
                    {
                        CardUuid = line.Item1,
                        Quantity = line.Item2,
                        RelationType = 1,
                    }
                });
            }
            foreach (Tuple<string, int> line in preco.MainCardUuids)
            {
                list.Add(new OpenedDeckEntry()
                {
                    Card = await mageek.FindCard_Data(line.Item1),
                    Line = new DeckCard()
                    {
                        CardUuid = line.Item1,
                        Quantity = line.Item2,
                        RelationType = 0,
                    }
                });
            }
            foreach (Tuple<string, int> line in preco.SideCardUuids)
            {
                list.Add(new OpenedDeckEntry()
                {
                    Card = await mageek.FindCard_Data(line.Item1),
                    Line = new DeckCard()
                    {
                        CardUuid = line.Item1,
                        Quantity = line.Item2,
                        RelationType = 2,
                    }
                });
            }
            return list;
        }

        #endregion

        public IEnumerable<OpenedDeckEntry> entries_Deck { get { return Entries.Where(x=> x.Line.RelationType == 0); } }
        public IEnumerable<OpenedDeckEntry> entries_Commanders { get { return Entries.Where(x=> x.Line.RelationType == 1); } }
        public IEnumerable<OpenedDeckEntry> entries_Side { get { return Entries.Where(x=> x.Line.RelationType == 2); } }

        public IEnumerable<OpenedDeckEntry> deck_Creatures { get { return entries_Deck.Where(x => x.Card.Type.Contains("Creature")); } }
        public IEnumerable<OpenedDeckEntry> deck_Instants { get { return entries_Deck.Where(x => x.Card.Type.Contains("Instant")); } }
        public IEnumerable<OpenedDeckEntry> deck_Sorceries { get { return entries_Deck.Where(x => x.Card.Type.Contains("Sorcery")); } }
        public IEnumerable<OpenedDeckEntry> deck_Enchantments { get { return entries_Deck.Where(x => x.Card.Type.Contains("Enchantment")); } }
        public IEnumerable<OpenedDeckEntry> deck_Artifacts { get { return entries_Deck.Where(x => x.Card.Type.Contains("Artifact")); } }
        public IEnumerable<OpenedDeckEntry> deck_Lands { get { return entries_Deck.Where(x => x.Card.Type.Contains("Land")); } }
        public IEnumerable<OpenedDeckEntry> deck_BasicLands { get { return deck_Lands.Where(x => x.Card.Type.Contains("Basic")); } }
        public IEnumerable<OpenedDeckEntry> deck_SpecialLands { get { return deck_Lands.Where(x => ! x.Card.Type.Contains("Basic")); } }
        public IEnumerable<OpenedDeckEntry> deck_Other 
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

        public IEnumerable<OpenedDeckEntry> deck_Cmc0 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 0); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc1 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 1); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc2 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 2); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc3 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 3); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc4 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 4); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc5 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 5); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc6 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 6); } }
        public IEnumerable<OpenedDeckEntry> deck_Cmc7 { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost == 7); } }
        public IEnumerable<OpenedDeckEntry> deck_CmcBig { get { return entries_Deck.Where(x => x.Card.FaceConvertedManaCost >= 8); } }

        public bool HasCmc0 { get { return deck_Cmc0.Count() > 0; } }
        public bool HasCmc1 { get { return deck_Cmc1.Count() > 0; } }
        public bool HasCmc2 { get { return deck_Cmc2.Count() > 0; } }
        public bool HasCmc3 { get { return deck_Cmc3.Count() > 0; } }
        public bool HasCmc4 { get { return deck_Cmc4.Count() > 0; } }
        public bool HasCmc5 { get { return deck_Cmc5.Count() > 0; } }
        public bool HasCmc6 { get { return deck_Cmc6.Count() > 0; } }
        public bool HasCmc7 { get { return deck_Cmc7.Count() > 0; } }
        public bool HasCmcBig { get { return deck_CmcBig.Count() > 0; } }

        public int count_Creature { get { return CountCard(deck_Creatures); } }
        public int count_instant { get { return CountCard(deck_Instants); } }
        public int count_Sorcery { get { return CountCard(deck_Sorceries); } }
        public int count_Enchantment { get { return CountCard(deck_Enchantments); } }
        public int count_Artifact { get { return CountCard(deck_Artifacts); } }
        public int count_Land { get { return CountCard(deck_Lands); } }
        public int count_BasicLand { get { return CountCard(deck_BasicLands); } }
        public int count_specialLand { get { return CountCard(deck_SpecialLands); } }
        public int count_Other { get { return CountCard(deck_Other); } }
        private int CountCard(IEnumerable<OpenedDeckEntry> entries)
        {
            int count = 0;
            foreach (OpenedDeckEntry entry in entries)
            {
                count += entry.Line.Quantity;
            }
            return count;
        }

        public bool HasCommandant { get { return entries_Commanders.Count() > 0; } }
        public bool HasSide { get { return entries_Side.Count() > 0; } }
        public bool HasLands { get { return count_Land > 0; } }
        public bool HasCreatures{ get { return count_Creature > 0; } }
        public bool HasInstants{ get { return count_instant > 0; } }
        public bool HasSorceries{ get { return count_Sorcery > 0; } }
        public bool HasEnchantments{ get { return count_Enchantment > 0; } }
        public bool HasArtifacts{ get { return count_Artifact > 0; } }
        public bool HasOther{ get { return count_Other > 0; } }

        public int DevotionB 
        {
            get { 
                try
                {
                    return FindDevotion(entries_Deck, 'B');
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return 0;
                }
            }
        }
        public int DevotionW
        {
            get { 
                try
                {
                    return FindDevotion(entries_Deck, 'W');
    }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return 0;
                }
            }
        }
        public int DevotionU
        {
            get
            {
                try
                {
                    return FindDevotion(entries_Deck, 'U');
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return 0;
                }
            }
        }
        public int DevotionG
        {
            get
            {
                try
                {
                    return FindDevotion(entries_Deck, 'G');
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return 0;
                }
            }
        }
        public int DevotionR
        {
            get
            {
                try
                {
                    return FindDevotion(entries_Deck, 'R');
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    return 0;
                }
            }
        }
        private int FindDevotion(IEnumerable<OpenedDeckEntry> entries, char color)
        {
            int devotion = 0;
            foreach (var entry in entries)
            {
                devotion += FindDevotion(entry.Card.ManaCost, color) * entry.Line.Quantity;
            }
            return devotion;
        }
        private int FindDevotion(string manaCost, char color)
        {
            if (manaCost == null) return 0;
            return manaCost.Length - manaCost.Replace(color.ToString(), "").Length;
        }

        public string StandardOk { get { return DeckValidity(Entries, "Standard").Result; } }
        public string CommanderOk { get { return DeckValidity(Entries, "Commander").Result; } }
        public int OwnedRatio { get { return GetOwnedRatio(Entries).Result; } }

        #region Methods

        [ObservableProperty] Point curveStart;
        public PointCollection CurvePoints
        {
            get
            {
                var manaCurve = GetManaCurve(entries_Deck);
                CurveStart = new Point(0, 0);
                PointCollection Points = new();
                if (Header != null)
                {
                    int manaMax = manaCurve.ToList().Max();
                    float factor;
                    if (manaMax == 0) factor = 0;
                    else factor = 100 / manaMax;
                    CurveStart = new Point(0, 100 - factor * manaCurve[0]);
                    Points.Add(CurveStart);
                    for (int i = 1; i < manaCurve.Length; i++) Points.Add(new Point(50 * i, 100 - factor * manaCurve[i]));

                }
                return Points;
            }
        }
        public int[] GetManaCurve(IEnumerable<OpenedDeckEntry> entries)
        {
            var manaCurve = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                foreach (OpenedDeckEntry entry in entries)
                {
                    Cards card = entry.Card;
                    if (!card.Type.Contains("Land"))
                    {
                        int manacost = (int)card.ManaValue;
                        manaCurve[manacost <= 10 ? manacost : 10]++;
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
            return manaCurve;
        }

        public async Task<int> GetOwnedRatio(IEnumerable<OpenedDeckEntry> entries)
        {
            int total = 0;
            int miss = 0;
            foreach (var entry in entries)
            {
                Cards card = entry.Card;
                if (!card.Type.Contains("Basic Land"))
                {
                    total += entry.Line.Quantity;
                    int got = await mageek.Collected(entry.Line.CardUuid, false);
                    int need = entry.Line.Quantity;
                    int diff = need - got;
                    if (diff > 0) miss += diff;
                }
            }
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        /// <summary>
        /// Get txt list of missing cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>a text formated as : X cardName\n</returns>
        public async Task<string> ListMissingCards(IEnumerable<OpenedDeckEntry> entries)
        {
            string missList = "";
            foreach (var v in entries)
            {
                if (!v.Card.Type.Contains("Basic Land"))
                {
                    int got = await mageek.Collected(v.Line.CardUuid, false);
                    int need = v.Line.Quantity;
                    int diff = need - got;
                    if (diff > 0) missList += diff + " " + v.Card.Name + "\n";
                }
            }
            return missList;
        }

        /// <summary>
        /// Get deck validity in a given format
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="format"></param>
        /// <returns>"OK" or an error msg</returns>
        public async Task<string> DeckValidity(IEnumerable<OpenedDeckEntry> entries,string format)
        {
            int minCards = GetMinCardInFormat(format);
            int maxCards = GetMaxCardInFormat(format);
            int cnt = CountCard(entries);
            if (cnt > maxCards) return "Maximum " + maxCards + " cards.";
            if (cnt < minCards) return "Minimum " + maxCards + " cards.";
            int maxOccurence = GetMaxOccurenceInFormat(format);

            foreach (var v in entries)
            {
                Cards card = v.Card;
                if (!card.Type.Contains("Basic Land"))
                {
                    CardLegalities cardLegalities = await mageek.GetLegalities(card.Uuid);
                    string legal = "";
                    switch (format)
                    {
                        case "Alchemy": legal = cardLegalities.Alchemy; break;
                        case "Brawl": legal = cardLegalities.Brawl; break;
                        case "Commander": legal = cardLegalities.Commander; break;
                        case "Duel": legal = cardLegalities.Duel; break;
                        case "Explorer": legal = cardLegalities.Explorer; break;
                        case "Future": legal = cardLegalities.Future; break;
                        case "Gladiator": legal = cardLegalities.Gladiator; break;
                        case "Historic": legal = cardLegalities.Historic; break;
                        case "Legacy": legal = cardLegalities.Legacy; break;
                        case "Modern": legal = cardLegalities.Modern; break;
                        case "Oathbreaker": legal = cardLegalities.Oathbreaker; break;
                        case "Oldschool": legal = cardLegalities.Oldschool; break;
                        case "Pauper": legal = cardLegalities.Pauper; break;
                        case "Paupercommander": legal = cardLegalities.Paupercommander; break;
                        case "Penny": legal = cardLegalities.Penny; break;
                        case "Pioneer": legal = cardLegalities.Pioneer; break;
                        case "Predh": legal = cardLegalities.Predh; break;
                        case "Premodern": legal = cardLegalities.Premodern; break;
                        case "Standard": legal = cardLegalities.Standard; break;
                        case "StandardBrawl": legal = cardLegalities.Standardbrawl; break;
                        case "Timeless": legal = cardLegalities.Timeless; break;
                        case "Vintage": legal = cardLegalities.Vintage; break;
                    }
                    if (legal == null || legal == "Legal")
                    {
                        if (v.Line.Quantity > maxOccurence) return "Too many " + card.Name + ".";
                    }
                    if (legal == "Restricted")
                    {
                        if (v.Line.Quantity > 1) return card.Name + " restricted.";
                    }
                    if (legal == "Banned")
                    {
                        if (v.Line.Quantity > maxOccurence) return card.Name + " banned.";
                    }
                }
            }
            return "OK";
        }

        /// <summary>
        /// Get the minimum cards in a deck for a given format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The number</returns>
        private int GetMinCardInFormat(string format)
        {
            return format switch
            {
                "Alchemy" => 60,
                "Brawl" => 60,
                "Commander" => 100,
                "Duel" => 60,
                "Explorer" => 60,
                "Future" => 60,
                "Gladiator" => 60,
                "Historic" => 60,
                "Historicbrawl" => 60,
                "Legacy" => 60,
                "Modern" => 60,
                "Oathbreaker" => 60,
                "Oldschool" => 60,
                "Pauper" => 60,
                "Paupercommander" => 100,
                "Penny" => 60,
                "Pioneer" => 60,
                "Predh" => 60,
                "Premodern" => 60,
                "Standard" => 60,
                "Vintage" => 60,
                _ => 60,
            };
        }

        /// <summary>
        /// Get the maximum cards in a deck for a given format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The number</returns>
        private int GetMaxCardInFormat(string format)
        {
            return format switch
            {
                "Commander" => 100,
                "Paupercommander" => 100,
                "Explorer" => 250,
                _ => -1,
            };
        }

        /// <summary>
        /// Get the maximum occurences of a cards in a deck for a given format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The number</returns>
        private int GetMaxOccurenceInFormat(string format)
        {
            return format switch
            {
                "commander" => 1,
                _ => 4,
            };
        }

        #endregion

    }

}
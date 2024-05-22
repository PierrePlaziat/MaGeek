using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using PlaziatTools;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;
using MageekCore.Services;

namespace MageekFrontWpf.MageekTools.DeckTools
{

    public class DeckManipulator
    {

        private IMageekService mageek;

        public DeckManipulator(IMageekService mageek)
        {
            this.mageek = mageek;
        }

        public List<DeckCard> GetSavableCardList(IEnumerable<ManipulableDeckEntry> entries)
        {
            List<DeckCard> lines = new List<DeckCard>();
            foreach (var v in entries) lines.Add(v.Line);
            return lines;
        }

        public Deck GetHeaderFromPreco(Preco preco)
        {
            Deck deck = new();
            deck.Title = preco.Title;
            deck.Description = string.Concat(preco.ReleaseDate, " - ", preco.Kind);
            deck.DeckId = string.Concat("[", preco.Code, "] ", preco.Title);
            return deck;
        }

        public async Task<List<ManipulableDeckEntry>> GetEntriesFromDeck(string deckId)
        {
            List<ManipulableDeckEntry> newEntries = new();
            var content = await mageek.Decks_Content(deckId);
            foreach (var line in content)
            {
                Cards card = await mageek.Cards_GetData(line.CardUuid);
                ManipulableDeckEntry entry = new ManipulableDeckEntry()
                {
                    Line = line,
                    Card = card,
                };
                newEntries.Add(entry);
            }
            return newEntries;
        }

        public async Task<List<ManipulableDeckEntry>> GetEntriesFromPreco(Preco preco)
        {
            List<ManipulableDeckEntry> list = new();
            foreach (DeckCard line in preco.Cards)
            {
                list.Add(new ManipulableDeckEntry()
                {
                    Card = await mageek.Cards_GetData(line.CardUuid),
                    Line = new DeckCard()
                    {
                        CardUuid = line.CardUuid,
                        Quantity = line.Quantity,
                        RelationType = line.RelationType,
                    }
                });
            }
            return list;
        }

        public async Task<List<ManipulableDeckEntry>> GetEntriesFromImport(List<DeckCard> importLines)
        {
            List<ManipulableDeckEntry> list = new();
            foreach (DeckCard line in importLines)
            {
                list.Add(new ManipulableDeckEntry()
                {
                    Card = await mageek.Cards_GetData(line.CardUuid),
                    Line = new DeckCard()
                    {
                        CardUuid = line.CardUuid,
                        Quantity = line.Quantity,
                        RelationType = line.RelationType,
                    }
                });
            }
            return list;
        }

        public string GetColors(ObservableCollection<ManipulableDeckEntry> entries)
        {
            bool hasB = CountDevotion(entries, 'B') > 0;
            bool hasW = CountDevotion(entries, 'W') > 0;
            bool hasU = CountDevotion(entries, 'U') > 0;
            bool hasR = CountDevotion(entries, 'R') > 0;
            bool hasG = CountDevotion(entries, 'G') > 0;
            return string.Concat(
                hasB ? "B" : "",
                hasW ? "W" : "",
                hasU ? "U" : "",
                hasR ? "R" : "",
                hasG ? "G" : ""
            );
        }

        public int CountQuantity(IEnumerable<ManipulableDeckEntry> entries)
        {
            int count = 0;
            foreach (ManipulableDeckEntry entry in entries)
            {
                count += entry.Line.Quantity;
            }
            return count;
        }

        public int CountDevotion(IEnumerable<ManipulableDeckEntry> entries, char color)
        {
            int devotion = 0;
            foreach (var entry in entries)
            {
                devotion += CountDevotion(entry.Card.ManaCost, color) * entry.Line.Quantity;
            }
            return devotion;
        }
        private int CountDevotion(string manaCost, char color)
        {
            if (manaCost == null) return 0;
            return manaCost.Length - manaCost.Replace(color.ToString(), "").Length;
        }

        internal PointCollection GetManaCurve(ManipulableDeck manipulableDeck)
        {
            var manaCurve = GetManaCurve(manipulableDeck.entries_Deck);
            var CurveStart = new Point(0, 0);
            PointCollection Points = new();
            if (manipulableDeck.Header != null)
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
        private int[] GetManaCurve(IEnumerable<ManipulableDeckEntry> entries)
        {
            var manaCurve = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                foreach (ManipulableDeckEntry entry in entries)
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

        public async Task<string> GetMissing(IEnumerable<ManipulableDeckEntry> entries)
        {
            string missList = "";
            foreach (var v in entries)
            {
                if (!v.Card.Type.Contains("Basic Land"))
                {
                    int got = await mageek.Collec_OwnedCombined(
                        await mageek.Cards_NameForGivenCardUuid(v.Line.CardUuid)
                    );
                    int need = v.Line.Quantity;
                    int diff = need - got;
                    if (diff > 0) missList += diff + " " + v.Card.Name + "\n";
                }
            }
            return missList;
        }

        public async Task<int> GetRatio(IEnumerable<ManipulableDeckEntry> entries)
        {
            int total = 0;
            int miss = 0;
            foreach (var entry in entries)
            {
                Cards card = entry.Card;
                if (!card.Type.Contains("Basic Land"))
                {
                    total += entry.Line.Quantity;
                    int got = await mageek.Collec_OwnedCombined(await mageek.Cards_NameForGivenCardUuid(entry.Line.CardUuid));
                    int need = entry.Line.Quantity;
                    int diff = need - got;
                    if (diff > 0) miss += diff;
                }
            }
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        //////////////////////////////////////////////////

        public async Task<string> DeckValidity(IEnumerable<ManipulableDeckEntry> entries, string format)
        {
            int minCards = GetMinCardInFormat(format);
            int maxCards = GetMaxCardInFormat(format);
            int cnt = CountQuantity(entries);
            if (cnt > maxCards) return "Maximum " + maxCards + " cards.";
            if (cnt < minCards) return "Minimum " + maxCards + " cards.";
            int maxOccurence = GetMaxOccurenceInFormat(format);

            foreach (var v in entries)
            {
                Cards card = v.Card;
                if (!card.Type.Contains("Basic Land"))
                {
                    CardLegalities cardLegalities = await mageek.Cards_GetLegalities(card.Uuid);
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
        public int GetMinCardInFormat(string format)
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
        public int GetMaxCardInFormat(string format)
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
        public int GetMaxOccurenceInFormat(string format)
        {
            return format switch
            {
                "commander" => 1,
                _ => 4,
            };
        }

    }

}

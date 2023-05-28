using MaGeek.Entities;
using MaGeek.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    internal static class MageekStats
    {

        #region Tags

        public static async Task<List<CardTag>> GetTagsDistinct()
        {
            List<CardTag> tags = new();
            await Task.Run(() => {
                using var DB = App.DB.NewContext;
                tags.Add(null);
                tags.AddRange(
                        DB.CardTags.GroupBy(x => x.Tag).Select(x => x.First())
                );
            });
            return tags;
        }

        public static async Task<bool> DoesCardHasTag(string cardId, string tagFilterSelected)
        {
            return (await FindTagsForCard(cardId)).Where(x => x.Tag == tagFilterSelected).Any();
        }

        public static async Task TagCard(CardModel selectedCard, string text)
        {
            using var DB = App.DB.NewContext;
            DB.CardTags.Add(new CardTag(text, selectedCard));
            await DB.SaveChangesAsync();
        }

        public static async Task UnTagCard(CardTag cardTag)
        {
            using var DB = App.DB.NewContext;
            DB.CardTags.Remove(cardTag);
            await DB.SaveChangesAsync();
        }

        public static async Task<List<CardTag>> FindTagsForCard(string cardId)
        {
            List<CardTag> tags = new();
            await Task.Run(() => {
                using var DB = App.DB.NewContext;
                tags.AddRange(DB.CardTags.Where(x => x.CardId == cardId));
            });
            return tags;
        }

        #endregion

        #region Statistics

        // TODO : Too Slow
        public static async Task<string> DeckColors(Deck deck)
        {
            string retour = "";
            await Task.Run(() => {
                if (deck.DeckCards.Where(x => x.Card.Card.ManaCost.Contains('B')).Any()) retour += "B";
                if (deck.DeckCards.Where(x => x.Card.Card.ManaCost.Contains('W')).Any()) retour += "W";
                if (deck.DeckCards.Where(x => x.Card.Card.ManaCost.Contains('U')).Any()) retour += "U";
                if (deck.DeckCards.Where(x => x.Card.Card.ManaCost.Contains('G')).Any()) retour += "G";
                if (deck.DeckCards.Where(x => x.Card.Card.ManaCost.Contains('R')).Any()) retour += "R";
            });
            return retour;
        }

        public static async Task<float> EstimateDeckPrice(Deck selectedDeck)
        {
            float total = 0;
            await Task.Run(() => {
                foreach (var v in selectedDeck.DeckCards)
                {
                    float price = float.Parse(v.Card.ValueEur);
                    total += v.Quantity * price;
                }
            });
            return total;
        }

        #region Devotions

        public static async Task<int> DevotionB(Deck deck)
        {
            if (deck == null) return 0;
            if (deck.DeckCards == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.DeckCards) devotion += c.Card.Card.DevotionB;
            });
            return devotion;
        }
        public static async Task<int> DevotionW(Deck deck)
        {
            if (deck == null) return 0;
            if (deck.DeckCards == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.DeckCards) devotion += c.Card.Card.DevotionW;
            });
            return devotion;
        }
        public static async Task<int> DevotionU(Deck deck)
        {
            if (deck == null) return 0;
            if (deck.DeckCards == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.DeckCards) devotion += c.Card.Card.DevotionU;
            });
            return devotion;
        }
        public static async Task<int> DevotionG(Deck deck)
        {
            if (deck == null) return 0;
            if (deck.DeckCards == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.DeckCards) devotion += c.Card.Card.DevotionG;
            });
            return devotion;
        }
        public static async Task<int> DevotionR(Deck deck)
        {
            if (deck == null) return 0;
            if (deck.DeckCards == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.DeckCards) devotion += c.Card.Card.DevotionR;
            });
            return devotion;
        }

        #endregion

        #region counts

        public static async Task<int> Count_Total(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var card in deck.DeckCards.Where(x => x.RelationType < 2))
                    {
                        count += card.Quantity;
                    }
                }
            });
            return count;
        }

        public static async Task<int> Count_Creature(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card != null && x.Card.Card.Type.ToLower().Contains("creature")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Instant(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("instant")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Sorcery(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("sorcery")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Enchantment(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("enchantment")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Artifact(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("artifact")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_BasicLand(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("basic land")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_SpecialLand(Deck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x => x.Card.Card.Type.ToLower().Contains("land") && !x.Card.Card.Type.ToLower().Contains("basic")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_other(Deck deck)
        {

            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.DeckCards != null)
                {
                    foreach (var v in deck.DeckCards.Where(x =>
                        !x.Card.Card.Type.ToLower().Contains("creature") &&
                        !x.Card.Card.Type.ToLower().Contains("instant") &&
                        !x.Card.Card.Type.ToLower().Contains("sorcery") &&
                        !x.Card.Card.Type.ToLower().Contains("enchantment") &&
                        !x.Card.Card.Type.ToLower().Contains("artifact") &&
                        !x.Card.Card.Type.ToLower().Contains("land")
                    ))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        #endregion

        #region Types

        public static async Task<IEnumerable<DeckCard>> GetCommanders(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(x => x.RelationType == 1);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCreatures(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("creature"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetInstants(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("instant"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetSorceries(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("sorcery"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetEnchantments(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("enchantment"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCurrentArtifacts(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("artifact"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCurrentNonBasicLands(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && !x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCurrentBasicLands(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCurrentOthers(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 0
                && x.Card != null
                && !x.Card.Card.Type.ToLower().Contains("artifact")
                && !x.Card.Card.Type.ToLower().Contains("creature")
                && !x.Card.Card.Type.ToLower().Contains("instant")
                && !x.Card.Card.Type.ToLower().Contains("sorcery")
                && !x.Card.Card.Type.ToLower().Contains("enchantment")
                && !x.Card.Card.Type.ToLower().Contains("land"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<DeckCard>> GetCurrentSide(Deck deck)
        {
            IEnumerable<DeckCard> rels = new List<DeckCard>();
            if (deck == null || deck.DeckCards == null) return rels;
            await Task.Run(() => {
                rels = deck.DeckCards.Where(
                x => x.RelationType == 2
                && x.Card != null)
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }

        #endregion

        #region Indicators

        public static async Task<int[]> GetManaCurve(Deck deck)
        {
            var manaCurve = new int[11];
            await Task.Run(() => {
                manaCurve[0] = deck.DeckCards.Where(x => !x.Card.Card.Type.ToLower().Contains("land") && x.Card.Card.Cmc == 0).Count();
                manaCurve[1] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 1).Count();
                manaCurve[2] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 2).Count();
                manaCurve[3] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 3).Count();
                manaCurve[4] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 4).Count();
                manaCurve[5] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 5).Count();
                manaCurve[6] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 6).Count();
                manaCurve[7] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 7).Count();
                manaCurve[8] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 8).Count();
                manaCurve[9] = deck.DeckCards.Where(x => x.Card.Card.Cmc == 9).Count();
                manaCurve[10] = deck.DeckCards.Where(x => x.Card.Card.Cmc >= 10).Count();
            });
            return manaCurve;
        }

        public static async Task<int> OwnedRatio(Deck currentDeck)
        {
            if (currentDeck == null) return 0;
            if (currentDeck.DeckCards == null) return 0;
            int total = 0;
            int miss = 0;
            await Task.Run(() => {
                foreach (var v in currentDeck.DeckCards)
                {
                    total += v.Quantity;

                    int got = v.Card.Got;
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) miss += diff;
                }
            });
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        public static async Task<string> ListMissingCards(Deck currentDeck)
        {
            if (currentDeck == null) return null;
            if (currentDeck.DeckCards == null) return null;
            string missList = "";
            await Task.Run(() => {
                foreach (var v in currentDeck.DeckCards)
                {
                    int got = v.Card.Got;
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) missList += diff + " " + v.Card.Card.CardId + "\n";
                }
            });
            return missList;
        }

        #endregion

        #region Validities

        public static async Task<string> Validity_Standard(Deck deck)
        {
            if (deck == null) return "";
            if (deck.CardCount < 60) return "Min 60 cards needed";
            if (!await RespectsMaxCardOccurence(deck, 4)) return "No more than 4 times the same card needed";
            using var DB = App.DB.NewContext;
            foreach (var v in deck.DeckCards)
            {
                if (!v.Card.Card.Type.Contains("Basic Land"))
                    if (DB.CardLegalities.Where(x => x.CardId == v.Card.Id && x.Format == "legacy" && x.IsLegal == "legal").Any())
                    {
                        return v.Card.Card.CardId + " is not legal";
                    }
            }
            return "OK";
        }

        public static async Task<string> Validity_Commander(Deck deck)
        {
            if (deck == null) return "";
            if (deck.CardCount != 100) return "Exctly 100 cards needed";
            if (!await RespectsMaxCardOccurence(deck, 1)) return "No more than 1 times the same card needed.";
            using var DB = App.DB.NewContext;
            foreach (var v in deck.DeckCards)
            {
                if (!v.Card.Card.Type.Contains("Basic Land"))
                    if (DB.CardLegalities.Where(x => x.CardId == v.Card.Id && x.Format == "commander" && x.IsLegal == "legal").Any())
                    {
                        return v.Card.Card.CardId + " is not legal";
                    }
            }
            return "OK";
        }

        private static async Task<bool> RespectsMaxCardOccurence(Deck deck, int limit)
        {
            if (deck == null) return false;
            bool ok = true;
            await Task.Run(() => {
                foreach (var v in deck.DeckCards.Where(x => !x.Card.Card.Type.ToString().ToLower().Contains("land")))
                {
                    if (v.Quantity > limit) ok = false;
                }
            });
            return ok;
        }

        internal static async Task<int> GetTotalOwned()
        {
            int total = 0;
            try
            {
                using var DB = App.DB.NewContext;
                await Task.Run(() =>
                {
                    total = DB.CardVariants.Sum(x => x.Got);
                });
            }
            catch (Exception e) { Log.Write(e, "GetTotalOwned"); }
            return total;
        }
        
        internal static async  Task<int> GetTotalDiffGot()
        {
            int total = 0;
            try
            {
                using var DB = App.DB.NewContext;
                await Task.Run(() =>
                {
                    total = DB.CardVariants.Where(x=>x.Got>0)
                                            .GroupBy(x=>x.Card)
                                            .Select(g => g.First().Card)
                                            .Count();
                    
                });
            }
            catch (Exception e) { Log.Write(e, "GetTotalDiffGot"); }
            return total;
        }
        
        internal static async  Task<int> GetTotalDiffExist()
        {
            int total = 0;
            try
            {
                using var DB = App.DB.NewContext;
                await Task.Run(() =>
                {
                    total = DB.CardModels.Count();
                });
            }
            catch (Exception e) { Log.Write(e, "GetTotalDiffExist"); }
            return total;
        }


        internal static async Task<float> AutoEstimatePrices()
        {
            float total = 0;
            //try
            //{
            //    await Task.Run(() => {
            //        using var DB = App.DB.GetNewContext();
            //        var miss = DB.CardVariants.Where(x => x.Got > 0).Include(x=>x.Card);
            //        foreach (CardVariant card in miss)
            //        {
            //            if (card.ValueEur != null)
            //            {
            //                total += card.Got * float.Parse(card.ValueEur);
            //            }
            //            else
            //            {
            //                var cardModel = DB.CardModels.Where(x => x.CardId == card.Id).FirstOrDefault();
            //                if (!string.IsNullOrEmpty(cardModel.MeanPrice))
            //                {
            //                    total += card.Got * float.Parse(card.ValueEur);
            //                }
            //                else
            //                {
            //                    missingList.Add(card);
            //                    MissingCount++;
            //                }
            //            }
            //        }
            //    });
            //}
            //catch (Exception e) { Log.Write(e, "AutoEstimatePrices"); }
            return total;
        }

        #endregion

        #endregion

    }
}

#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.
#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
#pragma warning disable CS8625 // Impossible de convertir un littéral ayant une valeur null en type référence non-nullable.
#pragma warning disable CS8603 // Existence possible d'un retour de référence null.

using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using MageekCore.Data;
using MageekCore.Data.Mtg;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Data.Collection;
using MageekCore.Data.Collection.Entities;
using PlaziatTools;

namespace MageekCore.Service
{

    public class MageekService : IMageekService
    {

        #region Construction

        private readonly MtgDbManager mtg;
        private readonly CollectionDbManager collec;

        private readonly MtgJsonManager mtgjson;
        private readonly ScryManager scryfall;

        public MageekService()
        {
            mtg = new MtgDbManager();
            collec = new CollectionDbManager(mtg);
            scryfall = new ScryManager(mtg);
            mtgjson = new MtgJsonManager(mtg, collec);
        }

        #endregion

        #region Implementation

        public async Task<MageekConnectReturn> Connect(string address)
        {
            return MageekConnectReturn.NotImplementedForServer;
        }

        public async Task<MageekInitReturn> Initialize()
        {
            try
            {
                Folders.InitServerFolders();
                if (!File.Exists(Folders.File_CollectionDB)) collec.CreateDb();
                bool needsUpdate = await mtgjson.CheckUpdate();
                return needsUpdate ? MageekInitReturn.MtgOutdated : MageekInitReturn.MtgUpToDate;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return MageekInitReturn.Error;
            }
        }

        public async Task<MageekUpdateReturn> Update()
        {
            Logger.Log("...Error");
            try
            {
                await mtgjson.DownloadData();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return MageekUpdateReturn.ErrorDownloading;
            }
            try
            {
                List<Task> tasks = new()
                {
                    mtgjson.FetchData(),
                    scryfall.FetchSets()
                };
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return MageekUpdateReturn.ErrorFetching;
            }
            mtgjson.HashSave();
            return MageekUpdateReturn.Success;
        }

        #region Cards

        // Search

        /// <summary>
        /// Search cards by name
        /// </summary>
        /// <param name="lang">search in translations</param>
        /// <param name="filterName">string to find in card name, original or selected lang</param>
        /// <param name="page">pagination data</param>
        /// <param name="pageSize">pagination data</param>
        /// <returns></returns>
        public async Task<List<SearchedCards>> NormalSearch(string filterName, string lang, int page, int pageSize)
        {
            Logger.Log("searching...");
            List<SearchedCards> retour = new();
            List<Cards> found = new();
            string lowerFilterName = filterName.ToLower();
            string normalizedFilterName = filterName.RemoveDiacritics().Replace('-', ' ').ToLower();

            using (CollectionDbContext DB = await collec.GetContext())
            using (MtgDbContext DB2 = await mtg.GetContext())
            {
                if (!string.IsNullOrEmpty(filterName))
                {
                    // Search in VO
                    var voResults = await DB.CardArchetypes.Where(x => x.ArchetypeId.ToLower().Contains(lowerFilterName)).ToListAsync();
                    foreach (var vo in voResults) found.AddRange(DB2.cards.Where(x => x.Uuid == vo.CardUuid));
                    // Search in foreign
                    var tradResults = await DB.CardTraductions.Where(x => x.Language == lang && x.NormalizedTraduction.Contains(normalizedFilterName)).ToListAsync();
                    foreach (var trad in tradResults) found.AddRange(DB2.cards.Where(x => x.Uuid == trad.CardUuid));
                }
                else found.AddRange(await DB2.cards.ToArrayAsync());
                // Remove duplicata
                found = found.GroupBy(x => x.Name).Select(g => g.First()).ToList();
                // Add infos
                Logger.Log("find collec data...");
                foreach (var card in found.Skip(page * pageSize).Take(pageSize))
                {
                    retour.Add(new SearchedCards(
                        card,
                        await Collected_AllVariants(card.Uuid),
                        await GetTranslatedData(card.Uuid, lang)
                    ));
                }
            }
            Logger.Log("Done - " + retour.Count + " results on " + found.Count);
            return retour;
        }

        /// <summary>
        /// Search cards by various parameters
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="filterName"></param>
        /// <returns>List of cards</returns>
        public async Task<List<SearchedCards>> AdvancedSearch(string filterName, string lang, string filterType, string filterKeyword, string filterText, string filterColor, string filterTag, bool onlyGot, bool colorisOr, int page, int pageSize)
        {
            Logger.Log("");
            List<SearchedCards> retour = await NormalSearch(lang, filterName, page, pageSize);

            try
            {

                if (!string.IsNullOrEmpty(filterType))
                {
                    string type = filterType.ToLower();
                    retour = retour.Where(x => x.Card.Type.ToLower().Contains(type)).ToList();
                }

                if (!string.IsNullOrEmpty(filterKeyword))
                {
                    string[] keywords = filterKeyword.Split(' ');
                    foreach (string keyword in keywords)
                    {
                        retour = retour.Where(x => x.Card.Keywords != null && x.Card.Keywords.ToLower().Contains(keyword)).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(filterText))
                {
                    string text = filterText.ToLower();
                    retour = retour.Where(x => x.Card.Text != null && x.Card.Text.ToLower().Contains(text)).ToList();
                }

                if (filterColor != "_")
                {
                    //if(colorisOr)
                    //{
                    //    switch (filterColor)
                    //    {
                    //        case "X": retour = retour.Where(x => string.IsNullOrEmpty(x.ColorIdentity)).ToList(); break;
                    //        case "W": retour = retour.Where(x => x.ColorIdentity.Contains('W')).ToList(); break;
                    //        case "B": retour = retour.Where(x => x.ColorIdentity.Contains('B')).ToList(); break;
                    //        case "U": retour = retour.Where(x => x.ColorIdentity.Contains('U')).ToList(); break;
                    //        case "G": retour = retour.Where(x => x.ColorIdentity.Contains('G')).ToList(); break;
                    //        case "R": retour = retour.Where(x => x.ColorIdentity.Contains('R')).ToList(); break;
                    //        case "GW": retour = retour.Where(x => x.ColorIdentity.Contains('G, W')).ToList(); break;
                    //        case "WU": retour = retour.Where(x => x.ColorIdentity.Contains('U, W')).ToList(); break;
                    //        case "BU": retour = retour.Where(x => x.ColorIdentity.Contains('B, U')).ToList(); break;
                    //        case "RB": retour = retour.Where(x => x.ColorIdentity.Contains('B, R')).ToList(); break;
                    //        case "GR": retour = retour.Where(x => x.ColorIdentity.Contains('G, R')).ToList(); break;
                    //        case "GU": retour = retour.Where(x => x.ColorIdentity.Contains('G, U')).ToList(); break;
                    //        case "WB": retour = retour.Where(x => x.ColorIdentity.Contains('B, W')).ToList(); break;
                    //        case "RU": retour = retour.Where(x => x.ColorIdentity.Contains('R, U')).ToList(); break;
                    //        case "GB": retour = retour.Where(x => x.ColorIdentity.Contains('B, G')).ToList(); break;
                    //        case "RW": retour = retour.Where(x => x.ColorIdentity.Contains('R, W')).ToList(); break;
                    //        case "GBW": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, W')).ToList(); break;
                    //        case "GWU": retour = retour.Where(x => x.ColorIdentity.Contains('G, U, W')).ToList(); break;
                    //        case "WRU": retour = retour.Where(x => x.ColorIdentity.Contains('R, U, W')).ToList(); break;
                    //        case "GRW": retour = retour.Where(x => x.ColorIdentity.Contains('G, R, W')).ToList(); break;
                    //        case "WUB": retour = retour.Where(x => x.ColorIdentity.Contains('B, U, W')).ToList(); break;
                    //        case "GUR": retour = retour.Where(x => x.ColorIdentity.Contains('G, R, U')).ToList(); break;
                    //        case "GRB": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, R')).ToList(); break;
                    //        case "RUB": retour = retour.Where(x => x.ColorIdentity.Contains('B, R, U')).ToList(); break;
                    //        case "BGU": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, U')).ToList(); break;
                    //        case "RWB": retour = retour.Where(x => x.ColorIdentity.Contains('B, R, W')).ToList(); break;
                    //        case "BGUR": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, R, U')).ToList(); break;
                    //        case "GURW": retour = retour.Where(x => x.ColorIdentity.Contains('G, R, U, W')).ToList(); break;
                    //        case "URWB": retour = retour.Where(x => x.ColorIdentity.Contains('B, R, U, W')).ToList(); break;
                    //        case "RWBG": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, R, W')).ToList(); break;
                    //        case "WBGU": retour = retour.Where(x => x.ColorIdentity.Contains('B, G, U, W')).ToList(); break;
                    //    }
                    //}
                    //else
                    {
                        switch (filterColor)
                        {
                            case "X": retour = retour.Where(x => string.IsNullOrEmpty(x.Card.ColorIdentity)).ToList(); break;
                            case "W": retour = retour.Where(x => x.Card.ColorIdentity == "W").ToList(); break;
                            case "B": retour = retour.Where(x => x.Card.ColorIdentity == "B").ToList(); break;
                            case "U": retour = retour.Where(x => x.Card.ColorIdentity == "U").ToList(); break;
                            case "G": retour = retour.Where(x => x.Card.ColorIdentity == "G").ToList(); break;
                            case "R": retour = retour.Where(x => x.Card.ColorIdentity == "R").ToList(); break;
                            case "GW": retour = retour.Where(x => x.Card.ColorIdentity == "G, W").ToList(); break;
                            case "WU": retour = retour.Where(x => x.Card.ColorIdentity == "U, W").ToList(); break;
                            case "BU": retour = retour.Where(x => x.Card.ColorIdentity == "B, U").ToList(); break;
                            case "RB": retour = retour.Where(x => x.Card.ColorIdentity == "B, R").ToList(); break;
                            case "GR": retour = retour.Where(x => x.Card.ColorIdentity == "G, R").ToList(); break;
                            case "GU": retour = retour.Where(x => x.Card.ColorIdentity == "G, U").ToList(); break;
                            case "WB": retour = retour.Where(x => x.Card.ColorIdentity == "B, W").ToList(); break;
                            case "RU": retour = retour.Where(x => x.Card.ColorIdentity == "R, U").ToList(); break;
                            case "GB": retour = retour.Where(x => x.Card.ColorIdentity == "B, G").ToList(); break;
                            case "RW": retour = retour.Where(x => x.Card.ColorIdentity == "R, W").ToList(); break;
                            case "GBW": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, W").ToList(); break;
                            case "GWU": retour = retour.Where(x => x.Card.ColorIdentity == "G, U, W").ToList(); break;
                            case "WRU": retour = retour.Where(x => x.Card.ColorIdentity == "R, U, W").ToList(); break;
                            case "GRW": retour = retour.Where(x => x.Card.ColorIdentity == "G, R, W").ToList(); break;
                            case "WUB": retour = retour.Where(x => x.Card.ColorIdentity == "B, U, W").ToList(); break;
                            case "GUR": retour = retour.Where(x => x.Card.ColorIdentity == "G, R, U").ToList(); break;
                            case "GRB": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, R").ToList(); break;
                            case "RUB": retour = retour.Where(x => x.Card.ColorIdentity == "B, R, U").ToList(); break;
                            case "BGU": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, U").ToList(); break;
                            case "RWB": retour = retour.Where(x => x.Card.ColorIdentity == "B, R, W").ToList(); break;
                            case "BGUR": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, R, U").ToList(); break;
                            case "GURW": retour = retour.Where(x => x.Card.ColorIdentity == "G, R, U, W").ToList(); break;
                            case "URWB": retour = retour.Where(x => x.Card.ColorIdentity == "B, R, U, W").ToList(); break;
                            case "RWBG": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, R, W").ToList(); break;
                            case "WBGU": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, U, W").ToList(); break;
                            case "WBGUR": retour = retour.Where(x => x.Card.ColorIdentity == "B, G, R, U, W").ToList(); break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(filterTag))
                {
                    var tagged = new List<SearchedCards>();
                    foreach (var card in retour)
                    {
                        if (await HasTag(card.Card.Name, filterTag))
                        {
                            tagged.Add(card);
                        }
                    }
                    retour = new List<SearchedCards>(tagged);
                }

                //if (onlyGot)
                //{
                //    using CollectionDbContext DB = await collection.GetContext();
                //    retour = retour.Where(x => x.Collected > 0).ToList();
                //}
            }
            catch (Exception e) { Logger.Log(e); }
            return retour;
        }

        // Uuid/Name/Uuids convertions

        /// <summary>
        /// Get all card variant ids from an archetypal card name
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>a list of uuid</returns>
        public async Task<List<string>> GetCardUuidsForGivenCardName(string archetypeId)
        {
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.ArchetypeId == archetypeId)
                .Select(p => p.CardUuid)
                .ToListAsync();
        }

        /// <summary>
        /// Get an archetypal card id from an card variant uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>a single archetype id</returns>
        public async Task<string> GetCardNameForGivenCardUuid(string cardUuid)
        {
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.CardUuid == cardUuid)
                .Select(p => p.ArchetypeId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetCardUuidsForGivenCardUuid(string uuid)
        {
            return await GetCardUuidsForGivenCardName(await GetCardNameForGivenCardUuid(uuid));
        }

        // Complete Data

        /// <summary>
        /// get the gameplay data of the card
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>Archetype</returns>
        public async Task<Cards> FindCard_Data(string cardUuid)
        {
            Logger.Log(cardUuid);
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.cards
                .Where(x => x.Uuid == cardUuid)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get all traducted infos of the card
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="lang"></param>
        /// <returns>The data if any</returns>
        public async Task<CardForeignData> GetTranslatedData(string cardUuid, string lang)
        {
            Logger.Log("");
            try
            {
                using MtgDbContext DB = await mtg.GetContext();
                {
                    CardForeignData? cardForeignData = await DB.cardForeignData.Where(x => x.Uuid == cardUuid && x.Language == lang).FirstOrDefaultAsync();
                    return cardForeignData;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// <summary>
        /// For double sided cards, get back uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>The card uuid of the back</returns>
        public async Task<string?> GetCardBack(string cardUuid)
        {
            Logger.Log("");
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.cards.Where(x => x.Uuid == cardUuid).Select(x => x.OtherFaceIds).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get card legalities
        /// </summary>
        /// <param name="selectedCard"></param>
        /// <returns>List of legalities</returns>
        public async Task<CardLegalities> GetLegalities(string CardUuid)
        {
            Logger.Log("");
            CardLegalities leg = null;
            try
            {
                using MtgDbContext DB = await mtg.GetContext();
                leg = await DB.cardLegalities.Where(x => x.Uuid == CardUuid).FirstOrDefaultAsync();

            }
            catch (Exception e) { Logger.Log(e, true); }
            return leg;
        }

        /// <summary>
        /// get card rulings
        /// </summary>
        /// <param name="selectedCard"></param>
        /// <returns>List of rulings</returns>
        public async Task<List<CardRulings>> GetRulings(string CardUuid)
        {
            Logger.Log("");
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.cardRulings.Where(x => x.Uuid == CardUuid).ToListAsync();
        }

        public async Task<List<CardCardRelation>> FindRelated(string uuid) // TODO cache data at db mtg import
        {
            Logger.Log("");
            List<CardCardRelation> outputCards = new();
            try
            {
                if (string.IsNullOrEmpty(uuid)) return outputCards;
                var scryCard = await scryfall.GetScryfallCard(uuid);
                string originalarchetype = scryCard.Name;
                if (scryCard == null) return outputCards;
                if (scryCard.AllParts == null) return outputCards;
                foreach (var part in scryCard.AllParts)
                {
                    part.TryGetValue("component", out string component);
                    part.TryGetValue("name", out string archetype);
                    if (!string.IsNullOrEmpty(component) && !string.IsNullOrEmpty(archetype))
                    {
                        if (archetype != originalarchetype)
                        {
                            CardCardRelationRole? role = null;
                            switch (component)
                            {
                                case "token": role = CardCardRelationRole.token; break;
                                case "meld_part": role = CardCardRelationRole.meld_part; break;
                                case "meld_result": role = CardCardRelationRole.meld_result; break;
                                case "combo_piece": role = CardCardRelationRole.combo_piece; break;
                            }
                            if (role.HasValue)
                            {
                                Cards cRes = null;
                                Tokens tRes = null;
                                if (role.Value == CardCardRelationRole.token)
                                {
                                    tRes = await FindToken(archetype);
                                }
                                else
                                {
                                    using MtgDbContext DB = await mtg.GetContext();
                                    cRes = await DB.cards
                                        .Where(x => x.Name == archetype)
                                        .FirstOrDefaultAsync();
                                }
                                outputCards.Add(new CardCardRelation()
                                {
                                    Role = role.Value,
                                    Card = cRes,
                                    Token = tRes
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
            return outputCards;
        }

        public async Task<Tokens> FindToken(string name)
        {
            Logger.Log("");
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.tokens
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get the illustration of a card, save it locally if not already done
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>a local url to a jpg</returns>
        public async Task<Uri> RetrieveImage(string cardUuid, CardImageFormat type, bool back = false)
        {
            try
            {
                string localFileName = Path.Combine(
                    Folders.Illustrations,
                    string.Concat(cardUuid, "_", type.ToString())
                );
                if (!File.Exists(localFileName))
                {
                    string name = await GetCardNameForGivenCardUuid(cardUuid);
                    await scryfall.DownloadImage(cardUuid, type, localFileName,back);
                }
                return new("file://" + Path.GetFullPath(localFileName), UriKind.Absolute);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// <summary>
        /// Estimate the price of a card
        /// </summary>
        /// <param name="v"></param>
        /// <param name="currency"></param>
        /// <returns>The estimation</returns>
        public async Task<PriceLine> EstimateCardPrice(string cardUuid)
        {
            using CollectionDbContext collecContext = await collec.GetContext();
            return await collecContext.PriceLine.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
        }

        #endregion

        #region Sets

        /// <summary>
        /// Get all sets
        /// </summary>
        /// <returns>List of sets</returns>
        public async Task<List<Sets>> LoadSets()
        {
            using MtgDbContext DB = await mtg.GetContext();
            return DB.sets.OrderByDescending(x => x.ReleaseDate).ToList();
        }

        public async Task<Sets> GetSet(string setCode)
        {
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.sets.Where(x => x.Code == setCode).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get cards in a set
        /// </summary>
        /// <param name="setCode"></param>
        /// <returns>Uuid list of cards in set</returns>
        public async Task<List<Cards>> GetCardsFromSet(string setCode)
        {
            List<Cards> cards = new();
            if (!string.IsNullOrEmpty(setCode))
            {
                try
                {
                    using MtgDbContext DB = await mtg.GetContext();
                    {
                        cards = await DB.cards.Where(x => x.SetCode == setCode)
                            .ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }
            return cards;
        }

        /// <summary>
        /// How many cards collected in this set
        /// </summary>
        /// <param name="setCode"></param>
        /// <param name="strict">if set to false, the archetype from any set counts</param>
        /// <returns>the distinct count</returns>
        public async Task<int> GetMtgSetCompletion(string setCode, bool strict)
        {
            int nb = 0;
            try
            {
                var cardUuids = await GetCardsFromSet(setCode);
                using CollectionDbContext DB = await collec.GetContext();
                foreach (var card in cardUuids)
                {
                    if (strict) nb += await Collected_SingleVariant(card.Uuid);
                    else nb += await Collected_AllVariants(card.Name);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            return nb;
        }

        #endregion

        #region Collection

        /// <summary>
        /// Determine favorite card variant for a card archetype
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="cardUuid"></param>
        public async Task SetFav(string archetypeId, string cardUuid)
        {
            Logger.Log("");
            try
            {
                if (string.IsNullOrEmpty(archetypeId)) return;
                using CollectionDbContext DB = await collec.GetContext();
                FavVariant? favCard = await DB.FavVariant.Where(x => x.ArchetypeId == archetypeId).FirstOrDefaultAsync();
                if (favCard == null)
                {
                    // Create
                    DB.FavVariant.Add(new FavVariant()
                    {
                        ArchetypeId = archetypeId,
                        FavUuid = cardUuid
                    });
                }
                else
                {
                    // Update
                    favCard.FavUuid = cardUuid;
                    DB.Entry(favCard).State = EntityState.Modified;
                }
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { Logger.Log(e); }
        }

        /// <summary>
        /// Add or remove card in the collection
        /// </summary>
        /// <param name="cardUuid">from mtgjson</param>
        /// <param name="quantityModification">how many</param>
        /// <returns>Quantity in collec before and after the move</returns>
        public async Task CollecMove(string cardUuid, int quantityModification)
        {
            Logger.Log("");
            // Guard
            if (string.IsNullOrEmpty(cardUuid)) return;// new Tuple<int, int>(0, 0);
            // Get or create collected card
            using CollectionDbContext DB = await collec.GetContext();
            CollectedCard collectedCard = await DB.CollectedCard.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
            if (collectedCard == null)
            {
                collectedCard = new CollectedCard() { CardUuid = cardUuid, Collected = 0 };
                DB.CollectedCard.Add(collectedCard);
                DB.Entry(collectedCard).State = EntityState.Added;
            }
            else
            {
                DB.Entry(collectedCard).State = EntityState.Modified;
            }
            // Make the move
            int quantityBeforeMove = collectedCard.Collected;
            collectedCard.Collected += quantityModification;
            if (collectedCard.Collected < 0) collectedCard.Collected = 0;
            int quantityAfterMove = collectedCard.Collected;
            await DB.SaveChangesAsync();
            //return new Tuple<int, int>(quantityBeforeMove, quantityAfterMove);
        }

        /// <summary>
        /// Counts how many cards collected variably
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="onlyThisVariant">set to false if you want to perform archetypal search from this card variant</param>
        /// <returns>The count</returns>
        public async Task<int> Collected_SingleVariant(string cardUuid)
        {
            try
            {
                if (string.IsNullOrEmpty(cardUuid)) return 0;
                using CollectionDbContext DB = await collec.GetContext();
                CollectedCard? collectedCard = await DB.CollectedCard.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
                return collectedCard != null ? collectedCard.Collected : 0;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return -1;
            }
        }

        public async Task<int> Collected_AllVariants(string archetypeId)
        {
            if (string.IsNullOrEmpty(archetypeId)) return 0;
            int count = 0;
            using CollectionDbContext DB = await collec.GetContext();
            List<string> uuids = await DB.CardArchetypes.Where(x => x.ArchetypeId == archetypeId).Select(p => p.CardUuid).ToListAsync();
            foreach (string uuid in uuids) count += await Collected_SingleVariant(uuid);
            return count;
        }

        /// <summary>
        /// Totality of cards including their quantity
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotal_Collected()
        {
            int total = 0;
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                total = DB.CollectedCard.Sum(x => x.Collected);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            return total;
        }

        /// <summary>
        /// Totality of cards variants but doesnt sur their quantity
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotal_CollectedDiff()
        {
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                return DB.CollectedCard.Count();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return 0;
            }
        }

        /// <summary>
        /// Totality of different archetypes
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotal_CollectedArchetype()
        {
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                return DB.CollectedCard.Count(); //TODO this is wrong
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return 0;
            }
        }

        /// <summary>
        /// Totality of different existing card archetypes
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotal_ExistingArchetypes()
        {
            int total = 0;
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                total = DB.CardArchetypes
                    .GroupBy(x => x.ArchetypeId)
                    .Select(grp => grp.First())
                    .Count();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            return total;
        }

        /// <summary>
        /// Auto estimate collection
        /// </summary>
        /// <returns>Estimated price and a list of missing estimations</returns>
        public async Task<Tuple<float, List<string>>> AutoEstimateCollection(string currency)
        {
            float total = 0;
            List<string> missingList = new();
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                var allGot = await DB.CollectedCard.Where(x => x.Collected > 0).ToListAsync();
                foreach (CollectedCard collectedCard in allGot)
                {
                    var price = await EstimateCardPrice(collectedCard.CardUuid);
                    if (price != null)
                    {
                        //if (currency == "Eur") total += price.GetLastPriceEur;
                        //if (currency == "Usd") total += price.GetLastPriceUsd;
                    }
                    else missingList.Add(collectedCard.CardUuid);
                }
            }
            catch (Exception e) { Logger.Log(e); }
            return new Tuple<float, List<string>>(total, missingList);
        }

        #endregion

        #region Decks

        /// <summary>
        /// Get decks registered
        /// </summary>
        /// <returns>A list containing the decks</returns>
        public async Task<List<Deck>> GetDecks()
        {
            Logger.Log("");
            List<Deck> decks = new();
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                decks = await DB.Decks.ToListAsync();
            }
            catch (Exception e) { Logger.Log(e); }
            return decks;
        }

        /// <summary>
        /// Get a deck by its id
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>The found deck or null</returns>
        public async Task<Deck> GetDeck(string deckId)
        {
            Logger.Log(deckId);
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                Deck deck = await DB.Decks.Where(x => x.DeckId == deckId).FirstOrDefaultAsync();
                return deck;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// <summary>
        /// Gets deck cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>A list of deck-card relations</returns>
        public async Task<List<DeckCard>> GetDeckContent(string deckId)
        {
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                return await DB.DeckCard.Where(x => x.DeckId == deckId).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return new List<DeckCard>();
            }
        }

        /// <summary>
        /// Creates an empty deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns>a reference to the deck</returns>
        public async Task<Deck> CreateDeck_Empty(string title, string description, string colors, int count)
        {
            Logger.Log("");
            if (string.IsNullOrEmpty(title)) return null;
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                Deck deck = new()
                {
                    Title = title,
                    CardCount = count,
                    DeckColors = colors,
                    Description = description
                };
                DB.Decks.Add(deck);
                await DB.SaveChangesAsync();
                return deck;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// <summary>
        /// Creates a filled deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="deckLines"></param>
        /// <returns>A list of messages, empty if everything went well</returns>
        public async Task<Deck> CreateDeck(string title, string description, string colors, int count, IEnumerable<DeckCard> deckLines = null)
        {
            if (deckLines == null) CreateDeck_Empty(title, description, colors, count);
            Logger.Log("");
            List<string> messages = new();
            Deck deck = await CreateDeck_Empty(title, description, colors, count);
            //TODO determine deck count and colors
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                foreach (DeckCard deckLine in deckLines)
                {
                    if (DB.CardArchetypes.Where(x => x.CardUuid == deckLine.CardUuid).Any())
                    {
                        DB.DeckCard.Add(
                            new DeckCard()
                            {
                                DeckId = deck.DeckId,
                                CardUuid = deckLine.CardUuid,
                                Quantity = deckLine.Quantity,
                                RelationType = deckLine.RelationType
                            }
                        );
                        deck.CardCount += deckLine.Quantity;
                    }
                    else
                    {
                        messages.Add("[CardNotFoud]" + deckLine.CardUuid);
                    }
                }

                await DB.SaveChangesAsync();
            }
            catch (Exception e)
            {
                messages.Add("[error]" + e.Message);
                Logger.Log(e);
            }

            return deck;
        }

        /// <summary>
        /// Rename a deck
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="title"></param>
        public async Task RenameDeck(string deckId, string title)
        {
            Logger.Log("");
            if (string.IsNullOrEmpty(title)) return;
            using CollectionDbContext DB = await collec.GetContext();
            var deck = await DB.Decks.Where(x => x.DeckId == deckId).FirstOrDefaultAsync();
            if (deck != null)
            {
                deck.Title = title;
                DB.Entry(deck).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Duplicate a deck
        /// </summary>
        /// <param name="deckToCopy"></param>
        public async Task DuplicateDeck(string deckId)
        {
            Logger.Log("");
            var deckToCopy = await GetDeck(deckId);
            Logger.Log("");
            if (deckToCopy == null) return;
            var newDeck = await CreateDeck_Empty(
                deckToCopy.Title + " - Copy",
                deckToCopy.Description, deckToCopy.DeckColors, deckToCopy.CardCount);
            if (newDeck == null) return;

            using CollectionDbContext DB = await collec.GetContext();
            foreach (DeckCard relation in DB.DeckCard.Where(x => x.DeckId == deckToCopy.DeckId))
            {
                DB.DeckCard.Add(
                    new DeckCard()
                    {
                        CardUuid = relation.CardUuid,
                        DeckId = newDeck.DeckId,
                        Quantity = relation.Quantity,
                        RelationType = relation.RelationType
                    }
                );
            }
            newDeck.CardCount = deckToCopy.CardCount;
            newDeck.DeckColors = deckToCopy.DeckColors;
            DB.Entry(newDeck).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public async Task SaveDeckContent(Deck header, List<DeckCard> lines)
        {
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                Deck d = DB.Decks.Where(x => x.DeckId == header.DeckId).FirstOrDefault();
                if (d != null) await UpdateDeckHeader(d.DeckId, d.Title, header.Description, header.DeckColors, header.CardCount, lines);
                else await CreateDeck(header.Title, header.Description, header.DeckColors, header.CardCount, lines);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        /// <summary>
        /// Change deck entirely
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task UpdateDeckHeader(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content)
        {
            Logger.Log("");
            await DeleteDeck(deckId);
            await CreateDeck(title, description, colors, count, content);
        }

        /// <summary>
        /// Delete a deck
        /// </summary>
        /// <param name="deck"></param>
        public async Task DeleteDeck(string deckId)
        {
            var deck = await GetDeck(deckId);
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            DB.Decks.Remove(deck);
            var cards = DB.DeckCard.Where(x => x.DeckId == deck.DeckId).ToList();
            DB.DeckCard.RemoveRange(cards);
            await DB.SaveChangesAsync();
        }

        public async Task<List<Preco>> GetPrecos()
        {
            string data = await File.ReadAllTextAsync(Folders.File_Precos);
            return JsonSerializer.Deserialize<List<Preco>>(data, new JsonSerializerOptions { IncludeFields = true });
        }

        /// <summary>
        /// Estimate the price of a deck
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>The estimation</returns>
        /// 
        public async Task<Tuple<decimal, List<string>>> EstimateDeckPrice(string deckId, string currency)
        {
            Logger.Log("");
            decimal total = 0;
            List<string> missingList = new();
            using CollectionDbContext DB = await collec.GetContext();
            List<DeckCard> deckCards = await GetDeckContent(deckId);
            foreach (var deckCard in deckCards)
            {
                //TODO
                //var price = await EstimateCardPrice(deckCard.CardUuid);
                //if (price != null)
                //{
                //    if (currency == "Eur") total += price.PriceEur ?? 0;
                //    if (currency == "Usd") total += price.PriceUsd ?? 0;
                //    if (currency == "Edh") total += price.EdhrecScore;
                //}
                //else missingList.Add(deckCard.CardUuid);
            }
            return new Tuple<decimal, List<string>>(total, missingList);
        }

        #endregion

        #region Txt parsing

        /// <summary>
        /// Exports a txt list from a registered deck
        /// format: 
        /// X{SetCode} CardName
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="withSetCode"></param>
        /// <returns>the formated decklist</returns>
        public async Task<string> DeckToTxt(string deckId, bool withSetCode = false)
        {
            Logger.Log("");
            using CollectionDbContext collecDb = await collec.GetContext();
            using MtgDbContext cardInfos = await mtg.GetContext();
            var deck = await collecDb.Decks.Where(x => x.DeckId == deckId).FirstOrDefaultAsync();
            if (deck == null) return "";

            StringBuilder result = new();
            result.AppendLine(deck.Title);
            result.AppendLine(deck.Description);
            result.AppendLine();
            var cardRelations = await GetDeckContent(deck.DeckId);
            int lastRelationType = -1;
            foreach (DeckCard cardRelation in cardRelations
                .Where(x => x.RelationType == 0)
                .OrderBy(x => x.RelationType)
                .ThenBy(x => x.RelationType == 1))
            {
                if (lastRelationType != cardRelation.RelationType)
                {
                    lastRelationType = cardRelation.RelationType;
                    switch (lastRelationType)
                    {
                        case 0: result.AppendLine("Content:"); break;
                        case 1: result.AppendLine("Commandment:"); break;
                        case 2: result.AppendLine("Side:"); break;
                    }
                }
                Cards? card = await cardInfos.cards.Where(x => x.Uuid == cardRelation.CardUuid).FirstOrDefaultAsync();
                if (card != null)
                {
                    result.Append(cardRelation.Quantity);
                    if (withSetCode)
                    {
                        result.Append('{');
                        result.Append(card.SetCode);
                        result.Append('}');
                    }
                    result.Append(' ');
                    result.Append(card.Name);
                    result.AppendLine();
                }
                else result.AppendLine("???");
            }
            return result.ToString();
        }

        public async Task<TxtImportResult> ParseCardList(string cardList)
        {
            TxtImportResult result = new();
            result.Cards = new();
            result.Status = string.Empty;
            result.Detail = string.Empty;
            try
            {
                var lines = cardList.Split(Environment.NewLine).ToList();
                int lineType = 0;
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        Logger.Log(string.Concat(">>>", line));
                        if (line.Trim() == "Main") lineType = 0;
                        if (line.Trim() == "Commandant") lineType = 1;
                        if (line.Trim() == "Side") lineType = 2;
                        else
                        {
                            Tuple<string, DeckCard> entry = await ParseCardLine(line, lineType);
                            if (entry != null)
                            {
                                if (entry.Item1 == string.Empty) result.Cards.Add(entry.Item2);
                                else result.Detail += string.Concat(entry.Item1, Environment.NewLine);
                            }
                        }
                    }
                }
                if (result.Status == string.Empty) { result.Status = "OK"; }
                else result.Status = result.Detail.Count() - 1 + " errors";

            }
            catch (Exception e)
            {
                result.Status = "KO";
                result.Detail = e.Message;
                result.Cards = null;
                Logger.Log(e);
            }
            return result;
        }

        private async Task<Tuple<string, DeckCard>> ParseCardLine(string line, int lineType)
        {
            try
            {
                if (IsLineEmpty(line)) return null;
                var v = await ParseLine(line);
                if (v.Item1 > 0)
                {
                    var c = new DeckCard()
                    {
                        Quantity = v.Item1,
                        CardUuid = v.Item2,
                        RelationType = lineType
                    };
                    return new Tuple<string, DeckCard>(string.Empty, c);
                }
                else
                {
                    return new Tuple<string, DeckCard>(v.Item2, null);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return new Tuple<string, DeckCard>(e.Message, null);
            }
        }

        private async Task<Tuple<int, string>> ParseLine(string line)
        {
            string trimmed = line.Trim();
            try
            {
                var splitted = line.Split(" ");
                if (splitted.Count() < 2) return new Tuple<int, string>(-1, "Ignored line");

                int quantity = int.Parse(splitted[0]);
                string name = line[(line.IndexOf(' ') + 1)..];
                name = name.Split(" // ")[0];
                var uuid = (await GetCardUuidsForGivenCardName(name)).FirstOrDefault();
                if (uuid != null)
                {
                    return new Tuple<int, string>(quantity, name);
                }
                else
                {
                    return new Tuple<int, string>(-1, "Not found : " + name);
                }
            }
            catch (Exception e)
            {
                return new Tuple<int, string>(-1, "Error : " + e.Message);
            }
        }

        private bool IsLineEmpty(string line)
        {
            return line.Trim().Length == 0;
        }

        #endregion

        #region Tags

        /// <summary>
        /// List all existing tags
        /// </summary>
        /// <returns>List of distinct tags</returns>
        public async Task<List<Tag>> GetTags()
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await collec.GetContext();
            tags.Add(null);
            tags.AddRange(
                    DB.Tag.GroupBy(x => x.TagContent).Select(x => x.First())
            );
            return tags;
        }

        /// <summary>
        /// Does this card have this tag
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="tagFilterSelected"></param>
        /// <returns>true if this card has this tag</returns>
        public async Task<bool> HasTag(string cardId, string tagFilterSelected)
        {
            return (await GetCardTags(cardId)).Where(x => x.TagContent == tagFilterSelected).Any();
        }

        /// <summary>
        /// Add a tag to a card
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="text"></param>
        public async Task TagCard(string archetypeId, string text)
        {
            using CollectionDbContext DB = await collec.GetContext();
            DB.Tag.Add(new Tag()
            {
                TagContent = text,
                ArchetypeId = archetypeId
            });
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Remove a tag from a card
        /// </summary>
        /// <param name="cardTag"></param>
        public async Task UnTagCard(string archetypeId, string text)
        {
            using CollectionDbContext DB = await collec.GetContext();
            var cardTag = DB.Tag.Where(x => x.ArchetypeId == archetypeId && x.TagContent == text).FirstOrDefault();
            if (cardTag != null)
            {
                DB.Tag.Remove(cardTag);
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Find if this card has tags
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>List of tags</returns>
        public async Task<List<Tag>> GetCardTags(string archetypeId)
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await collec.GetContext();
            tags.AddRange(DB.Tag.Where(x => x.ArchetypeId == archetypeId));
            return tags;
        }

        #endregion

        #endregion

    }

}

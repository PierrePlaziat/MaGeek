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

    /// <summary>
    /// IMageekService implementation for local operations (monolith archi / server side)
    /// </summary>
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

        #region initialisation

        public async Task<MageekConnectReturn> Client_Connect(string address)
        {
            return MageekConnectReturn.NotImplementedForServer;
        }

        public async Task<MageekInitReturn> Server_Initialize()
        {
            try
            {
                Folders.InitServerFolders();
                if (!File.Exists(Folders.File_CollectionDB)) collec.CreateDb();
                bool needsUpdate = await mtgjson.CheckUpdate();
                return needsUpdate ? MageekInitReturn.Outdated : MageekInitReturn.UpToDate;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return MageekInitReturn.Error;
            }
        }

        public async Task<MageekUpdateReturn> Server_Update()
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

        #endregion

        #region Cards

        public Task<List<SearchedCards>> Cards_Search(string filterName, string lang, int page, int pageSize, string? filterType = null, string? filterKeyword = null, string? filterText = null, string? filterColor = null, string? filterTag = null, bool onlyGot = false, bool colorisOr = false)
        {
            if (filterType==null 
             && filterKeyword==null 
             && filterText==null 
             && filterColor==null 
             && filterTag==null 
             && onlyGot==false 
             && colorisOr==false
            ){
                return NormalSearch(filterName, lang, page, pageSize);
            }
            else
            {
                return AdvancedSearch
                (
                    filterName, lang, page, pageSize,
                    filterType, filterKeyword, filterText, filterColor, filterTag, 
                    onlyGot, 
                    colorisOr
                );
            }
        }
        
        private async Task<List<SearchedCards>> NormalSearch(string filterName, string lang, int page, int pageSize)
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
                    retour.Add(new SearchedCards() {
                        Card = card,
                        CardUuid = card.Uuid,
                        Translation = (await Cards_GetTranslation(card.Uuid, lang)).Name,
                        Collected = await Collec_OwnedCombined(card.Uuid),
                    });
                }
            }
            Logger.Log("Done - " + retour.Count + " results on " + found.Count);
            return retour;
        }
        
        private async Task<List<SearchedCards>> AdvancedSearch(string filterName, string lang, int page, int pageSize, string? filterType, string? filterKeyword, string? filterText, string? filterColor, string? filterTag, bool onlyGot, bool colorisOr)
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
                        if (await Tags_CardHasTag(card.Card.Name, filterTag))
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

        public async Task<List<string>> Cards_UuidsForGivenCardName(string archetypeId)
        {
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.ArchetypeId == archetypeId)
                .Select(p => p.CardUuid)
                .ToListAsync();
        }

        public async Task<string> Cards_NameForGivenCardUuid(string cardUuid)
        {
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.CardUuid == cardUuid)
                .Select(p => p.ArchetypeId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> Cards_UuidsForGivenCardUuid(string uuid)
        {
            return await Cards_UuidsForGivenCardName(await Cards_NameForGivenCardUuid(uuid));
        }

        public async Task<Cards> Cards_GetData(string cardUuid)
        {
            Logger.Log(cardUuid);
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.cards
                .Where(x => x.Uuid == cardUuid)
                .FirstOrDefaultAsync();
        }

        public async Task<CardForeignData> Cards_GetTranslation(string cardUuid, string lang)
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

        public async Task<CardLegalities> Cards_GetLegalities(string CardUuid)
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

        public async Task<List<CardRulings>> Cards_GetRulings(string CardUuid)
        {
            Logger.Log("");
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.cardRulings.Where(x => x.Uuid == CardUuid).ToListAsync();
        }

        public async Task<List<CardRelation>> Cards_GetRelations(string uuid)
        {
            Logger.Log("");
            List<CardRelation> outputCards = new();
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
                                outputCards.Add(new CardRelation()
                                {
                                    Role = role.Value,
                                    CardUuid = role.Value != CardCardRelationRole.token ? archetype : string.Empty,
                                    TokenUuid = role.Value == CardCardRelationRole.token ? archetype : string.Empty
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

        public async Task<Uri> Cards_GetIllustration(string cardUuid, CardImageFormat type, bool back = false)
        {
            try
            {
                string localFileName = Path.Combine(
                    Folders.Illustrations,
                    string.Concat(cardUuid, "_", type.ToString())
                );
                if (!File.Exists(localFileName))
                {
                    string name = await Cards_NameForGivenCardUuid(cardUuid);
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

        public async Task<PriceLine> Cards_GetPrice(string cardUuid)
        {
            using CollectionDbContext collecContext = await collec.GetContext();
            return await collecContext.PriceLine.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
        }

        #endregion

        #region Sets

        public async Task<List<Sets>> Sets_All()
        {
            using MtgDbContext DB = await mtg.GetContext();
            return DB.sets.OrderByDescending(x => x.ReleaseDate).ToList();
        }

        public async Task<Sets> Sets_Get(string setCode)
        {
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.sets.Where(x => x.Code == setCode).FirstOrDefaultAsync();
        }

        public async Task<List<Cards>> Sets_Content(string setCode)
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

        public async Task<int> Sets_Completion(string setCode, bool strict)
        {
            int nb = 0;
            try
            {
                var cardUuids = await Sets_Content(setCode);
                using CollectionDbContext DB = await collec.GetContext();
                foreach (var card in cardUuids)
                {
                    if (strict) nb += await Collec_OwnedVariant(card.Uuid);
                    else nb += await Collec_OwnedCombined(card.Name);
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

        public async Task Collec_SetFavCardVariant(string archetypeId, string cardUuid)
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

        public async Task Collec_Move(string cardUuid, int quantityModification)
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

        public async Task<int> Collec_OwnedVariant(string cardUuid)
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

        public async Task<int> Collec_OwnedCombined(string archetypeId)
        {
            if (string.IsNullOrEmpty(archetypeId)) return 0;
            int count = 0;
            using CollectionDbContext DB = await collec.GetContext();
            List<string> uuids = await DB.CardArchetypes.Where(x => x.ArchetypeId == archetypeId).Select(p => p.CardUuid).ToListAsync();
            foreach (string uuid in uuids) count += await Collec_OwnedVariant(uuid);
            return count;
        }

        public async Task<int> Collec_TotalOwned()
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

        public Task<int> Collec_TotalDifferentOwned(bool combined = true)
        {
            if (combined) return GetTotal_CollectedCombined();
            else return GetTotal_CollectedDiff();
        }

        private async Task<int> GetTotal_CollectedDiff()
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

        private async Task<int> GetTotal_CollectedCombined()
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

        public async Task<int> Collec_TotalDifferentExisting(bool combined = true)
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

        #endregion

        #region Decks

        public async Task<List<Deck>> Decks_All()
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

        public async Task<Deck> Decks_Get(string deckId)
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

        public async Task<List<DeckCard>> Decks_Content(string deckId)
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

        public async Task<Deck> Decks_Create(string title, string description, IEnumerable<DeckCard> deckLines = null)
        {
            if (deckLines == null) await CreateDeck_Empty(title, description);
            Logger.Log("");
            List<string> messages = new();
            Deck deck = await CreateDeck_Empty(title, description);
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

        private async Task<Deck> CreateDeck_Empty(string title, string description)
        {
            Logger.Log("");
            if (string.IsNullOrEmpty(title)) return null;
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                Deck deck = new()
                {
                    Title = title,
                    CardCount = 0,
                    DeckColors = string.Empty,
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

        public async Task Decks_Rename(string deckId, string title)
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

        public async Task Decks_Duplicate(string deckId)
        {
            Logger.Log("");
            var deckToCopy = await Decks_Get(deckId);
            Logger.Log("");
            if (deckToCopy == null) return;
            var newDeck = await CreateDeck_Empty(
                deckToCopy.Title + " - Copy",
                deckToCopy.Description/*, deckToCopy.DeckColors, deckToCopy.CardCount*/);
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

        public async Task Decks_Save(Deck header, List<DeckCard> lines)
        {
            //TODO count and colors
            try
            {
                using CollectionDbContext DB = await collec.GetContext();
                Deck d = DB.Decks.Where(x => x.DeckId == header.DeckId).FirstOrDefault();
                if (d != null) await UpdateDeckHeader(d.DeckId, d.Title, header.Description, header.DeckColors, header.CardCount, lines);
                else await Decks_Create(header.Title, header.Description, /*header.DeckColors, header.CardCount,*/ lines);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public async Task UpdateDeckHeader(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content)
        {
            Logger.Log("");
            await Decks_Delete(deckId);
            await Decks_Create(title, description, /*colors, count,*/ content);
        }

        public async Task Decks_Delete(string deckId)
        {
            var deck = await Decks_Get(deckId);
            Logger.Log("");
            using CollectionDbContext DB = await collec.GetContext();
            DB.Decks.Remove(deck);
            var cards = DB.DeckCard.Where(x => x.DeckId == deck.DeckId).ToList();
            DB.DeckCard.RemoveRange(cards);
            await DB.SaveChangesAsync();
        }

        public async Task<List<Preco>> Decks_Precos()
        {
            string data = await File.ReadAllTextAsync(Folders.File_Precos);
            return JsonSerializer.Deserialize<List<Preco>>(data, new JsonSerializerOptions { IncludeFields = true });
        }

        #endregion

        #region Tags

        public async Task<List<Tag>> Tags_All()
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await collec.GetContext();
            tags.Add(null);
            tags.AddRange(
                    DB.Tag.GroupBy(x => x.TagContent).Select(x => x.First())
            );
            return tags;
        }

        public async Task<bool> Tags_CardHasTag(string cardId, string tagFilterSelected)
        {
            return (await Tags_GetCardTags(cardId)).Where(x => x.TagContent == tagFilterSelected).Any();
        }

        public async Task Tags_TagCard(string archetypeId, string text)
        {
            using CollectionDbContext DB = await collec.GetContext();
            DB.Tag.Add(new Tag()
            {
                TagContent = text,
                ArchetypeId = archetypeId
            });
            await DB.SaveChangesAsync();
        }

        public async Task Tags_UntagCard(string archetypeId, string text)
        {
            using CollectionDbContext DB = await collec.GetContext();
            var cardTag = DB.Tag.Where(x => x.ArchetypeId == archetypeId && x.TagContent == text).FirstOrDefault();
            if (cardTag != null)
            {
                DB.Tag.Remove(cardTag);
                await DB.SaveChangesAsync();
            }
        }

        public async Task<List<Tag>> Tags_GetCardTags(string archetypeId)
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await collec.GetContext();
            tags.AddRange(DB.Tag.Where(x => x.ArchetypeId == archetypeId));
            return tags;
        }

        #endregion

        #region CardLists

        public async Task<string> CardLists_FromDeck(string deckId, bool withSetCode = false)
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
            var cardRelations = await Decks_Content(deck.DeckId);
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

        public async Task<CardList> CardLists_Parse(string cardList)
        {
            CardList result = new();
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
                var uuid = (await Cards_UuidsForGivenCardName(name)).FirstOrDefault();
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

        #endregion

    }

}

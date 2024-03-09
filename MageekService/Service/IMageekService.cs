using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;

namespace MageekCore.Service
{

    public interface IMageekService
    {

        Task<MageekConnectReturn> Connect(string address);
        Task<MageekInitReturn> Initialize();
        Task<MageekUpdateReturn> Update();

        Task<List<SearchedCards>> NormalSearch(string filterName, string lang, int page, int pageSize);
        Task<List<SearchedCards>> AdvancedSearch(string filterName, string lang, string filterType, string filterKeyword, string filterText, string filterColor, string filterTag, bool onlyGot, bool colorisOr, int page, int pageSize);

        Task<List<string>> GetCardUuidsForGivenCardName(string archetypeId);
        Task<string> GetCardNameForGivenCardUuid(string cardUuid);
        Task<List<string>> GetCardUuidsForGivenCardUuid(string uuid);

        Task<Cards> FindCard_Data(string cardUuid);
        Task<CardForeignData> GetTranslatedData(string cardUuid, string lang);
        Task<string?> GetCardBack(string cardUuid);
        Task<CardLegalities> GetLegalities(string CardUuid);
        Task<List<CardRulings>> GetRulings(string CardUuid);
        Task<List<CardCardRelation>> FindRelated(string SelectedUuid);
        Task<Uri> RetrieveImage(string cardUuid, CardImageFormat type, bool back = false);
        Task<PriceLine> EstimateCardPrice(string cardUuid);

        Task<List<Sets>> LoadSets();
        Task<Sets> GetSet(string setCode);
        Task<List<Cards>> GetCardsFromSet(string setCode);
        Task<int> GetMtgSetCompletion(string setCode, bool strict);

        Task SetFav(string archetypeId, string cardUuid);
        Task CollecMove(string cardUuid, int quantityModification);
        Task<int> Collected_SingleVariant(string cardUuid);
        Task<int> Collected_AllVariants(string archetypeId);
        Task<int> GetTotal_Collected();
        Task<int> GetTotal_CollectedDiff();
        Task<int> GetTotal_CollectedArchetype();
        Task<int> GetTotal_ExistingArchetypes();
        //Task<Tuple<float, List<string>>> AutoEstimateCollection(string currency);

        Task<List<Deck>> GetDecks();
        Task<Deck> GetDeck(string deckId);
        Task<List<DeckCard>> GetDeckContent(string deckId);
        Task<Deck> CreateDeck(string title, string description, string colors, int count, IEnumerable<DeckCard> deckLines = null);
        Task RenameDeck(string deckId, string title);
        Task DuplicateDeck(string deckId);
        Task SaveDeckContent(Deck header, List<DeckCard> lines);
        Task UpdateDeckHeader(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content);
        Task DeleteDeck(string deckId);
        Task<List<Preco>> GetPrecos();
        //Task<Tuple<decimal, List<string>>> EstimateDeckPrice(string deckId, string currency);

        Task<string> DeckToTxt(string deckId, bool withSetCode = false);
        Task<TxtImportResult> ParseCardList(string cardList);

        Task<List<Tag>> GetTags();
        Task<bool> HasTag(string cardId, string tagFilterSelected);
        Task TagCard(string archetypeId, string text);
        Task UnTagCard(string archetypeId, string text);
        Task<List<Tag>> GetCardTags(string archetypeId);

    }

}

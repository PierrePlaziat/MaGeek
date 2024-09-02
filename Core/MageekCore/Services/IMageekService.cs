using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Data.MtgFetched.Entities;

namespace MageekCore.Services
{

    /// <summary>
    /// Mageek functionalities contract
    /// Implemented twice:
    /// - MageekService, used for local operations (monolith archi / server side)
    /// - MageekServer, used for distant operations (distributed archi / client side)
    /// </summary>
    public interface IMageekService
    {

        #region Initialisation

        /// <summary>
        /// Access distant server
        /// </summary>
        /// <param name="serverAddress">formatted as "ip:port"</param>
        /// <returns>Comprehensive enum</returns>
        Task<MageekConnectReturn> Client_Connect(string user, string pass, string serverAddress);
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="serverAddress">formatted as "ip:port"</param>
        /// <returns>Comprehensive enum</returns>
        Task<MageekConnectReturn> Client_Register(string user, string pass);
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="serverAddress">formatted as "ip:port"</param>
        /// <returns>Comprehensive enum</returns>
        Task<MageekConnectReturn> Client_Authentify(string user, string pass);

        /// <summary>
        /// Setup system for server abilities
        /// </summary>
        /// <returns>Comprehensive enum</returns>
        Task<MageekInitReturn> Server_Initialize();

        /// <summary>
        /// Update server data
        /// </summary>
        /// <returns>Comprehensive enum</returns>
        Task<MageekUpdateReturn> Server_Update();

        #endregion

        #region Cards

        /// <summary>
        /// Search cards
        /// </summary>
        /// <param name="cardName">Can be incomplete</param>
        /// <param name="lang">Search in corresponding translations</param>
        /// <param name="page">Pagination purpose</param>
        /// <param name="pageSize">Pagination purpose</param>
        /// <param name="cardType">Advanced parameter</param>
        /// <param name="keyword">Advanced parameter</param>
        /// <param name="text">Advanced parameter, search input in card text</param>
        /// <param name="color">Advanced parameter, card color identity</param>
        /// <param name="tag">Advanced parameter</param>
        /// <param name="onlyGot">Advanced parameter, ignore not owned cards</param>
        /// <param name="colorisOr">Advanced parameter, Precises how the color filter is interpreted</param>
        /// <returns>List of cards with some additional data</returns>
        Task<List<SearchedCards>> Cards_Search(
            string cardName, string lang, int page, int pageSize,
            string? cardType = null, string? keyword = null, string? text = null, string? color = null, string? tag = null,
            bool onlyGot = false, bool colorisOr = false
        );

        /// <summary>
        /// Get all card variant ids from an archetypal card name
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>a list of uuid</returns>
        Task<List<string>> Cards_UuidsForGivenCardName( string cardName);

        /// <summary>
        /// Get an archetypal card id from an card variant uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>a single archetype id</returns>
        Task<string> Cards_NameForGivenCardUuid( string cardUuid);

        Task<List<string>> Cards_UuidsForGivenCardUuid( string cardUuid);

        /// <summary>
        /// get the gameplay data of the card
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>Archetype</returns>
        Task<Cards> Cards_GetData( string cardUuid);

        /// <summary>
        /// Get all traducted infos of the card
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="lang"></param>
        /// <returns>The data if any</returns>
        Task<CardForeignData> Cards_GetTranslation( string cardUuid, string lang);

        /// <summary>
        /// Get card legalities
        /// </summary>
        /// <param name="selectedCard"></param>
        /// <returns>List of legalities</returns>
        Task<CardLegalities> Cards_GetLegalities( string cardUuid);

        /// <summary>
        /// get card rulings
        /// </summary>
        /// <param name="selectedCard"></param>
        /// <returns>List of rulings</returns>
        Task<List<CardRulings>> Cards_GetRulings( string cardUuid);

        Task<List<CardRelation>> Cards_GetRelations( string cardUuid);

        /// <summary>
        /// Get the illustration of a card, save it locally if not already done
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>a local url to a jpg</returns>
        Task<Uri> Cards_GetIllustration( string cardUuid, CardImageFormat type, bool back = false);

        /// <summary>
        /// Estimate the price of a card
        /// </summary>
        /// <param name="v"></param>
        /// <param name="currency"></param>
        /// <returns>The estimation</returns>
        Task<PriceLine> Cards_GetPrice( string cardUuid);

        #endregion

        #region Sets

        /// <summary>
        /// Get all sets
        /// </summary>
        /// <returns>List of sets</returns>
        Task<List<Sets>> Sets_All();

        Task<Sets> Sets_Get( string setCode);

        /// <summary>
        /// Get cards in a set
        /// </summary>
        /// <param name="setCode"></param>
        /// <returns>Uuid list of cards in set</returns>
        Task<List<string>> Sets_Content(string setCode);

        /// <summary>
        /// How many cards collected in this set
        /// </summary>
        /// <param name="setCode"></param>
        /// <param name="strict">if set to false, the archetype from any set counts</param>
        /// <returns>the distinct count</returns>
        Task<int> Sets_Completion(string user, string setCode, bool strict);

        #endregion

        #region Collec

        /// <summary>
        /// Determine favorite card variant for a card archetype
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="cardUuid"></param>
        Task Collec_SetFavCardVariant(string user, string cardName, string cardUuid);

        /// <summary>
        /// Add or remove card in the collection
        /// </summary>
        /// <param name="cardUuid">from mtgjson</param>
        /// <param name="quantityModification">how many</param>
        /// <returns>Quantity in collec before and after the move</returns>
        Task Collec_Move(string user, string cardUuid, int quantity);

        /// <summary>
        /// Counts how many cards collected variably
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="onlyThisVariant">set to false if you want to perform archetypal search from this card variant</param>
        /// <returns>The count</returns>
        Task<int> Collec_OwnedVariant(string user, string cardUuid);

        Task<int> Collec_OwnedCombined(string user, string cardName);

        /// <summary>
        /// Totality of cards including their quantity
        /// </summary>
        /// <returns></returns>
        Task<int> Collec_TotalOwned(string user);

        /// <summary>
        /// Totality of different archetypes
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Totality of cards variants but doesnt sur their quantity
        /// </summary>
        /// <returns></returns>
        Task<int> Collec_TotalDifferentOwned(string user, bool combined = true);

        /// <summary>
        /// Totality of different existing card archetypes
        /// </summary>
        /// <returns></returns>
        Task<int> Collec_TotalDifferentExisting(bool combined = true);

        #endregion

        #region Decks

        /// <summary>
        /// Get decks registered
        /// </summary>
        /// <returns>A list containing the decks</returns>
        Task<List<Deck>> Decks_All(string user);

        /// <summary>
        /// Get a deck by its id
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>The found deck or null</returns>
        Task<Deck> Decks_Get(string user, string deckId);

        /// <summary>
        /// Gets deck cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>A list of deck-card relations</returns>
        Task<List<DeckCard>> Decks_Content(string user, string deckId);

        /// <summary>
        /// Creates an empty deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns>a reference to the deck</returns>
        /// <summary>
        /// Creates a filled deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="deckLines"></param>
        /// <returns>A list of messages, empty if everything went well</returns>
        Task Decks_Create(string user, string title, string description, IEnumerable<DeckCard> deckLines);

        /// <summary>
        /// Rename a deck
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="title"></param>
        Task Decks_Rename(string user, string deckId, string title);

        /// <summary>
        /// Duplicate a deck
        /// </summary>
        /// <param name="deckToCopy"></param>
        Task Decks_Duplicate(string user, string deckId);

        /// <summary>
        /// Change deck entirely
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        Task Decks_Save(string user, Deck header, List<DeckCard> lines);

        /// <summary>
        /// Delete a deck
        /// </summary>
        /// <param name="deck"></param>
        Task Decks_Delete(string user, string deckId);

        Task<List<Preco>> Decks_Precos();

        #endregion

        #region Tags

        /// <summary>
        /// List all existing tags
        /// </summary>
        /// <returns>List of distinct tags</returns>
        Task<List<string>> Tags_All(string user);

        /// <summary>
        /// Does this card have this tag
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="tagFilterSelected"></param>
        /// <returns>true if this card has this tag</returns>
        Task<bool> Tags_CardHasTag(string user, string cardName, string tag);

        /// <summary>
        /// Add a tag to a card
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="text"></param>
        Task Tags_TagCard(string user, string cardName, string tag);

        /// <summary>
        /// Remove a tag from a card
        /// </summary>
        /// <param name="cardTag"></param>
        Task Tags_UntagCard(string user, string cardName, string tag);

        /// <summary>
        /// Find if this card has tags
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>List of tags</returns>
        Task<List<Tag>> Tags_GetCardTags(string user, string cardName);


        #endregion

        #region CardLists

        Task<CardList> CardLists_Parse(string input);

        /// <summary>
        /// Exports a txt list from a registered deck
        /// format: 
        /// X{SetCode} CardName
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="withSetCode"></param>
        /// <returns>the formated decklist</returns>
        Task<string> CardLists_FromDeck(string user, string deckId, bool withSetCode = false);

        #endregion

    }

}

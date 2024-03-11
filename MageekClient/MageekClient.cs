using Grpc.Net.Client;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Service;

namespace MageekMaui
{

    //TODO

    public class MageekClient : IMageekService
    {

        #region Connexion

        bool connected = false;
        private GrpcChannel channel;
        //Greeter.GreeterClient greeter;
        //Collectionner.CollectionnerClient collection;

        //Collectionner.CollectionnerClient collectionner;
        //DeckBuilder.DeckBuilderClient deckBuilder;
        //Tagger.TaggerClient tagger;

        const int timeout = 5;

        private void Init()
        {
            //collectionner = new Collectionner.CollectionnerClient(channel);
            //deckBuilder = new DeckBuilder.DeckBuilderClient(channel);
            //tagger = new Tagger.TaggerClient(channel);
            //connected = true;
        }

        public Task<MageekConnectReturn> Client_Connect(string serverAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<MageekInitReturn> Server_Initialize()
        {
            return MageekInitReturn.NotImplementedForClient;
        }

        public async Task<MageekUpdateReturn> Server_Update()
        {
            return MageekUpdateReturn.NotImplementedForClient;
        }

        #endregion

        #region Implementation

        public Task<List<SearchedCards>> Cards_Search(string cardName, string lang, int page, int pageSize, string? cardType = null, string? keyword = null, string? text = null, string? color = null, string? tag = null, bool onlyGot = false, bool colorisOr = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> Cards_UuidsForGivenCardName(string cardName)
        {
            throw new NotImplementedException();
        }

        public Task<string> Cards_NameForGivenCardUuid(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> Cards_UuidsForGivenCardUuid(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<Cards> Cards_GetData(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<CardForeignData> Cards_GetTranslation(string cardUuid, string lang)
        {
            throw new NotImplementedException();
        }

        public Task<CardLegalities> Cards_GetLegalities(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<CardRulings>> Cards_GetRulings(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<CardRelation>> Cards_GetRelations(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> Cards_GetIllustration(string cardUuid, CardImageFormat type, bool back = false)
        {
            throw new NotImplementedException();
        }

        public Task<PriceLine> Cards_GetPrice(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<Sets>> Sets_All()
        {
            throw new NotImplementedException();
        }

        public Task<Sets> Sets_Get(string setCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<Cards>> Sets_Content(string setCode)
        {
            throw new NotImplementedException();
        }

        public Task<int> Sets_Completion(string setCode, bool strict)
        {
            throw new NotImplementedException();
        }

        public Task Collec_SetFavCardVariant(string cardName, string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task Collec_Move(string cardUuid, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collec_OwnedVariant(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collec_OwnedCombined(string cardName)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collec_TotalOwned()
        {
            throw new NotImplementedException();
        }

        public Task<int> Collec_TotalDifferentOwned(bool combined = true)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collec_TotalDifferentExisting(bool combined = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<Deck>> Decks_All()
        {
            throw new NotImplementedException();
        }

        public Task<Deck> Decks_Get(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeckCard>> Decks_Content(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<Deck> Decks_Create(string title, string description, IEnumerable<DeckCard> deckLines = null)
        {
            throw new NotImplementedException();
        }

        public Task Decks_Rename(string deckId, string title)
        {
            throw new NotImplementedException();
        }

        public Task Decks_Duplicate(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task Decks_Save(Deck header, List<DeckCard> lines)
        {
            throw new NotImplementedException();
        }

        public Task Decks_Delete(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Preco>> Decks_Precos()
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> Tags_All()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Tags_CardHasTag(string cardName, string tag)
        {
            throw new NotImplementedException();
        }

        public Task Tags_TagCard(string cardName, string tag)
        {
            throw new NotImplementedException();
        }

        public Task Tags_UntagCard(string cardName, string tag)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> Tags_GetCardTags(string cardName)
        {
            throw new NotImplementedException();
        }

        public Task<CardList> CardLists_Parse(string input)
        {
            throw new NotImplementedException();
        }

        public Task<string> CardLists_FromDeck(string deckId, bool withSetCode = false)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

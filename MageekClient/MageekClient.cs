using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Service;
using System;
using System.Net;

//TODO sortie dans son propre projet

namespace MageekMaui
{

    public class MageekClient : IMageekService
    {

        #region Connexion

        bool connected = false;
        private GrpcChannel channel;
        Greeter.GreeterClient greeter;
        Collectionner.CollectionnerClient collection;

        Collectionner.CollectionnerClient collectionner;
        DeckBuilder.DeckBuilderClient deckBuilder;
        Tagger.TaggerClient tagger;

        const int timeout = 5;

        private void Init()
        {
            collectionner = new Collectionner.CollectionnerClient(channel);
            deckBuilder = new DeckBuilder.DeckBuilderClient(channel);
            tagger = new Tagger.TaggerClient(channel);
            connected = true;
        }

        #endregion

        #region Implem

        public async Task<MageekConnectReturn> Connect(string address)
        {
            Console.WriteLine("Connecting...");
            try
            {
                channel = GrpcChannel.ForAddress(address);
                greeter = new Greeter.GreeterClient(channel);
                var reply = await greeter.SayHelloAsync(
                    new HelloRequest { Name = "GreeterClient" },
                    deadline: DateTime.UtcNow.AddSeconds(timeout));
                collection = new Collectionner.CollectionnerClient(channel);
                Console.WriteLine("Success!");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Timeout.");
                connected = false;
                return MageekConnectReturn.Failure;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connected = false;
                return MageekConnectReturn.Failure;
            }
            Init();
            return MageekConnectReturn.Success;
        }

        public async Task<MageekInitReturn> Initialize()
        {
            return MageekInitReturn.NotImplementedForClient;
        }

        public async Task<MageekUpdateReturn> Update()
        {
            return MageekUpdateReturn.NotImplementedForClient;
        }

        public Task<List<SearchedCards>> NormalSearch(string filterName, string lang, int page, int pageSize)
        {
            throw new NotImplementedException();
            //var reply = await collection.norm(
            //       new HelloRequest { Name = "GreeterClient" },
            //       deadline: DateTime.UtcNow.AddSeconds(timeout));
        }

        public Task<List<SearchedCards>> AdvancedSearch(string filterName, string lang, string filterType, string filterKeyword, string filterText, string filterColor, string filterTag, bool onlyGot, bool colorisOr, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetCardUuidsForGivenCardName(string archetypeId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCardNameForGivenCardUuid(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetCardUuidsForGivenCardUuid(string uuid)
        {
            throw new NotImplementedException();
        }

        public Task<Cards> FindCard_Data(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<CardForeignData> GetTranslatedData(string cardUuid, string lang)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetCardBack(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<CardLegalities> GetLegalities(string CardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<CardRulings>> GetRulings(string CardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<CardCardRelation>> FindRelated(string SelectedUuid, string SelectedArchetype)
        {
            throw new NotImplementedException();
        }

        public List<CardCardRelation> FindRelateds(string uuid, string originalarchetype)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> RetrieveImage(string cardUuid, CardImageFormat type)
        {
            throw new NotImplementedException();
        }

        public Task<PriceLine> EstimateCardPrice(string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<Sets>> LoadSets()
        {
            throw new NotImplementedException();
        }

        public Task<Sets> GetSet(string setCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<Cards>> GetCardsFromSet(string setCode)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetMtgSetCompletion(string setCode, bool strict)
        {
            throw new NotImplementedException();
        }

        public Task SetFav(string archetypeId, string cardUuid)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collected(string cardUuid, bool onlyThisVariant = true)
        {
            throw new NotImplementedException();
        }

        public Task<int> Collected_AllVariants(string archetypeId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotal_Collected()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotal_CollectedDiff()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotal_CollectedArchetype()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotal_ExistingArchetypes()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<float, List<string>>> AutoEstimateCollection(string currency)
        {
            throw new NotImplementedException();
        }

        public Task<List<Deck>> GetDecks()
        {
            throw new NotImplementedException();
        }

        public Task<Deck> GetDeck(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeckCard>> GetDeckContent(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<Deck> CreateDeck_Empty(string title, string description, string colors, int count)
        {
            throw new NotImplementedException();
        }

        public Task<Deck> CreateDeck(string title, string description, string colors, int count, IEnumerable<DeckCard> deckLines)
        {
            throw new NotImplementedException();
        }

        public Task RenameDeck(string deckId, string title)
        {
            throw new NotImplementedException();
        }

        public Task DuplicateDeck(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task SaveDeckContent(Deck header, List<DeckCard> lines)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeckHeader(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeck(string deckId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Preco>> GetPrecos()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<decimal, List<string>>> EstimateDeckPrice(string deckId, string currency)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeckToTxt(string deckId, bool withSetCode = false)
        {
            throw new NotImplementedException();
        }

        public Task<TxtImportResult> ParseCardList(string cardList)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetTags()
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasTag(string cardId, string tagFilterSelected)
        {
            throw new NotImplementedException();
        }

        public Task TagCard(string archetypeId, string text)
        {
            throw new NotImplementedException();
        }

        public Task UnTagCard(string archetypeId, string text)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetCardTags(string archetypeId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

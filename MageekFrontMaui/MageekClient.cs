using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using System;

//TODO sortie dans son propre projet

namespace MageekMaui
{

    public interface IMageekClient
    {

        abstract Task<bool> Connect(string address);

        abstract Task<Reply_CollecMove> CollecMove(Request_CollecMove request);
        abstract Task<Reply_Quantity> Collected(Request_Collected request);

        abstract Task<Reply_DeckList> GetDecks(Request_Default request);
        abstract Task<Reply_Deck> GetDeckInfo(Request_DeckId request);
        abstract Task<Reply_DeckContent> GetDeckContent(Request_DeckId request);
        abstract Task<Reply_Default> CreateDeck(Request_CreateDeck request);
        abstract Task<Reply_Default> UpdateDeck(Request_UpdateDeck request);
        abstract Task<Reply_Default> DeleteDeck(Request_DeckId request);


        abstract Task<Reply_TagList> GetExistingTags(Request_Default request);
        abstract Task<Reply_TagList> GetCardTags(Request_archetypeId request);
        abstract Task<Reply_Default> TagCard(Request_CardTag request);
        abstract Task<Reply_Default> UnTagCard(Request_CardTag request);
        abstract Task<Reply_HasTag> HasTag(Request_CardTag request);

    }

    public class MageekClient : IMageekClient
    {

        #region Connexion

        bool connected = false;
        private GrpcChannel channel;
        Greeter.GreeterClient greeter;

        Collectionner.CollectionnerClient collectionner;
        DeckBuilder.DeckBuilderClient deckBuilder;
        Tagger.TaggerClient tagger;

        const int timeout = 5;

        public async Task<bool> Connect(string address)
        {
            Console.WriteLine("Connecting...");
            try
            {
                channel = GrpcChannel.ForAddress(address);
                greeter = new Greeter.GreeterClient(channel);
                var reply = await greeter.SayHelloAsync(
                    new HelloRequest { Name = "GreeterClient" },
                    deadline: DateTime.UtcNow.AddSeconds(timeout));
                Console.WriteLine("Success!");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Timeout.");
                connected = false;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connected = false;
                return false;
            }
            Init();
            return true;
        }

        private void Init()
        {
            collectionner = new Collectionner.CollectionnerClient(channel);
            deckBuilder = new DeckBuilder.DeckBuilderClient(channel);
            tagger = new Tagger.TaggerClient(channel);
            connected = true;
        }

        #endregion

        #region Boilerplate

        public async Task<Reply_CollecMove> CollecMove (Request_CollecMove request)
        {
            if (!connected) return null;
            var reply = await collectionner.CollecMoveAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Quantity> Collected(Request_Collected request)
        {
            if (!connected) return null;
            var reply = await collectionner.CollectedAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        
        public async Task<Reply_DeckList> GetDecks(Request_Default request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.GetDecksAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Deck> GetDeckInfo(Request_DeckId request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.GetDeckInfoAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_DeckContent> GetDeckContent(Request_DeckId request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.GetDeckContentAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Default> CreateDeck(Request_CreateDeck request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.CreateDeckAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Default> UpdateDeck(Request_UpdateDeck request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.UpdateDeckAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Default> DeleteDeck(Request_DeckId request)
        {
            if (!connected) return null;
            var reply = await deckBuilder.DeleteDeckAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }


        public async Task<Reply_TagList> GetExistingTags(Request_Default request)
        {
            if (!connected) return null;
            var reply = await tagger.GetExistingTagsAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_TagList> GetCardTags(Request_archetypeId request)
        {
            if (!connected) return null;
            var reply = await tagger.GetCardTagsAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Default> TagCard(Request_CardTag request)
        {
            if (!connected) return null;
            var reply = await tagger.TagCardAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_Default> UnTagCard(Request_CardTag request)
        {
            if (!connected) return null;
            var reply = await tagger.UnTagCardAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }
        public async Task<Reply_HasTag> HasTag(Request_CardTag request)
        {
            if (!connected) return null;
            var reply = await tagger.HasTagAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(timeout)
            );
            return reply;
        }

        #endregion

    }

}

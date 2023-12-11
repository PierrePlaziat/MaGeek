using Grpc.Net.Client;

namespace MageekMaui
{

    public interface IMageekClient
    {
        public Task<bool> SayHello();
    }
    public class MageekClient : IMageekClient
    {

        GrpcChannel channel;
        Greeter.GreeterClient greeter;

        Collectionner.CollectionnerClient collectionner;
        DeckBuilder.DeckBuilderClient deckBuilder;

        public async Task<bool> SayHello()
        {
            try
            {
                channel = GrpcChannel.ForAddress("http://10.0.2.2:8089");
                greeter = new Greeter.GreeterClient(channel);
                collectionner = new Collectionner.CollectionnerClient(channel);
                deckBuilder = new DeckBuilder.DeckBuilderClient(channel);

                var reply = await greeter.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });

                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<Reply_CollecMove> CollecMove (Request_CollecMove request)
        {
            var reply = await collectionner.CollecMoveAsync(request);
            return reply;
        }
        
        public async Task<Reply_Quantity> Collected(Request_Collected request)
        {
            var reply = await collectionner.CollectedAsync(request);
            return reply;
        }
        
        public async Task<Reply_DeckList> GetDecks(Request_Default request)
        {
            var reply = await deckBuilder.GetDecksAsync(request);
            return reply;
        }
        
        public async Task<Reply_Deck> GetDeckInfo(Request_DeckId request)
        {
            var reply = await deckBuilder.GetDeckInfoAsync(request);
            return reply;
        }
        
        public async Task<Reply_DeckContent> GetDeckContent(Request_DeckId request)
        {
            var reply = await deckBuilder.GetDeckContentAsync(request);
            return reply;
        }
        
        public async Task<Reply_Default> CreateDeck(Request_CreateDeck request)
        {
            var reply = await deckBuilder.CreateDeckAsync(request);
            return reply;
        }
        
        public async Task<Reply_Default> UpdateDeck(Request_UpdateDeck request)
        {
            var reply = await deckBuilder.UpdateDeckAsync(request);
            return reply;
        }
        
        public async Task<Reply_Default> DeleteDeck(Request_DeckId request)
        {
            var reply = await deckBuilder.DeleteDeckAsync(request);
            return reply;
        }

    }

}

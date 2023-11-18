using Grpc.Core;

namespace MageekGrpc.Services
{
    public class DeckBuilderService : DeckBuilder.DeckBuilderBase
    {

        private readonly ILogger<DeckBuilderService> _logger;
        public DeckBuilderService(ILogger<DeckBuilderService> logger)
        {
            _logger = logger;
        }

        public override Task<Reply_DeckList> GetDecks(Request_Default request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_DeckList
            {
                // ?
            });
        }

        public override Task<Reply_Deck> GetDeckInfo(Request_DeckId request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Deck
            {
                CardCount = 0,
                DeckColors = null,
                DeckId = request.DeckId,
                Description = "",
                Title = ""
            });
        }

        public override Task<Reply_DeckContent> GetDeckContent(Request_DeckId request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_DeckContent
            {
                // ?
            });
        }

        public override Task<Reply_Default> CreateDeck(Request_CreateDeck request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Default
            {
                Success = true
            });
        }

        public override Task<Reply_Default> UpdateDeck(Request_UpdateDeck request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Default
            {
                Success = true
            });
        }

        public override Task<Reply_Default> DeleteDeck(Request_DeckId request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Default
            {
                Success = true
            });
        }

    }
}

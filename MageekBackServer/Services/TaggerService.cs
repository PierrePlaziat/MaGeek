using Grpc.Core;

namespace MageekGrpc.Services
{
    public class TaggerService : Tagger.TaggerBase
    {

        private readonly ILogger<TaggerService> _logger;
        public TaggerService(ILogger<TaggerService> logger)
        {
            _logger = logger;
        }

        public override Task<Reply_TagList> GetExistingTags(Request_Default request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_TagList
            {
                // ?
            });
        }

        public override Task<Reply_TagList> GetCardTags(Request_archetypeId request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_TagList
            {
                // ?
            });
        }

        public override Task<Reply_Default> TagCard(Request_CardTag request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Default
            {
                Success = true
            });
        }

        public override Task<Reply_Default> UnTagCard(Request_CardTag request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Default
            {
                Success = true
            });
        }

        public override Task<Reply_HasTag> HasTag(Request_CardTag request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_HasTag
            {
                HasTag = false
            });
        }

    }
}

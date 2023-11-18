using Grpc.Core;

namespace MageekGrpc.Services
{
    public class CollectionnerService : Collectionner.CollectionnerBase
    {

        private readonly ILogger<CollectionnerService> _logger;
        public CollectionnerService(ILogger<CollectionnerService> logger)
        {
            _logger = logger;
        }

        public override Task<Reply_CollecMove> CollecMove(Request_CollecMove request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_CollecMove
            {
                QuantityAfterMove = 0,
                QuantityBeforeMove = 0,
                Success = true
            });
        }
        
        public override Task<Reply_Quantity> Collected(Request_Collected request, ServerCallContext context)
        {
            // TODO
            return Task.FromResult(new Reply_Quantity
            {
                Quantity = 0,
                Success = true
            });
        }

    }
}

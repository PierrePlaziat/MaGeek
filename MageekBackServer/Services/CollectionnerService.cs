using Grpc.Core;
using MageekService;

namespace MageekGrpc.Services
{

    public class CollectionnerService : Collectionner.CollectionnerBase
    {

        private readonly ILogger<CollectionnerService> _logger;
        private readonly IMageekServer _server;
        
        public CollectionnerService(ILogger<CollectionnerService> logger)
        {
            _logger = logger;
            _server = new MageekServer();
        }

        public override Task<Reply_CollecMove> CollecMove(
            Request_CollecMove request, 
            ServerCallContext context
        ){
            var retour = _server.CollecMove(
                request.CardUuid,
                request.QuantityModification
            ).Result;
            return Task.FromResult(new Reply_CollecMove
            {
                QuantityAfterMove = retour.Item1,
                QuantityBeforeMove = retour.Item2,
                Success = true
            });
        }
        
        public override Task<Reply_Quantity> Collected(
            Request_Collected request, 
            ServerCallContext context
        ){
            var retour = _server.Collected(
                request.CardUuid, 
                request.OnlyThisVariant
            ).Result;
            return Task.FromResult(new Reply_Quantity
            {
                Quantity = retour,
                Success = true
            });
        }

    }

}

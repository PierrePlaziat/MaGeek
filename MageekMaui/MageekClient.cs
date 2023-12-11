using Grpc.Net.Client;

namespace MageekMaui
{

    public interface IMageekClient
    {
        public Task<string> SayHello();
    }
    public class MageekClient : IMageekClient
    {

        public async Task<string> SayHello()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("http://10.0.2.2:8089");
                var client = new Greeter.GreeterClient(channel);
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
                return "Greeting: " + reply.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

    }

}

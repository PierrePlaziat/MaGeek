using Grpc.Net.Client;

namespace MageekMaui
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            CounterBtn.Text = ConnectGrpc().Result;
        }

        private async Task<string> ConnectGrpc()
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

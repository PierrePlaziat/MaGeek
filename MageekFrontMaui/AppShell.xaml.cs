namespace MageekMaui
{

    public partial class AppShell : Shell
    {

        public AppShell()
        {
            bool success = GrpcCo().Result;
            if(!success) { Environment.Exit(0); }
            InitializeComponent();
        }

        private async Task<bool> GrpcCo()
        {
            return await ServiceHelper.GetService<IMageekClient>().SayHello();
        }

    }

}

using MageekClient.Services;
using MageekCore.Services;

namespace MageekMobile.ViewModels
{
    public partial class CollecViewModel : ViewModel
    {

        private IMageekService client;

        public CollecViewModel(IMageekService client)
        {
            this.client = client;
        }

    }
}
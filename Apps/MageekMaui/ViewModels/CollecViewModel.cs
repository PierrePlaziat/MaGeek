using MageekClient.Services;
using MageekCore.Services;

namespace MageekMaui.ViewModels
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
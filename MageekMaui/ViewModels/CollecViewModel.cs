namespace MageekMaui.ViewModels
{
    public partial class CollecViewModel : ViewModel
    {

        private IMageekClient client;

        public CollecViewModel(IMageekClient client)
        {
            this.client = client;
        }

    }
}
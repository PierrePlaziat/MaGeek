namespace MageekMaui.ViewModels
{
    public partial class CollecViewModel : ViewModel
    {

        private MageekClient client;

        public CollecViewModel(MageekClient client)
        {
            this.client = client;
        }

    }
}
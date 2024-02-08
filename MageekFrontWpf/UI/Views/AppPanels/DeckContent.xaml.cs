using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckContent : BaseUserControl
    {
        private DeckContentViewModel vm;

        public DeckContent()
        {
        }
        
        public DeckContent(DeckContentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            
        }

        private void SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void UnsetCommandant(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void SetCommandant(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ToSide(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ToDeck(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

}

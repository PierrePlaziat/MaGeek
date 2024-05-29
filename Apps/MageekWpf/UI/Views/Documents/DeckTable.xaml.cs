using MageekFrontWpf.UI.ViewModels;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckTable : BaseUserControl
    {

        private DocumentViewModel vm;

        public DeckTable() {}

        public void SetDataContext(DocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void ToCommandant_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ToSide_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void AddOne_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void RemoveOne_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

}

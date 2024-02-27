using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System.Windows.Controls;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckContent : BaseUserControl
    {

        private DeckDocumentViewModel vm;

        public DeckContent() {}
        
        public void SetDataContext(DeckDocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((ListView)sender).UnselectAll();
        }

        private void ButtonLess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenedDeckEntry entry = ((OpenedDeckEntry)((Button)sender).CommandParameter);
            vm.LessCard(entry).ConfigureAwait(false);
        }

        private void ButtonMore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenedDeckEntry entry = ((OpenedDeckEntry)((Button)sender).CommandParameter);
            vm.MoreCard(entry).ConfigureAwait(false);
        }

        private void SetCommandant(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            OpenedDeckEntry entry = (OpenedDeckEntry)item.CommandParameter;
            vm.ToCommandant(entry);
        }

        private void ToSide(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            OpenedDeckEntry entry = (OpenedDeckEntry)item.CommandParameter;
            vm.ToSide(entry);
        }

        private void ToDeck(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            OpenedDeckEntry entry = (OpenedDeckEntry)item.CommandParameter;
            vm.ToDeck(entry);
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //TODO
        }

    }

}

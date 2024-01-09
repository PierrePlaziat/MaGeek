using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using MageekService.Data.Collection.Entities;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class CardInspector : BaseUserControl
    {

        private CardInspectorViewModel vm;

        public CardInspector(CardInspectorViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            if (sendedBy.SelectedItem is DeckCard cardRel) vm.SelectCard(cardRel);
            sendedBy.UnselectAll();
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer s = (ScrollViewer)sender;
            s.ScrollToVerticalOffset(s.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void NewTag_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void NewTag_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

}
using System.Windows;

namespace MaGeek.UI
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //C0.MaxWidth = Math.Min(ResizeGrid.ActualWidth, ActualWidth) - (C2.MinWidth + 5);

        }
    }

}

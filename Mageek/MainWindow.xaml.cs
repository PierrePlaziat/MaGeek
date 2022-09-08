using System.Windows;

namespace MaGeek
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        #region SELECTED CARD DRAWER

        private bool cardInspectorOpened = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cardInspectorOpened) BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(0);
            else BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(255);
            cardInspectorOpened = !cardInspectorOpened;
        }

        #endregion

    }

}

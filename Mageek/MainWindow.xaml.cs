using System.Windows;

namespace MaGeek.UI
{

    public partial class MainWindow : Window
    {

        private bool cardInspectorOpened = true;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }   

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cardInspectorOpened)    BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(0);
            else                        BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(255);
            cardInspectorOpened = !cardInspectorOpened;
        }
    }

}

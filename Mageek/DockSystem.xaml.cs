using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MaGeek
{
    /// <summary>
    /// Logique d'interaction pour DockSystem.xaml
    /// </summary>
    public partial class DockSystem : Window
    {
        public DockSystem()
        {
            DataContext = this;
            InitializeComponent();
        }

        private bool cardInspectorOpened = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cardInspectorOpened) BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(0);
            else BaseGrid.ColumnDefinitions[BaseGrid.ColumnDefinitions.Count - 1].Width = new GridLength(255);
            cardInspectorOpened = !cardInspectorOpened;
        }

    }
}

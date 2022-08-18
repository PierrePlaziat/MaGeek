using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class Options : UserControl
    {

        public Options()
        {
            DataContext = this;
            InitializeComponent();
            SelectCurrentLanguage();
        }

        private void SaveDb_Click(object sender, RoutedEventArgs e) { App.Database.SaveDb(); }
        private void LoadDb_Click(object sender, RoutedEventArgs e) { App.Database.LoadDb(); }
        private void EraseDb_Click(object sender, RoutedEventArgs e) { App.Database.EraseDb(); }

        private void SelectCurrentLanguage()
        {
            foreach (var i in LanguageBox.Items)
            {
                string itemName = ((ComboBoxItem)i).Content as string;
                if (itemName == App.State.GetForeignLanguage()) LanguageBox.SelectedItem = i;
            }
        }
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.State.SetForeignLanguage ( ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content as string );
        }

    }

}

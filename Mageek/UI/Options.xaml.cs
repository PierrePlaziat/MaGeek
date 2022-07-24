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
            SelectCurrentSearchBehaviour();
        }

        private void SaveDb_Click(object sender, RoutedEventArgs e) { App.cardManager.SaveDb(); }
        private void LoadDb_Click(object sender, RoutedEventArgs e) { App.cardManager.LoadDb(); }
        private void EraseDb_Click(object sender, RoutedEventArgs e) { App.cardManager.EraseDb(); }

        private void SelectCurrentLanguage()
        {
            foreach (var i in LanguageBox.Items)
            {
                string itemName = ((ComboBoxItem)i).Content as string;
                if (itemName == App.state.GetForeignLanguage()) LanguageBox.SelectedItem = i;
            }
        }
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.state.SetForeignLanguage ( ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content as string );
        }

        private void SelectCurrentSearchBehaviour()
        {
            /*foreach (var i in SearchBehaviourBox.Items)
            {
                string itemName = ((ComboBoxItem)i).Content as string;
                if (itemName == App.state.GetSearchBehaviour()) SearchBehaviourBox.SelectedItem = i;
            }*/
        }
        private void SearchBehaviour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.state.SetSearchBehaviour(((ComboBoxItem)((ComboBox)sender).SelectedItem).Content as string);
        }

    }

}

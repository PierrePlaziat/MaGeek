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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaGeek.UI
{

    public partial class Options : UserControl
    {


        public Options()
        {
            DataContext = this;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            foreach (var i in LanguageBox.Items)
            {
                string itemName = ((ComboBoxItem)i).Content as string;
                if (itemName == App.state.GetForeignLanguage()) LanguageBox.SelectedItem = i;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.state.SetForeignLanguage ( ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content as string );
        }
    }

}

using PlaziatWpf.Controls.TextBoxHelpers;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MageekDesktopServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        RichTextBoxLogger logger;
        ConsoleAppManager manager;

        public MainWindow(ConsoleAppManager manager)
        {
            this.manager = manager;
            DataContext = this;
            InitializeComponent();
            logger = new RichTextBoxLogger(RTB);
            manager.StandartTextReceived += Manager_StandartTextReceived;
            manager.ErrorTextReceived += Manager_ErrorTextReceived;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Manager_ErrorTextReceived(object sender, string e)
        {
            logger.LogAsync(e, "Red").ConfigureAwait(false);
        }

        private void Manager_StandartTextReceived(object sender, string e)
        {
            string color = "White";
            if (e.Contains("fail: ")) color = "Red";
            if (e.Contains("/!\\")) color = "Red";
            logger.LogAsync(e, color).ConfigureAwait(false);
        }

    }
}
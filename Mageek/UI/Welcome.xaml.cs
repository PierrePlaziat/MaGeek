using MaGeek.AppBusiness;
using MaGeek.UI;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaGeek
{

    public partial class Welcome : TemplatedWindow
    {

        #region Attributes

        string mageekMessage;
        public string MageekMessage {
            get { return mageekMessage; }
            set { mageekMessage = value; OnPropertyChanged(); }
        }

        #region Visibilities

        private Visibility visibility_NormalLaunch = Visibility.Collapsed;
        public Visibility Visibility_NormalLaunch
        {
            get { return visibility_NormalLaunch; }
            set { visibility_NormalLaunch = value; OnPropertyChanged(); }
        }

        private Visibility visibility_NewCards = Visibility.Collapsed;
        public Visibility Visibility_NewCards
        {
            get { return visibility_NewCards; }
            set { visibility_NewCards = value; OnPropertyChanged(); }
        }

        private Visibility visibility_NewUpdate = Visibility.Collapsed;
        public Visibility Visibility_NewUpdate
        {
            get { return visibility_NewUpdate; }
            set { visibility_NewUpdate = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #region CTOR

        public Welcome()
        {
            DataContext = this;
            InitializeComponent();
            Init().ConfigureAwait(false);
        }

        #endregion

        #region Methods

        private async Task Init()
        {
            if (App.Config.SeemToBeFirstLaunch) await UpdateCardDb(true);
            else
            { 
                MageekMessage = "Check updates";
                if (await MageekBulkinator.AreDataOutdated())
                {
                    MageekMessage = "Cards database update available";
                    Visibility_NewCards = Visibility.Visible;
                }
                else if (App.IsUpdateAvailable())
                {
                    MageekMessage = "Software update available";
                    Visibility_NewUpdate = Visibility.Visible;
                }
                else Launch();

            }
               
        }


        private async Task UpdateCardDb(bool isFirst)
        {
            MageekMessage = "Downloading Data";
            await MageekBulkinator.DownloadBulkData();
            MageekMessage = "Importing Cards (~5 min)";
            await MageekBulkinator.ImportAllData(isFirst);
            Launch();
        }

        private void Launch()
        {
            Hide();
            App.LaunchMainWin();
            Close();
        }

        private void Button_UpdateCardDb(object sender, RoutedEventArgs e)
        {
            Visibility_NewCards = Visibility.Collapsed;
            UpdateCardDb(false).ConfigureAwait(false);
        }
        private void Button_UpdateSoftware(object sender, RoutedEventArgs e)
        {
            App.UpdateSoftware();
        }
        private void Button_Ignore(object sender, RoutedEventArgs e)
        {
            Launch();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        #endregion

    }

}

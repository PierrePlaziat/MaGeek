using MaGeek.UI;
using MageekSdk;
using MageekSdk.Data.Collection;
using MageekSdk.Data.Mtg;
using MageekSdk.Tools;
using System;
using System.IO;
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
        
        private Visibility visibility_IsLoading = Visibility.Visible;
        public Visibility Visibility_IsLoading
        {
            get { return visibility_IsLoading; }
            set { visibility_IsLoading = value; OnPropertyChanged(); }
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
            MageekMessage = "Check software update";
            if (await App.IsUpdateAvailable()) await App.UpdateSoftware();
            MageekMessage = "Check database update";
            await InitializeMageekServer();
            MageekMessage = "Launching";
            await Launch();
        }

        private async Task InitializeMageekServer()
        {
            MageekFolders.InitFolders();
            if (!File.Exists(MageekFolders.DB)) CollectionDbManager.CreateDb();

            try
            {

                bool mtgUpdated = false;
                try
                {
                    if (await MtgDbManager.NeedsUpdate())
                    {
                        Logger.Log("Updating...");
                        await MtgDbManager.DatabaseDownload();
                        Logger.Log("Updated!");
                        MtgDbManager.HashSave();
                        mtgUpdated = true;
                    }
                    else
                    {
                        Logger.Log("No Update");
                        mtgUpdated = false;
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                    mtgUpdated = false;
                }

                if (mtgUpdated) await CollectionDbManager.FetchMtg();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private async Task Launch()
        {
            Hide();
            await Task.Delay(310);
            App.LaunchMainWin();
            Close();
        }

        private void Button_UpdateSoftware(object sender, RoutedEventArgs e)
        {
            Visibility_NewUpdate= Visibility.Collapsed;
            Visibility_IsLoading = Visibility.Visible;
            App.UpdateSoftware().ConfigureAwait(false);
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

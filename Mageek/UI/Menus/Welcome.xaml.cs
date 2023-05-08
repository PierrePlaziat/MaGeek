using MaGeek.AppBusiness;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MaGeek
{

    public partial class Welcome : Window, INotifyPropertyChanged
    {

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        string mageekMessage = "test msg";
        public string MageekMessage {
            get { return mageekMessage; }
            set { mageekMessage = value; OnPropertyChanged(); }
        }

        #region Visibilities

        private Visibility visibility_FirstLaunch = Visibility.Collapsed;
        public Visibility Visibility_FirstLaunch
        {
            get { return visibility_FirstLaunch; }
            set { visibility_FirstLaunch = value; OnPropertyChanged(); }
        }
        
        private Visibility visibility_NormalLaunch = Visibility.Collapsed;
        public Visibility Visibility_NormalLaunch
        {
            get { return visibility_NormalLaunch; }
            set { visibility_NormalLaunch = value; OnPropertyChanged(); }
        }

        private Visibility visibility_MtgJsonDownloaded = Visibility.Collapsed;
        public Visibility Visibility_MtgJsonDownloaded
        {
            get { return visibility_MtgJsonDownloaded; }
            set { visibility_MtgJsonDownloaded = value; OnPropertyChanged(); }
        }

        private Visibility visibility_ForeignNamesImported = Visibility.Collapsed;
        public Visibility Visibility_ForeignNamesImported
        {
            get { return visibility_ForeignNamesImported; }
            set { visibility_ForeignNamesImported = value; OnPropertyChanged(); }
        }


        private Visibility visibility_NoNews = Visibility.Collapsed;
        public Visibility Visibility_NoNews
        {
            get { return visibility_NoNews; }
            set { visibility_NoNews = value; OnPropertyChanged(); }
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
            if (App.Config.seemToBeFirstLaunch) Activate_FirstLaunch().ConfigureAwait(false);
            else Activate_NormalLaunch();
        }

        #endregion

        #region Methods

        private async Task Activate_FirstLaunch()
        {
            Visibility_FirstLaunch = Visibility.Visible;
            MageekMessage = "First launch.";
            await MageekBulkinator.Download_MtgJsonSqlite();
            Visibility_MtgJsonDownloaded = Visibility.Visible;
            await MageekBulkinator.Bulk_CardTraductions();
            Visibility_ForeignNamesImported = Visibility.Visible;
        }
        
        private void Activate_NormalLaunch()
        {
            Hide();
            App.LaunchMainWin();
            Close();
            //Visibility_NormalLaunch = Visibility.Visible;
            //using (var DB = App.DB.GetNewContext())
            //{
            //    MageekMessage = "I currently know " + await DB.CardVariants.CountAsync() + " cards.";
            //}
            //if (!DetermineIfNewUpdate()) DetermineIfNewCards();
        }

        //private bool DetermineIfNewUpdate()
        //{
        //    // TODO
        //    return false;
        //}

        //private void DetermineIfNewCards()
        //{
        //    // TODO
        //}

        private void LaunchMassImport(object sender, RoutedEventArgs e)
        {
            Hide();
            bool fun = true;
            App.LaunchMainWin();
            //if (Fun.IsChecked.HasValue) fun = Fun.IsChecked.Value;
            MageekBulkinator.Bulk_Cards(fun).ConfigureAwait(false);
            Close();
        }

        private void DoClose(object sender, RoutedEventArgs e)
        {
            Hide();
            App.LaunchMainWin();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void GoToGithub(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

        #endregion

    }

}

﻿using MaGeek.UI;
using MtgSqliveSdk;
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
            await Mageek.Initialize();
            MageekMessage = "Launching";
            await Launch();
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
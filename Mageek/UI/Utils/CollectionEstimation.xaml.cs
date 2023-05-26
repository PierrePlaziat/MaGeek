using System;
using System.Collections.Generic;
using System.Windows;

namespace MaGeek.UI
{

    public partial class CollectionEstimation : TemplatedWindow
    {

        #region Attributes

        public int TotalGot { get; set; } = 0;
        public int TotalDiffGot { get; set; } = 0;
        public int TotalDiffExist { get; set; } = 0;
        public float AutoEstimation { get; set; } = 0;
        public float FinalEstimation { get; set; } = 0;
        public int MissingCount { get; set; } = 0;

        List<string> missingList = new List<string>();
        public List<string> MissingList
        { 
            get { return missingList; }
            set { missingList = value; OnPropertyChanged(); }
        }

        Visibility isLoading = Visibility.Visible;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region CTOR

        public CollectionEstimation()
        {
            DataContext = this;
            InitializeComponent();
            StartAutoEstimation();
        }

        #endregion

        #region Methods

        private void StartAutoEstimation()
        {
            //
        }

        private void AddManualEstimations(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion

    }

}

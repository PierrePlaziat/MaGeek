using MaGeek.AppBusiness;
using MaGeek.Entities;
using MaGeek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        List<CardVariant> missingList = new List<CardVariant>();
        public List<CardVariant> MissingList
        { 
            get { return missingList; }
            set { missingList = value; OnPropertyChanged(); }
        }

        #region Visibilities

        Visibility isLoading = Visibility.Visible;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        #endregion
        
        #endregion

        #region CTOR

        public CollectionEstimation()
        {
            DataContext = this;
            InitializeComponent();
            DelayLoad().ConfigureAwait(false);
        }

        private async Task DelayLoad()
        {
            await Task.Delay(1);
            await AutoEstimate();
        }
        
        #endregion

        #region Methods

        private async Task AutoEstimate()
        {
            try
            {
                IsLoading = Visibility.Visible;
                await Task.Run(async () => {
                    TotalDiffExist = await MageekStats.GetTotalDiffExist();
                    TotalDiffGot = await MageekStats.GetTotalDiffGot();
                    TotalGot = await MageekStats.GetTotalOwned();
                    var est = await MageekStats.AutoEstimatePrices(); ;
                    AutoEstimation = est.Item1;
                    MissingList = est.Item2;
                    OnPropertyChanged(nameof(TotalDiffExist));
                    OnPropertyChanged(nameof(TotalDiffGot));
                    OnPropertyChanged(nameof(TotalGot));
                    OnPropertyChanged(nameof(AutoEstimation));
                    OnPropertyChanged(nameof(missingList));
                    OnPropertyChanged(nameof(MissingCount));
                });
                IsLoading = Visibility.Collapsed;
            }
            catch (Exception e) { Log.Write(e); }
        }

        private void AddManualEstimations(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion

    }

}

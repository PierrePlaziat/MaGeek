using MaGeek.Framework.Utils;
using MageekSdk.Tools;
using MtgSqliveSdk;
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
        public Decimal AutoEstimation { get; set; } = 0;
        public float FinalEstimation { get; set; } = 0;
        public int MissingCount { get; set; } = 0;

        List<string> missingList = new();
        public List<string> MissingList
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
                    TotalDiffExist = await Mageek.GetTotal_ExistingArchetypes();
                    TotalDiffGot = await Mageek.GetTotal_CollectedArchetype();
                    TotalGot = await Mageek.GetTotal_CollectedDiff();
                    var est = await Mageek.AutoEstimatePrices(App.Config.Settings[Setting.ForeignLanguage]);
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
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private void AddManualEstimations(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion

    }

}

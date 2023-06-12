using MaGeek.AppBusiness;
using MaGeek.Entities;
using MaGeek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                    AutoEstimation = await AutoEstimatePrices(); ;
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

        private async Task<float> AutoEstimatePrices()
        {

            float total = 0;
            try
            {
                    using var DB = App.DB.NewContext;
                    foreach (CardVariant card in DB.CardVariants)
                    {
                        var gotLine = await DB.User_GotCards.Where(x => x.CardVariantId == card.Id).FirstOrDefaultAsync();
                        if (gotLine!=null)
                        {
                            if (card.ValueEur != null)
                            {
                                total += gotLine.got * float.Parse(card.ValueEur);
                            }
                            else
                            {
                                missingList.Add(card);
                            }

                        }
                    }
            }
            catch (Exception e) { Log.Write(e, "AutoEstimatePrices"); }
            return total;
        }

        private void AddManualEstimations(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion

    }

}

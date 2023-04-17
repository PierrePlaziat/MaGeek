using MaGeek.AppData.Entities;
using MaGeek.UI.Windows.ImportExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MaGeek.UI
{

    public partial class DeckStats : TemplatedUserControl
    {

        #region Attributes

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsActive));
                AsyncReload();
            }
        }

        public int CreatureCount { get; private set; } 
        public int InstantCount { get; private set; }  
        public int SorceryCount { get; private set; }   
        public int EnchantmentCount { get; private set; }  
        public int ArtifactCount { get; private set; }  
        public int BasicLandCount { get; private set; }  
        public int SpecialLandCount { get; private set; } 
        public int OtherCount { get; private set; }  
        public int DevotionB { get; private set; }    
        public int DevotionW { get; private set; }  
        public int DevotionU { get; private set; } 
        public int DevotionG { get; private set; }    
        public int DevotionR { get; private set; }   
        public string StandardOk { get; private set; }      
        public string CommanderOk { get; private set; }   
        public int OwnedRatio { get; private set; } 

        #region Visibilities

        private Visibility isLoading = Visibility.Collapsed;
        public Visibility IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        public Visibility IsActive {
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #endregion

        #region CTOR

        public DeckStats()
        {
            DataContext = this;
            InitializeComponent();
            ConfigureEvents();
        }

        #endregion

        #region Events

        private void ConfigureEvents()
        {
            App.Events.SelectDeckEvent += HandleDeckSelected;
            App.Events.UpdateDeckEvent += HandleDeckModified;
        }

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
        }

        void HandleDeckModified()
        {
            AsyncReload();
        }

        #endregion

        #region Async Reload

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            IsLoading = Visibility.Visible;
            CreatureCount = await App.Biz.Utils.count_Creature(currentDeck);
            InstantCount = await App.Biz.Utils.count_Instant(currentDeck);
            SorceryCount = await App.Biz.Utils.count_Sorcery(currentDeck);
            EnchantmentCount = await App.Biz.Utils.count_Enchantment(currentDeck); 
            ArtifactCount = await App.Biz.Utils.count_Artifact(currentDeck);
            BasicLandCount = await App.Biz.Utils.count_BasicLand(currentDeck);
            SpecialLandCount = await App.Biz.Utils.count_SpecialLand(currentDeck);
            OtherCount = await App.Biz.Utils.count_other(currentDeck);
            DevotionB = await App.Biz.Utils.DevotionB(currentDeck);
            DevotionW = await App.Biz.Utils.DevotionW(currentDeck);
            DevotionU = await App.Biz.Utils.DevotionU(currentDeck);
            DevotionG = await App.Biz.Utils.DevotionG(currentDeck);
            DevotionR = await App.Biz.Utils.DevotionR(currentDeck);
            StandardOk = await App.Biz.Utils.Validity_Standard(currentDeck) ? "YES" : "NO";
            CommanderOk = await App.Biz.Utils.Validity_Commander(currentDeck) ? "YES" : "NO";
            OwnedRatio = await App.Biz.Utils.OwnedRatio(currentDeck);
            await Task.Run(() => {
                OnPropertyChanged(nameof(CreatureCount));
                OnPropertyChanged(nameof(InstantCount));
                OnPropertyChanged(nameof(SorceryCount));
                OnPropertyChanged(nameof(EnchantmentCount));
                OnPropertyChanged(nameof(ArtifactCount));
                OnPropertyChanged(nameof(BasicLandCount));
                OnPropertyChanged(nameof(SpecialLandCount));
                OnPropertyChanged(nameof(OtherCount));
                OnPropertyChanged(nameof(StandardOk));
                OnPropertyChanged(nameof(CommanderOk));
                OnPropertyChanged(nameof(DevotionB));
                OnPropertyChanged(nameof(DevotionW));
                OnPropertyChanged(nameof(DevotionU));
                OnPropertyChanged(nameof(DevotionG));
                OnPropertyChanged(nameof(DevotionR));
                OnPropertyChanged(nameof(OwnedRatio));
            });
            int[] manacurve = await App.Biz.Utils.GetManaCurve(currentDeck);
            DrawNewHand();
            DrawManacurve(manacurve);
            await Task.Run(() =>
            {
                IsLoading = Visibility.Collapsed;
            });
        }

        #endregion

        #region Methods

        private async void ListMissing(object sender, RoutedEventArgs e)
        {
            string missList = await App.Biz.Utils.ListMissingCards(currentDeck);
            if (!string.IsNullOrEmpty(missList))
            {
                var window = new DeckListExporter(missList);
                window.Show();
            }
        }

        #region ManaCurve

        private PointCollection curvePoints;
        public PointCollection CurvePoints
        {
            get { return curvePoints; }
            set { curvePoints = value; OnPropertyChanged(); }
        }

        private Point curveStart;
        public Point CurveStart
        {
            get { return curveStart; }
            set { curveStart = value; OnPropertyChanged(); }
        }

        private void DrawManacurve(int[] manaCurve)
        {
            CurveStart = new Point(0,0);
            CurvePoints = null;
            PointCollection Points = new PointCollection();
            if (CurrentDeck != null)
            { 
                int manaMax = manaCurve.ToList().Max();
                float factor;
                if (manaMax == 0) factor = 0;
                else factor = 100 / manaMax;
                CurveStart = new Point(0, 100 - factor * manaCurve[0]);
                Points.Add(CurveStart);
                for (int i=1;i<manaCurve.Length;i++) Points.Add(new Point(50*i, 100 - factor * manaCurve[i]));
                
            }
            CurvePoints = Points;
        }

        #endregion

        #region Hand

        private List<MagicCardVariant> hand;
        public List<MagicCardVariant> Hand
        {
            get { return hand; }
            set { hand = value; OnPropertyChanged(); }
        }

        List<int> alreadyDrawed;

        Random random = new Random();

        private void DrawNewHand()
        {
            Hand = new List<MagicCardVariant>();
            alreadyDrawed = new List<int>();
            HandPanel.Children.Clear();
            for (int i = 0; i < 7; i++)
            {
                DrawNewCard();
            }
        }

        private void DrawNewCard()
        {
            MagicCardVariant newCard = DoDraw();
            if (newCard != null)
            {
                HandPanel.Children.Add(
                    new CardIllustration(newCard)
                    {
                        Width = 50,
                        Height = 50,
                    }
                );
            }
        }

        private MagicCardVariant DoDraw()
        {
            if (CurrentDeck == null) return null;
            if (currentDeck.CardRelations.Count <= alreadyDrawed.Count) return null;
            int rgn;
            do { rgn = random.Next(CurrentDeck.CardRelations.Count); }
            while (alreadyDrawed.Contains(rgn));
            return currentDeck.CardRelations[rgn].Card;
        }

        #region UI Link

        private void DrawNewHandButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewHand();
        }

        private void DrawCardButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewCard();
        }

        #endregion

        #endregion

        #endregion
    }

}

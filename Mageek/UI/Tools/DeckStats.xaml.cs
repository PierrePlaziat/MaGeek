using MaGeek.Data.Entities;
using MaGeek.UI.Windows.ImportExport;
using System;
using System.Collections.Generic;
using System.Linq;
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
                OnPropertyChanged("CurrentDeck");
                OnPropertyChanged("Visible");
            }
        }

        public Visibility Visible { get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; } }

        public int CreatureCount    { get { return App.Biz.Utils.count_Creature(currentDeck); } }
        public int InstantCount     { get { return App.Biz.Utils.count_Instant(currentDeck); } }
        public int SorceryCount     { get { return App.Biz.Utils.count_Sorcery(currentDeck); } }
        public int EnchantmentCount { get { return App.Biz.Utils.count_Enchantment(currentDeck); } }
        public int ArtifactCount    { get { return App.Biz.Utils.count_Artifact(currentDeck); } }
        public int BasicLandCount   { get { return App.Biz.Utils.count_BasicLand(currentDeck); } }
        public int SpecialLandCount { get { return App.Biz.Utils.count_SpecialLand(currentDeck); } }
        public int OtherCount       { get { return App.Biz.Utils.count_other(currentDeck); } }
        public int DevotionB        { get { return App.Biz.Utils.DevotionB(currentDeck); } }
        public int DevotionW        { get { return App.Biz.Utils.DevotionW(currentDeck); } }
        public int DevotionU        { get { return App.Biz.Utils.DevotionU(currentDeck); } }
        public int DevotionG        { get { return App.Biz.Utils.DevotionG(currentDeck); } }
        public int DevotionR        { get { return App.Biz.Utils.DevotionR(currentDeck); } }
        public string StandardOk    { get { return App.Biz.Utils.validity_Standard(currentDeck) ? "YES" : "NO"; } }
        public string CommanderOk   { get { return App.Biz.Utils.validity_Commander(currentDeck) ? "YES" : "NO"; } }
        public int OwnedRatio       { get { return App.Biz.Utils.OwnedRatio(currentDeck); } }

        #endregion

        #region CTOR

        public DeckStats()
        {
            InitializeComponent();
            DataContext = this;
            App.Events.SelectDeckEvent += HandleDeckSelected;
            App.Events.UpdateDeckEvent += HandleDeckModified;
        }

        void HandleDeckModified()
        {
            FullRefresh();
        }

        void HandleDeckSelected(MagicDeck deck)
        {
            CurrentDeck = deck;
            FullRefresh();
        }

        #endregion

        #region Methods

        private void FullRefresh()
        {
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
            DrawManacurve(App.Biz.Utils.GetManaCurve(currentDeck));
            DrawNewHand();
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

        private void ListMissing(object sender, RoutedEventArgs e)
        {
            string missList = App.Biz.Utils.ListMissingCards(currentDeck);
            if (!string.IsNullOrEmpty(missList))
            {
                var window = new DeckListExporter(missList);
                window.Show();
            }
        }
    }

}

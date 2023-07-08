using MageekSdk.Collection.Entities;
using MtgSqliveSdk;
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

        private Deck currentDeck;
        public Deck CurrentDeck
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

        void HandleDeckSelected(int deckId)
        {
            CurrentDeck = Mageek.GetDeck(deckId).Result;
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
            if (CurrentDeck == null) return;
            IsLoading = Visibility.Visible;
            await Task.Run(async () => {
                CreatureCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Creature");
                InstantCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Instant");
                SorceryCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Sorcery"); 
                EnchantmentCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Enchantment");
                ArtifactCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Artifact"); 
                BasicLandCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Planeswalker"); 
                BasicLandCount = await Mageek.Count_Typed(CurrentDeck.DeckId, "Land"); 
                DevotionB = await Mageek.Devotion(currentDeck.DeckId, 'B');
                DevotionW = await Mageek.Devotion(currentDeck.DeckId, 'W'); 
                DevotionU = await Mageek.Devotion(currentDeck.DeckId, 'U'); 
                DevotionG = await Mageek.Devotion(currentDeck.DeckId, 'G'); 
                DevotionR = await Mageek.Devotion(currentDeck.DeckId, 'R'); 
                StandardOk = await Mageek.DeckValidity(currentDeck, "Standard");
                CommanderOk = await Mageek.DeckValidity(currentDeck, "Commander");
                OwnedRatio = await Mageek.OwnedRatio(currentDeck.DeckId);
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
            int[] manacurve = await Mageek.GetManaCurve(CurrentDeck.DeckId);
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
            string missList = await Mageek.ListMissingCards(CurrentDeck.DeckId);
            if (!string.IsNullOrEmpty(missList))
            {
                var window = new TxtImporter(missList);
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

        private List<string> hand;
        public List<string> Hand
        {
            get { return hand; }
            set { hand = value; OnPropertyChanged(); }
        }

        List<int> alreadyDrawed;

        Random random = new Random();

        private void DrawNewHand()
        {
            Hand = new List<string>();
            alreadyDrawed = new List<int>();
            HandPanel.Children.Clear();
            for (int i = 0; i < 7; i++)
            {
                DrawNewCard();
            }
        }

        private async void DrawNewCard()
        {
            string newCard = await DoDraw();
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

        private async Task<string> DoDraw()
        {
            if (CurrentDeck == null) return null;
            if (currentDeck.CardCount <= alreadyDrawed.Count) return null;
            int rgn;
            do { rgn = random.Next(CurrentDeck.CardCount); }
            while (alreadyDrawed.Contains(rgn));
            //TODO
            await Task.Delay(0);
            return ""; 
            //return await Mageek.GetDeckContent(currentDeck.DeckId)[rgn].;
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

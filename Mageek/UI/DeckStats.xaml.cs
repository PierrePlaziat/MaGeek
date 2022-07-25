using MaGeek.Data.Entities;
using MaGeek.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaGeek.UI
{

    public partial class DeckStats : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        private MagicDeck currentDeck;
        public MagicDeck CurrentDeck
        {
            get { return currentDeck; }
            set
            {
                currentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged("Visible");
            }
        }

        private PointCollection manacurve;
        public PointCollection Manacurve
        {
            get { return manacurve; }
            set { manacurve = value; OnPropertyChanged(); }
        }

        private Point manaCurveStart;
        public Point ManaCurveStart
        {
            get { return manaCurveStart; }
            set { manaCurveStart = value; OnPropertyChanged(); }
        }

        #region Validities

        private string standardOk;
        public string StandardOk
        {
            get { return standardOk; }
            set
            {
                standardOk = value;
                OnPropertyChanged();
            }
        }
        private void SetStandardOk()
        {
            bool ok = true;
            ok = ok && CurrentDeck.CardCount >= 60;
            ok = ok && CheckRelationQuantityLimit(4);
            StandardOk = ok ? "YES" : "NO";
        }

        private string commanderOk;
        public string CommanderOk
        {
            get { return commanderOk; }
            set
            {
                commanderOk = value;
                OnPropertyChanged();
            }
        }
        private void SetCommanderOk()
        {
            bool ok = true;
            ok = ok && CurrentDeck.CardCount == 100;
            ok = ok && CheckRelationQuantityLimit(1);
            ok = ok && CurrentDeck.CardRelations.Where(x => x.RelationType > 0).Any();
            CommanderOk = ok ? "YES" : "NO";
        }

        #endregion

        #region Types


        private int creaturesNb;
        public int CreaturesNb
        {
            get { return creaturesNb; }
            set
            {
                creaturesNb = value;
                OnPropertyChanged();
            }
        }
        private void SetCreaturesNb()
        {
            int count = 0;
            foreach (var v in CurrentDeck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("creature")))
            {
                count += v.Quantity;
            }
            CreaturesNb = count;
        }

        private int spellsNb;
        public int SpellsNb
        {
            get { return spellsNb; }
            set
            {
                spellsNb = value;
                OnPropertyChanged();
            }
        }

        private void SetSpellsNb()
        {
            int count = 0;
            foreach (var v in CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToLower().Contains("creature")&& !x.Card.Card.Type.ToLower().Contains("land")))
            {
                count += v.Quantity;
            }
            SpellsNb = count;
        }


        private int landsNb;
        public int LandsNb
        {
            get { return landsNb; }
            set
            {
                landsNb = value;
                OnPropertyChanged();
            }
        }
        private void SetLandsNb()
        {
            int count = 0;
            foreach (var v in CurrentDeck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("land")))
            {
                count += v.Quantity;
            }
            LandsNb = count;
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

        #endregion

        public Visibility Visible
        {
            get { return currentDeck == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region CTOR

        public DeckStats()
        {
            InitializeComponent();
            DataContext = this;
            App.state.RaiseSelectDeck += HandleDeckSelected;
            App.state.RaiseDeckModif += HandleDeckModified;
        }

        void HandleDeckModified(object sender, DeckModifEventArgs e)
        {
            FullRefresh();
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            CurrentDeck = e.Deck;
            FullRefresh();
        }

        #endregion

        #region Methods

        private void FullRefresh()
        {
            CurrentDeck = App.state.SelectedDeck;
            DrawManacurve();
            SetCreaturesNb();
            SetSpellsNb();
            SetLandsNb();
            SetStandardOk();
            SetCommanderOk();
            NewHand();
        }

        private void DrawManacurve()
        {
            int mana0 = CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToLower().Contains("land") && x.Card.Card.Cmc == 0).Count();
            int mana1 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 1).Count();
            int mana2 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 2).Count();
            int mana3 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 3).Count();
            int mana4 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 4).Count();
            int mana5 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 5).Count();
            int mana6 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 6).Count();
            int mana7 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 7).Count();
            int mana8 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 8).Count();
            int mana9 = CurrentDeck.CardRelations.Where(x => x.Card.Card.Cmc == 9).Count();
            int manaMax = Math.Max(mana0, mana1);
            manaMax = Math.Max(manaMax, mana2);
            manaMax = Math.Max(manaMax, mana3);
            manaMax = Math.Max(manaMax, mana4);
            manaMax = Math.Max(manaMax, mana5);
            manaMax = Math.Max(manaMax, mana6);
            manaMax = Math.Max(manaMax, mana7);
            manaMax = Math.Max(manaMax, mana8);
            manaMax = Math.Max(manaMax, mana9);
            float factor;
            if (manaMax == 0) factor = 0; 
            else factor = 100/manaMax;
            ManaCurveStart = new Point(0, 100 - factor * mana0);
            PointCollection Points = new PointCollection();
            Points.Add(ManaCurveStart);
            Points.Add(new Point(50,  100 - factor * mana1));
            Points.Add(new Point(100, 100 - factor * mana2));
            Points.Add(new Point(150, 100 - factor * mana3));
            Points.Add(new Point(200, 100 - factor * mana4));
            Points.Add(new Point(250, 100 - factor * mana5));
            Points.Add(new Point(300, 100 - factor * mana6));
            Points.Add(new Point(350, 100 - factor * mana7));
            Points.Add(new Point(400, 100 - factor * mana8));
            Points.Add(new Point(450, 100 - factor * mana9));
            Manacurve = Points  ;
        }

        private bool CheckRelationQuantityLimit(int limit)
        {
            bool ok = true;
            foreach (var v in CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToString().ToLower().Contains("land")))
            {
                if (v.Quantity > limit) ok = false;
            }
            return ok;
        }

        #region Hand

        private void NewHand()
        {
            Hand = new List<MagicCardVariant>();
            alreadyDrawed = new List<int>();
            HandPanel.Children.Clear();
            for (int i = 0; i < 7; i++) HandPanel.Children.Add(
                new CardIllustration(Draw())
                {
                    Width = 50,
                    Height = 50,
                }
            );
        }

        private MagicCardVariant Draw()
        {
            int rgn;
            do { rgn = random.Next(CurrentDeck.CardRelations.Count); }
            while (alreadyDrawed.Contains(rgn));
            return currentDeck.CardRelations[rgn].Card;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewHand();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HandPanel.Children.Add(
                   new CardIllustration(Draw())
                   {
                       Width = 50,
                       Height = 50,
                   }
            );
        }

        #endregion

        #endregion

    }

}

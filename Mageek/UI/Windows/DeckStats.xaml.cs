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
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;

namespace MaGeek.UI
{

    public partial class DeckStats : UserControl, INotifyPropertyChanged, IXmlSerializable
    {

        public XmlSchema GetSchema()
        {
            return (null);
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
        }

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
            App.State.SelectDeckEvent += HandleDeckSelected;
            App.State.UpdateDeckEvent += HandleDeckModified;
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
            // = App.State.SelectedDeck;
            DrawManacurve();
            SetCreaturesNb();
            SetSpellsNb();
            SetLandsNb();
            SetStandardOk();
            SetCommanderOk();
            DrawNewHand();
        }

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
            if (CurrentDeck == null) return;
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
            if (CurrentDeck == null) return;
            int count = 0;
            foreach (var v in CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToLower().Contains("creature") && !x.Card.Card.Type.ToLower().Contains("land")))
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
            if (CurrentDeck == null) return;
            int count = 0;
            foreach (var v in CurrentDeck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("land")))
            {
                count += v.Quantity;
            }
            LandsNb = count;
        }

        #endregion

        #region ManaCurve

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

        private void DrawManacurve()
        {
            if (CurrentDeck == null) return;
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
            else factor = 100 / manaMax;
            ManaCurveStart = new Point(0, 100 - factor * mana0);
            PointCollection Points = new PointCollection();
            Points.Add(ManaCurveStart);
            Points.Add(new Point(50, 100 - factor * mana1));
            Points.Add(new Point(100, 100 - factor * mana2));
            Points.Add(new Point(150, 100 - factor * mana3));
            Points.Add(new Point(200, 100 - factor * mana4));
            Points.Add(new Point(250, 100 - factor * mana5));
            Points.Add(new Point(300, 100 - factor * mana6));
            Points.Add(new Point(350, 100 - factor * mana7));
            Points.Add(new Point(400, 100 - factor * mana8));
            Points.Add(new Point(450, 100 - factor * mana9));
            Manacurve = Points;
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
            if (CurrentDeck == null) return;
            bool ok = true;
            ok = ok && CurrentDeck.CardCount >= 60;
            ok = ok && HasMaxCardOccurence(4);
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
            if (CurrentDeck == null) return;
            bool ok = true;
            ok = ok && CurrentDeck.CardCount == 100;
            ok = ok && HasMaxCardOccurence(1);
            ok = ok && CurrentDeck.CardRelations.Where(x => x.RelationType == 1).Any();
            CommanderOk = ok ? "YES" : "NO";
        }

        private bool HasMaxCardOccurence(int limit)
        {
            if (CurrentDeck == null) return false;
            bool ok = true;
            foreach (var v in CurrentDeck.CardRelations.Where(x => !x.Card.Card.Type.ToString().ToLower().Contains("land")))
            {
                if (v.Quantity > limit) ok = false;
            }
            return ok;
        }

        #endregion

        #endregion

    }

}

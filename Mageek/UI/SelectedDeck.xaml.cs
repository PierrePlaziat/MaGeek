using MaGeek.Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaGeek.UI
{

    public partial class SelectedDeck : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Attributes

        public MagicDeck CurrentDeck { 
            get { 
                return App.state.CurrentDeck; 
            } 
            set { 
                App.state.CurrentDeck = value;
                OnPropertyChanged();
                OnPropertyChanged("DevotionB");
                OnPropertyChanged("DevotionB");
                OnPropertyChanged("DevotionW");
                OnPropertyChanged("DevotionU");
                OnPropertyChanged("DevotionG");
                OnPropertyChanged("DevotionR");
                UpdateUGrid();
            } 
        }

        public ObservableCollection<MagicCard> Cards
        {
            get {
                if (App.state.CurrentDeck == null) return null;
                return App.state.CurrentDeck.Cards as ObservableCollection<MagicCard>; 
            }
        }

        #region Devotion

        #endregion

        #endregion

        #region CTOR

        public SelectedDeck()
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        private void UpdateUGrid()
        {

            /*UGrid.Children.Clear();
            foreach(var card in App.state.CurrentDeck.Cards)
            {
                var v = new StackPanel()
                {
                    Background = Brushes.Black,
                    Width= 125,
                    Height= 174,
                };
                var i = new Image()
                {
                    //Source = new Uri(card.variants[0].ImageUrl, UriKind.Absolute);
                };
                v.Children.Add(i);
                UGrid.Children.Add(v);
            
            }*/

        }

    }

}

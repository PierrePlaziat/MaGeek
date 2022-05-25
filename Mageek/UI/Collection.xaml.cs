using MaGeek.Data;
using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class Collection : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        MTG_API mtgApi = new MTG_API();

        private bool isSearching = false;
        public bool IsSearching { get { return isSearching; } set { isSearching = value; OnPropertyChanged(); OnPropertyChanged("IsNotSearching"); } }
        public bool IsNotSearching { get { return !isSearching; } }

        public delegate void CustomEventHandler(object sender, SelectCardEventArgs args);
        public event CustomEventHandler RaiseSelectCard;

        public ObservableCollection<MagicCard> CardsBind { get { return App.appContext.cardsBind; } }

        public Collection()
        { 
            DataContext = this;
            InitializeComponent();
        }
        private async void DoSearch()
        {
            IsSearching = true;
            var NewCardList = new List<MagicCard>();
            var cs = await mtgApi.GetAllCardsByName_Async(CurrentSearch.Text);
            foreach (var iCard in cs)
            {
                if (!NewCardList.Where(x => x.Name_VO == iCard.Name).Any())
                {
                    var card = new MagicCard(iCard);
                    NewCardList.Add(card);
                }
                NewCardList.Where(x => x.Name_VO == iCard.Name).FirstOrDefault().variants.Add(new MagicCardVariant(iCard) { });
                // TODO check french name
            }
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var card in NewCardList)
                {
                    App.SaveCard(card);
                }
            });
            IsSearching = false;
        }

        private void CardList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MagicCard card = CardList.SelectedItem as MagicCard;
            if (card != null)
            {
                OnRaiseCustomEvent(new SelectCardEventArgs(card));
            }
        }

        protected virtual void OnRaiseCustomEvent(SelectCardEventArgs e)
        {
            CustomEventHandler raiseEvent = RaiseSelectCard;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoSearch();
        }
    }

}

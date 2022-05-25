using MaGeek.Data;
using MaGeek.Data.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class Importer : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion


        MTG_API api = new MTG_API();

        public Importer()
        {
            InitializeComponent();
            LoadPanel.Visibility = Visibility.Collapsed;
        }

        private async void StartImport(object sender, RoutedEventArgs e)
        {
            bool asDeck = AsDeck.IsChecked.Value; 
            bool asGot = AsGot.IsChecked.Value;
            if (asDeck && string.IsNullOrEmpty(DeckTitle.Text))
            {
                MessageBoxHelper.ShowMsg("Please enter a title to the deck.");
                return;
            }
            if (asDeck && App.appContext.decks.Where(x=>x.Name == DeckTitle.Text).Any())
            {
                MessageBoxHelper.ShowMsg("A deck with that name already exists.");
                return;
            }
            LoadPanel.Visibility = Visibility.Visible;
            if (asDeck) await ImportAsDeck(asGot);
            else await ImportIntoCollection(asGot);
            LoadPanel.Visibility = Visibility.Collapsed;
        }

        private async Task ImportAsDeck(bool asGot)
        {
            var deck = new MagicDeck();
            deck.Name = DeckTitle.Text;
            deck.Cards = new List<MagicCard>();
            List<string> importLines = ImportTxt.Text.Split(Environment.NewLine).ToList();
            LoadBar.Maximum = importLines.Count;
            LoadBar.Minimum = 0;
            LoadBar.Value=0;
            foreach (string line in importLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string cardname = line.Substring(line.IndexOf(' ') + 1);
                    if (!App.appContext.cards.Where(x => x.Name_VO == cardname).Any())
                    {
                        await DoSearch(cardname);
                    }
                    var card = App.appContext.cards.Where(x => x.Name_VO == cardname).FirstOrDefault();
                    if (card != null) deck.Cards.Add(card);
                    LoadBar.Value++;
                }
            }
            App.appContext.decks.Add(deck);
            App.appContext.SaveChanges();
        }

        private async Task ImportIntoCollection(bool asGot)
        {
            List<string> importLines = ImportTxt.Text.Split(Environment.NewLine).ToList();
            foreach (string line in importLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string cardname = line.Substring(line.IndexOf(' ') + 1);
                    if (!App.appContext.cards.Where(x => x.Name_VO == cardname).Any())
                    {
                        await DoSearch(cardname);
                    }
                }
            }
            App.appContext.SaveChanges();
        }

        private async Task DoSearch(string cardname)
        {
            var NewCardList = new List<MagicCard>();
            var cs = await api.GetAllCardsByName_Async(cardname);
            foreach (var iCard in cs)
            {
                if(iCard != null && iCard.Name == cardname)
                {
                    if (!NewCardList.Where(x => x.Name_VO == iCard.Name).Any())
                    {
                        var card = new MagicCard(iCard);
                        NewCardList.Add(card);
                    }
                    NewCardList.Where(x => x.Name_VO == iCard.Name).FirstOrDefault().variants.Add(new MagicCardVariant(iCard) { });
                }
            }
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var card in NewCardList)
                {
                    App.SaveCard(card);
                }
            });
        }

    }

}

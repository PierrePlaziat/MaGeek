using MaGeek.CommonWpf;
using MaGeek.Data.Entities;
using MaGeek.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class ImportExport : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region CTOR

        public ImportExport()
        {
            InitializeComponent();
            LoadPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Methods

        #region UI Link

        private void StartExport(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private async void LaunchImportation(object sender, RoutedEventArgs e)
        {
            bool asDeck = AsDeck.IsChecked.Value;
            bool asGot = AsGot.IsChecked.Value;
            if (asDeck && string.IsNullOrEmpty(DeckTitle.Text))
            {
                MessageBoxHelper.ShowMsg("Please enter a title to the deck.");
                return;
            }
            LoadPanel.Visibility = Visibility.Visible;
            await ImportCards(asGot,asDeck);
            LoadPanel.Visibility = Visibility.Collapsed;
        }
        
        private void ResetLoadBar(int max)
        {
            LoadBar.Maximum = max;
            LoadBar.Minimum = 0;
            LoadBar.Value = 0;
        }

        #endregion

        private async Task ImportCards(bool asObtained,bool asDeck)
        {

            // Prepare

            List<string> importLines = RichTextBoxHelper
                .GetContent(ImportTxt)
                .Split(Environment.NewLine)
                .ToList();

            ResetLoadBar(importLines.Count);

            ImportOutput.Text = "";
            string errors = "";

            var deck = new MagicDeck()
            {
                Title = DeckTitle.Text,
                CardRelations = new ObservableCollection<CardDeckRelation>()
            };

            // Process

            foreach (string line in importLines)
            {
                if (!string.IsNullOrEmpty(line))
                {

                    // Parse

                    ImportOutput.Text += "Importing : "+ line+"\n";

                    string cardname = line[(line.IndexOf(' ') + 1)..];
                    cardname = cardname.Split(" // ")[0];
                    int cardQuantity = int.Parse(line.Split(" ")[0]);

                    // Search

                    var card = App.database.cards.Where(x => x.CardId == cardname || (x.CardId.Contains(cardname) && x.CardId.Contains(" // "))).FirstOrDefault();
                    if (card == null)
                    {
                        await App.cardManager.MtgApi.SearchCardsOnline(cardname, true);
                        card = App.database.cards.Where(x => x.CardId == cardname || (x.CardId.Contains(cardname) && x.CardId.Contains(" // "))).FirstOrDefault();
                    }

                    // Add

                    if (card != null)
                    {
                        if (asDeck)
                        {
                            App.cardManager.AddCardToDeck(card,deck, cardQuantity);
                        }
                        if (asObtained) card.CollectedQuantity += cardQuantity;
                    }
                    else
                    {
                        errors += cardname + " not found\n";
                    }

                    LoadBar.Value++;

                }
            }

            // Finalize

            if (asDeck) App.database.decks.Add(deck);
            App.database.SaveChanges();
            ImportTxt.Document.Blocks.Clear();
            ImportTxt.AppendText(errors);

        }

        #endregion

    }

}

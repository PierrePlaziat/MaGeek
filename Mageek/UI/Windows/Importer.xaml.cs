using MaGeek.CommonWpf;
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

    public partial class Importer : Window, INotifyPropertyChanged
    {

        public string currentImportState = "";
        public string CurrentImportState {
            get { return currentImportState; }
            set { currentImportState = value; OnPropertyChanged(); }
        }

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region CTOR

        public Importer()
        {
            InitializeComponent();
            LoadPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Methods

        #region UI Link

        private async void LaunchImportation(object sender, RoutedEventArgs e)
        {
            bool asDeck = MakeNewDeckCheckBox.IsChecked.Value;
            bool asOwned = AsOwnedCheckBox.IsChecked.Value;
            if (asDeck && string.IsNullOrEmpty(DeckTitle.Text))
            {
                MessageBoxHelper.ShowMsg("Please enter a title to the deck.");
                return;
            }
            LoadPanel.Visibility = Visibility.Visible;
            await ImportCards(asOwned, asDeck);
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
            CurrentImportState = "Init";

            string errors = "";
            ImportOutput.Document.Blocks.Clear();
            var importDeck = new MagicDeck(DeckTitle.Text);
            List<string> importLines = RichTextBoxHelper.GetContent(ImportTxt).Split(Environment.NewLine).ToList();
            ResetLoadBar(importLines.Count);
            
            // Importation
            
            foreach (string line in importLines)
            {
                if (IsLineValid(line))
                {

                    CurrentImportState = "Parsing";


                    ImportOutput.AppendText("Importing : "+ line);
                    ImportOutput.AppendText("\u2028"); // Linebreak, not paragraph break
                    ImportOutput.ScrollToEnd();

                    string cardname = line[(line.IndexOf(' ') + 1)..];
                    cardname = cardname.Split(" // ")[0];
                    int cardQuantity = int.Parse(line.Split(" ")[0]);

                    CurrentImportState = "Searching "+ cardname;

                    var card = App.Database.cards.Where(x => x.CardId == cardname || (x.CardId.Contains(cardname) && x.CardId.Contains(" // "))).FirstOrDefault();
                    if (card == null)
                    {
                        await App.CardManager.Api.SearchCards(cardname, true);
                        card = App.Database.cards.Where(x => x.CardId == cardname || (x.CardId.Contains(cardname) && x.CardId.Contains(" // "))).FirstOrDefault();
                    }

                    CurrentImportState = "Adding"+ cardname;

                    if (card != null)
                    {
                        if (asDeck)
                        {
                            App.CardManager.AddCardToDeck(card.Variants[0],importDeck, cardQuantity);
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

            if (asDeck) App.Database.decks.Add(importDeck);
            App.Database.SaveChanges();
            ImportTxt.Document.Blocks.Clear();
            ImportTxt.AppendText(errors);

        }

        private bool IsLineValid(string line)
        {
            bool isValid = true;
            isValid = isValid && !string.IsNullOrEmpty(line);
            return isValid; 
        }

        #endregion

    }

}

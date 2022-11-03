using MaGeek.CommonWpf;
using System;
using System.Windows;

namespace MaGeek.UI
{

    public partial class TxtImporter : Window
    {

        #region CTOR

        public TxtImporter()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void LaunchImportation(object sender, RoutedEventArgs e)
        {
            bool asOwned = AsOwnedCheckBox.IsChecked.Value;
            string title = string.IsNullOrEmpty(DeckTitle.Text) ? DateTime.Now.ToString() : DeckTitle.Text;
            App.MaGeek.Importer.AddImportToQueue(
                new Data.PendingImport
                {
                    mode = Data.ImportMode.List,
                    content = RichTextBoxHelper.GetContent(ImportTxt),
                    title = title,
                    asOwned = asOwned
                }
            );
            ImportTxt.Document.Blocks.Clear();
            ImportTxt.AppendText("Added to import list!");
        }

        #endregion

    }

}

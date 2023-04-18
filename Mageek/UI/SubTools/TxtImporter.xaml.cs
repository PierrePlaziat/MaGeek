using MaGeek.AppBusiness;
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
            App.Biz.Importer.AddImportToQueue(
                new PendingImport
                {
                    Mode = ImportMode.List,
                    Content = RichTextBoxHelper.GetContent(ImportTxt),
                    Title = title,
                    AsOwned = asOwned
                }
            );
            ImportTxt.Document.Blocks.Clear();
            ImportTxt.AppendText("Added to import list!");
        }

        #endregion

    }

}

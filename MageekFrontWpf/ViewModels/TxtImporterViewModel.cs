using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;

namespace MageekFrontWpf.ViewModels
{
    public class TxtImporterViewModel : BaseViewModel
    {


        #region Construction


        private CollectionImporter importer;
        private WindowsManager winManager;

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }
        private string title;

        public bool AsOwned
        {
            get { return asOwned; }
            set { asOwned = value; OnPropertyChanged(); }
        }
        private bool asOwned;

        public TxtImporterViewModel
        (
            CollectionImporter importer,
            WindowsManager winManager
        )
        {
            this.winManager = winManager;
            this.importer = importer;
        }

        #endregion

        #region Usage

        private void LaunchImportation(string content)
        {
            importer.AddImportToQueue(
                new PendingImport
                {
                    Content = content,
                    Title = title,
                    AsOwned = asOwned
                }
            );
            winManager.CloseWindow(this);
        }

        #endregion


    }
}

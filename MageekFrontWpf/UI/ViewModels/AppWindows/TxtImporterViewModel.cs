using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class ImportViewModel : BaseViewModel
    {

        private CollectionImporter importer;
        private WindowsManager winManager;

        public ImportViewModel
        (
            CollectionImporter importer,
            WindowsManager winManager
        )
        {
            this.winManager = winManager;
            this.importer = importer;
        }

        [ObservableProperty] private string title;
        [ObservableProperty] private bool asOwned;

        [RelayCommand]
        private void LaunchImportation(string content)
        {
            importer.AddImportToQueue(
                new PendingImport
                {
                    Content = content,
                    Title = Title,
                    AsOwned = AsOwned
                }
            );
            winManager.CloseWindow(AppWindowEnum.Import);
        }

    }
}

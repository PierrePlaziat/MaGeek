using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{
    public partial class ImportViewModel : BaseViewModel
    {

        private CollectionImporter importer;
        private WindowsService winManager;

        public ImportViewModel
        (
            CollectionImporter importer,
            WindowsService winManager
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

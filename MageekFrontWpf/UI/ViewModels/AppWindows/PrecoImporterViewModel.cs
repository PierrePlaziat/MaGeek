using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class PrecosViewModel : BaseViewModel
    {

        private const string path = "D:\\PROJECTS\\VS\\MaGeek\\Preco";
        private WindowsManager winManager;
        private CollectionImporter importer;
        private bool importingSeveral = false;

        public PrecosViewModel
        (
            CollectionImporter importer,
            WindowsManager winManager
        )
        {
            this.winManager = winManager;
            this.importer = importer;
        }

        [ObservableProperty] bool asOwned = false;
        [ObservableProperty] List<string> precoList = new();

        public async Task Init()
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var splited = file.Split("\\");
                string title = splited[splited.Length - 1].Replace(".txt", "");
                PrecoList.Add(title);
            }
            OnPropertyChanged(nameof(PrecoList));
        }

        private async Task ImportPrecos(List<string> titles)
        {
            importingSeveral = true;
            List<Task> tasks = new List<Task>();
            foreach (string preco in titles) ImportPreco(preco);
            importingSeveral = false;
            winManager.CloseWindow(AppWindowEnum.Precos);
        }
        
        [RelayCommand]
        private async Task ImportPreco(string title)
        {
            importer.AddImportToQueue(
                new PendingImport
                {
                    Title = "[Preco] " + title,
                    Content = File.ReadAllText(path + "\\" + title + ".txt"),
                    AsOwned = AsOwned
                }
            );
            if (!importingSeveral) winManager.CloseWindow(AppWindowEnum.Precos);
        }

    }
}

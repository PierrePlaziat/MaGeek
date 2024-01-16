using CommunityToolkit.Mvvm.ComponentModel;
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

        public ICommand ImportPrecoCommand;
        public ICommand ImportPrecosCommand;

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

        private void ListView_MouseDoubleClick(string selectedItem)
        {
            ImportPreco(selectedItem, AsOwned);
            winManager.CloseWindow(AppWindowEnum.Precos);
        }

        private void Button_Click(List<string> PrecoListViewSelectedItems, bool asOwned)
        {
            foreach (string preco in PrecoListViewSelectedItems) ImportPreco(preco, asOwned);
            winManager.CloseWindow(AppWindowEnum.Precos);
        }

        private void ImportPreco(string title, bool asOwned)
        {
            importer.AddImportToQueue(
                new PendingImport
                {
                    Title = "[Preco] " + title,
                    Content = File.ReadAllText(path + "\\" + title + ".txt"),
                    AsOwned = asOwned
                }
            );
        }

    }
}

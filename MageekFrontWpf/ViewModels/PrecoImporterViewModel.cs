using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekService;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MageekFrontWpf.ViewModels
{
    public class PrecoImporterViewModel : BaseViewModel
    {

        public PrecoImporterViewModel
        (
            CollectionImporter importer, 
            WindowsManager winManager
        ){
            this.winManager = winManager;
            this.importer = importer;
        }

        public ICommand ImportPrecoCommand;
        public ICommand ImportPrecosCommand;

        bool asOwned = false;
        public bool AsOwned
        {
            get { return asOwned; }
            set { asOwned = value; OnPropertyChanged(); }
        }

        string path = "D:\\PROJECTS\\VS\\MaGeek\\Preco";

        List<string> precoList = new List<string>();
        private WindowsManager winManager;
        private CollectionImporter importer;

        public List<string> PrecoList
        {
            get { return precoList; }
            set { precoList = value; OnPropertyChanged(); }
        }

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
            winManager.CloseWindow(this);
        }

        private void Button_Click(List<string> PrecoListViewSelectedItems, bool asOwned)
        {
            foreach (string preco in PrecoListViewSelectedItems) ImportPreco(preco, asOwned);
            winManager.CloseWindow(this);
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

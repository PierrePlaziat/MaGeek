using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekServices;
using MageekServices.Data.Collection.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{
    public partial class PrecoListViewModel : BaseViewModel
    {

        private const string path = "D:\\PROJECTS\\VS\\MaGeek\\Preco";
        private WindowsService winManager;
        private MageekService mageek;
        private bool importingSeveral = false;

        public PrecoListViewModel
        (
            WindowsService winManager,
            MageekService mageek
        )
        {
            this.winManager = winManager;
            this.mageek = mageek;
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
            //winManager.CloseWindow(AppWindowEnum.Precos);
        }
        
        [RelayCommand]
        private async Task ImportPreco(string title)
        {
            List<DeckCard> importLines = new();
            //importLines = await mageek.ParseCardList(content); // TODO recup preco content
            await mageek.CreateDeck_Contructed(
                title,
                "Preco",
                importLines
            );
            //TODO asOwned
        }

    }
}

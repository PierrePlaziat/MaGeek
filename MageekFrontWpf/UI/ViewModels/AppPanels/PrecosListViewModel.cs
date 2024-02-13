using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : BaseViewModel
    {

        private MageekService mageek;

        public PrecoListViewModel(MageekService mageek)
        {
            this.mageek = mageek;
        }

        [ObservableProperty] bool asOwned = false;
        [ObservableProperty] List<Tuple<string,string>> precoList = new();

        public void Reload() // TODO
        {
           
            //string[] files = Directory.GetFiles(Folders.PrecosFolder);
            //foreach (string file in files)
            //{
            //    var splited = file.Split("\\");
            //    string title = splited[splited.Length - 1].Replace(".txt", "");
            //    content
            //    PrecoList.Add(new PrecoDeck(title,content));
            //}
        }

        [RelayCommand]
        public async Task ImportPreco(string title) // TODO
        {
           
            //List<DeckCard> importLines = new();
            //importLines = await mageek.ParseCardList(PrecoDeck.CardList); // TODO recup preco content
            //await mageek.CreateDeck_Contructed(
            //    title,
            //    "Preco",
            //    importLines
            //);
        }

    }
}

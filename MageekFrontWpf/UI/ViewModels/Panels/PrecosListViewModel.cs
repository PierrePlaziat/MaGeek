using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Data;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : BaseViewModel
    {

        private MageekService mageek;
        private WindowsService win;

        public PrecoListViewModel(MageekService mageek, WindowsService win)
        {
            this.mageek = mageek;
            this.win = win;
            PrecoList = mageek.GetPrecos();
        }

        [ObservableProperty] List<Preco> precoList = new();

        // TODO : better system

        //[RelayCommand]
        //public async Task ImportPreco(Preco deck)
        //{
        //    await mageek.CreateDeck(deck);
        //}
        
        //[RelayCommand]
        //public async Task ImportPrecoAsOwned(Preco deck)
        //{
        //    await mageek.CreateDeck(deck);
        //}

        [RelayCommand]
        public async Task SelectDeck(Preco preco)
        {
            MageekDocumentInitArgs doc = new MageekDocumentInitArgs(preco: preco);
            win.OpenDoc(doc);
        }

    }

}

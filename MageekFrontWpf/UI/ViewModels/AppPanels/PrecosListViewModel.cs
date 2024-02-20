using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Data;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : BaseViewModel
    {

        private MageekService mageek;

        public PrecoListViewModel(MageekService mageek)
        {
            this.mageek = mageek;
            PrecoList = mageek.GetAllPrecos();
        }

        [ObservableProperty] List<Preco> precoList = new();
        [ObservableProperty] bool asOwned = false;

        [RelayCommand]
        public async Task ImportPreco(Preco deck)
        {
            await mageek.CreateDeck_Contructed(deck, asOwned);
        }

    }
}

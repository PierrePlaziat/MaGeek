using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Data;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data.Collection.Entities;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;

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
            PrecoList = mageek.GetAllPrecos();
        }

        [ObservableProperty] List<Preco> precoList = new();

        [RelayCommand]
        public async Task ImportPreco(Preco deck)
        {
            await mageek.CreateDeck_Contructed(deck, false);
        }
        
        [RelayCommand]
        public async Task ImportPrecoAsOwned(Preco deck)
        {
            await mageek.CreateDeck_Contructed(deck, true);
        }

        [RelayCommand]
        public async Task SelectDeck(Preco preco)
        {
            Deck deck = await mageek.CreateDeck_Contructed(preco,false);
            WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(deck.DeckId));
            win.OpenDoc(deck);
        }

    }

}

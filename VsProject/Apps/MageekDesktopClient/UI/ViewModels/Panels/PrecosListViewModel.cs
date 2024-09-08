using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Data;
using PlaziatWpf.Services;
using CommunityToolkit.Mvvm.Messaging;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;

namespace MageekDesktopClient.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : 
        ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private IMageekService mageek;
        private WindowsService win;

        public PrecoListViewModel(IMageekService mageek, WindowsService win)
        {
            this.mageek = mageek;
            this.win = win;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(LaunchAppMessage message)
        {
            InitPrecos().ConfigureAwait(false);
        }

        private async Task InitPrecos()
        {
            PrecoList = await mageek.Decks_Precos();
        }

        [ObservableProperty] List<Preco> precoList = new();

        [RelayCommand]
        public async Task SelectDeck(Preco preco)
        {
            DocumentArguments doc = new DocumentArguments(preco: preco);
            win.OpenDocument(doc);
        }

    }

}

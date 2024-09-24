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
using System.Linq;

namespace MageekDesktopClient.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : 
        ObservableViewModel,
        IRecipient<LaunchAppMessage>,
        IRecipient<OpenPrecoMessage>
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
        
        public void Receive(OpenPrecoMessage message)
        {
            SelectDeck(
                PrecoList.Where( 
                    x=> message.Value == string.Concat("[", x.Code, "] ", x.Title)
                ).FirstOrDefault()
            ).ConfigureAwait(false);
        }

        private async Task InitPrecos()
        {
            PrecoList = await mageek.Decks_Precos();
        }

        [ObservableProperty] List<Preco> precoList = new();

        [RelayCommand]
        public async Task SelectDeck(Preco preco)
        {
            if (preco == null) return;
            DocumentArguments doc = new DocumentArguments(preco: preco);
            win.OpenDocument(doc);
        }

    }

}

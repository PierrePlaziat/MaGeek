﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MageekCore.Data;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using CommunityToolkit.Mvvm.Messaging;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class PrecoListViewModel : ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private MageekService mageek;
        private WindowsService win;

        public PrecoListViewModel(MageekService mageek, WindowsService win)
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
            PrecoList = await mageek.GetPrecos();
        }

        [ObservableProperty] List<Preco> precoList = new();

        [RelayCommand]
        public async Task SelectDeck(Preco preco)
        {
            DocumentArguments doc = new DocumentArguments(preco: preco);
            win.OpenDoc(doc);
        }

    }

}

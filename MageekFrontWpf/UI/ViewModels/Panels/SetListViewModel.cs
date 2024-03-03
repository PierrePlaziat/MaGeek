using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore.Data.Mtg.Entities;
using PlaziatTools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{
    public partial class SetListViewModel : ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private MageekCore.MageekService mageek;

        public SetListViewModel(MageekCore.MageekService mageek)
        {
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] List<Sets> setList = new();
        [ObservableProperty] List<string> types = new();
        [ObservableProperty] string filterType = "All types";
        [ObservableProperty] List<string> blocks = new();
        [ObservableProperty] string filterBlock = "All blocks";
        [ObservableProperty] List<Cards> variants = new();

        public void Receive(LaunchAppMessage message)
        {
            Reload().ConfigureAwait(false);
        }

        [RelayCommand]
        public async Task Reload()
        {
            Logger.Log("Reload");
            SetList = mageek.LoadSets().Result.Where(x => FilterBlock == "All blocks" || x.Block == FilterBlock)
                                .Where(x => FilterType == "All types" || x.Type == FilterType).ToList();
        }

        public async void SelectSet(Sets s)
        {
            Variants = await mageek.GetCardsFromSet(s.Code);
        }

        public void SelectCard(Cards c)
        {
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(c.Uuid));
        }

    }
}
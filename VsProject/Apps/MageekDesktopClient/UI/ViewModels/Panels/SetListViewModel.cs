using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekCore.Data.Mtg.Entities;
using PlaziatTools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;
using MageekDesktopClient.UI.Views.AppPanels;

namespace MageekDesktopClient.UI.ViewModels.AppPanels
{
    public partial class SetListViewModel : ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private IMageekService mageek;

        public SetListViewModel(IMageekService mageek)
        {
            this.mageek = mageek;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] List<Sets> setCached = new();
        [ObservableProperty] List<Sets> setList = new();
        [ObservableProperty] List<string> types = new();
        [ObservableProperty] string filterType;
        [ObservableProperty] List<string> blocks = new();
        [ObservableProperty] string filterBlock;
        [ObservableProperty] List<Cards> variants = new();
        [ObservableProperty] bool isLoading = new();

        public void Receive(LaunchAppMessage message)
        {
            Init().ConfigureAwait(false);
        }

        private void FillTypes()
        {
            Types.Add("");
            foreach (var set in SetList) 
            {
                if (!Types.Contains(set.Type)) 
                {
                    Types.Add(set.Type);
                }
            }
            FilterType = "";
        }

        private void FillBlocks()
        {
            Blocks.Add("");
            foreach (var set in SetList)
            {
                if(set.Block!=null) //TODO why always null???
                {
                    if (!Blocks.Contains(set.Block))
                    {
                        Blocks.Add(set.Block);
                    }
                }
            }
            FilterBlock = "";
        }

        public async Task Init()
        {
            Logger.Log("Init");
            SetList = SetCached = await mageek.Sets_All();
            FillTypes();
            FillBlocks();
        }
        
        [RelayCommand]
        public async Task Reload()
        {
            Logger.Log("Reload");
            SetList = SetCached.Where(x => FilterBlock == "" || x.Block == FilterBlock)
                                .Where(x => FilterType == "" || x.Type == FilterType).ToList();
        }

        public async void SelectSet(Sets s)
        {
            IsLoading = true;
            var v = await mageek.Sets_Content(s.Code);
            var VVariants = new List<Cards>();
            foreach (var card in v)
            {
                VVariants.Add(await mageek.Cards_GetData(card));
            }
            Variants = VVariants;
            IsLoading = false;
        }

        public void SelectCard(Cards c)
        {
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(c.Uuid));
        }

    }
}
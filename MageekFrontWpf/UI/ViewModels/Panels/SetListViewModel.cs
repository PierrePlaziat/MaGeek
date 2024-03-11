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
using MageekCore.Service;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
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

        [ObservableProperty] List<Sets> setList = new();
        [ObservableProperty] List<string> types = new();
        [ObservableProperty] string filterType;
        [ObservableProperty] List<string> blocks = new();
        [ObservableProperty] string filterBlock;
        [ObservableProperty] List<Cards> variants = new();

        public void Receive(LaunchAppMessage message)
        {
            SetList = mageek.Sets_All().Result;
            FillTypes();
            FillBlocks();
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

        [RelayCommand]
        public async Task Reload()
        {
            Logger.Log("Reload");
            SetList = mageek.Sets_All().Result.Where(x => FilterBlock == "" || x.Block == FilterBlock)
                                .Where(x => FilterType == "" || x.Type == FilterType).ToList();
        }

        public async void SelectSet(Sets s)
        {
            Variants = await mageek.Sets_Content(s.Code);
        }

        public void SelectCard(Cards c)
        {
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(c.Uuid));
        }

    }
}
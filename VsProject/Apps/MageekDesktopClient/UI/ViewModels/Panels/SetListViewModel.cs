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
using PlaziatWpf.Services;
using MageekCore.Data;
using System.Collections.ObjectModel;

namespace MageekDesktopClient.UI.ViewModels.AppPanels
{
    public partial class SetListViewModel : ObservableViewModel,
        IRecipient<LaunchAppMessage>
    {

        private IMageekService mageek;
        private SessionBag bag;
        private SettingService config;

        public SetListViewModel(
            IMageekService mageek,
            SessionBag bag, 
            SettingService config
        ){
            this.mageek = mageek;
            this.bag = bag;
            this.config = config;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] List<Sets> setCached = new();
        [ObservableProperty] List<Sets> setList = new();
        [ObservableProperty] List<string> types = new();
        [ObservableProperty] string filterType;
        [ObservableProperty] List<string> blocks = new();
        [ObservableProperty] string filterBlock;
        [ObservableProperty] ObservableCollection<SearchedCards> variants = new();
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
            var v = await mageek.Sets_Content(bag.UserName, s.Code, "French");//TODO config.Settings[Setting.Translations.ToString()]);
            var VVariants = new ObservableCollection<SearchedCards>();
            foreach (var card in v)
            {
                card.Card = await mageek.Cards_GetData(card.CardUuid);
                VVariants.Add(card);

            }
            Variants = VVariants;
            IsLoading = false;
        }

        public void SelectCard(string uuid)
        {
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(uuid));
        }

        [RelayCommand]
        public async Task AddCardToCollection(string uuid)
        {
            await mageek.Collec_Move(bag.UserName, uuid, 1);
            var v = Variants.Where(x => x.CardUuid == uuid).First();
            v.Collected++;
            v.OnPropertyChanged("Collected");
        }

        [RelayCommand]
        public async Task SubstractCardFromCollection(string uuid)
        {
            await mageek.Collec_Move(bag.UserName, uuid, -1);
            var v = Variants.Where(x => x.CardUuid == uuid).First();
            v.Collected--;
            if (v.Collected < 1) v.Collected = 0;
            v.OnPropertyChanged("Collected");
        }

    }
}
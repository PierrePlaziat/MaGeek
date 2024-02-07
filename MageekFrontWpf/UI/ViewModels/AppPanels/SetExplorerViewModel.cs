using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekServices.Data.Mtg.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{
    public partial class SetListViewModel : BaseViewModel
    {

        private MageekServices.MageekService mageek;

        public SetListViewModel(MageekServices.MageekService mageek)
        {
            this.mageek = mageek;
        }

        [ObservableProperty] List<Sets> setList = new();
        [ObservableProperty] List<string> types = new();
        [ObservableProperty] string filterType = "All types";
        [ObservableProperty] List<string> blocks = new();
        [ObservableProperty] string filterBlock = "All blocks";
        [ObservableProperty] List<Cards> variants = new();

        private async void LoadSets()
        {
            SetList = await mageek.LoadSets();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetList = mageek.LoadSets().Result.Where(x => FilterBlock == "All blocks" || x.Block == FilterBlock)
                                .Where(x => FilterType == "All types" || x.Type == FilterType).ToList();
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = ((ListView)sender).SelectedItem as Sets;
            Variants = null;
            Variants = await mageek.GetCardsFromSet(s.Code);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var v = (DataGrid)sender;
            if (v.SelectedItem == null) return;
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage((v.SelectedItem as Cards).Uuid));
        }


        //private async void AddToDeck(object sender, RoutedEventArgs e)
        //{
        //    foreach (Cards c in CardGrid.SelectedItems)
        //    {
        //        await MageekService.MageekService.AddCardToDeck(c.Uuid, state.SelectedDeck, 1);
        //    }
        //    events.RaiseUpdateDeck();
        //}

    }
}
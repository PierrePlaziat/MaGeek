﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using MageekService;
using MageekService.Data.Mtg.Entities;

namespace MaGeek.UI
{
    public partial class SetExplorer : TemplatedUserControl
    {

        List<Sets> setList = new();
        public List<Sets> SetList
        {
            get { return setList; }
            set { 
                setList = value; 
                OnPropertyChanged(nameof(SetList));
            }
        }

        List<string> types = new();
        public List<string> Types
        {
            get { return types; }
            set {
                types = value; 
                OnPropertyChanged(nameof(types));
            }
        }

        string filterType = "All types";
        public string FilterType
        {
            get { return filterType; }
            set
            {
                filterType = value;
                OnPropertyChanged();
            }
        }

        List<string> blocks = new();
        public List<string> Blocks
        {
            get { return blocks; }
            set {
                blocks = value; 
                OnPropertyChanged();
            }
        }

        string filterBlock = "All blocks";
        public string FilterBlock
        {
            get { return filterBlock; }
            set
            {
                filterBlock = value;
                OnPropertyChanged();
            }
        }

        List<Cards> variants = new();
        public List<Cards> Variants
        {
            get { return variants; }
            set { variants = value; OnPropertyChanged(); }
        }

        public SetExplorer()
        {
            DataContext = this;
            InitializeComponent();
            LoadSets();
        }

        private async void LoadSets()
        {
            SetList = await MageekService.MageekService.LoadSets();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetList = (MageekService.MageekService.LoadSets().Result).Where(x => FilterBlock == "All blocks" || x.Block == FilterBlock)
                                .Where(x => FilterType == "All types" || x.Type == FilterType).ToList();
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = ((ListView)sender).SelectedItem as Sets;
            Variants = null;
            Variants = await MageekService.MageekService.GetCardsFromSet(s.Code);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var v = (DataGrid)sender;
            if (v.SelectedItem == null) return;
            App.Events.RaiseCardSelected((v.SelectedItem as Cards).Uuid);
        }


        private async void AddToDeck(object sender, RoutedEventArgs e)
        {
            foreach (Cards c in CardGrid.SelectedItems)
            {
                await MageekService.MageekService.AddCardToDeck(c.Uuid, App.State.SelectedDeck, 1);
            }
            App.Events.RaiseUpdateDeck();
        }



    }

}
using MaGeek.Entities;
using MaGeek.Framework.Utils;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System.Linq;

namespace MaGeek.UI
{
    public partial class SetExplorer : TemplatedUserControl
    {

        List<MtgSet> setList = new();
        public List<MtgSet> SetList
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

        List<CardVariant> variants = new();
        public List<CardVariant> Variants
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

        private void LoadSets()
        {
            using (var DB = App.DB.NewContext)
            {
                Types.Add("All types");
                foreach(var v in DB.Sets.GroupBy(x=>x.Type).Select(x => x.First()))
                {
                    Types.Add(v.Type);
                }
                Blocks.Add("All blocks");
                foreach (var v in DB.Sets.GroupBy(x=>x.Block).Select(x => x.First()))
                {
                    Blocks.Add(v.Block);
                }
                SetList =  DB.Sets.OrderBy(x=>x.ReleaseDate).ToList();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var DB = App.DB.NewContext)
            {
                SetList = DB.Sets.Where(x => FilterBlock == "All blocks" || x.Block == FilterBlock)
                                 .Where(x => FilterType == "All types" || x.Type == FilterType)
                                 .OrderBy(x => x.ReleaseDate).ToList();
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Variants = SelectSet(((ListView)sender).SelectedItem as MtgSet);
        }

        private List<CardVariant> SelectSet(MtgSet set)
        {
            List<CardVariant> variants = new();
            if (set != null) 
            {
                try
                {
                    using (var DB = App.DB.NewContext)
                    {
                        variants = DB.CardVariants.Where(x => x.SetName == set.Name)
                                                  .Include(x => x.Card)
                                                  .ToList();
                    }
                }
                catch (Exception e)
                {
                    Log.Write(e,"SelectSet");
                }
            }
            return variants;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = (DataGrid)sender;
            App.Events.RaiseCardSelected((v.SelectedItem as CardVariant).Card);
        }

    }

}

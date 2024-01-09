using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class DeckTable : BaseUserControl
    {

        public DeckTable(DeckTableViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            //ConfigureEvents();
        }

        private void UnsetCommandant_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}

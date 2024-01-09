using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using MageekFrontWpf;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;

namespace MaGeek.UI
{


    public partial class CardSearcher : BaseUserControl
    {

        public CardSearcher(CardSearcherViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            //FillColorFilterCombo();
            //App.Events.UpdateCardCollecEvent += async () => { await ReloadData(); };
        }

        private void FilterName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilterTag_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }


}

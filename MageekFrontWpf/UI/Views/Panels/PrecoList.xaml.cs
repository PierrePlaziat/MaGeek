﻿using MageekCore.Data;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using System.Windows.Controls;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class PrecoList : BaseUserControl
    {

        private PrecoListViewModel vm;

        public PrecoList(PrecoListViewModel vm)
        {
            DataContext = vm;
            this.vm = vm;
            InitializeComponent();
        }

        private void OpenDeck(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid d = (DataGrid)sender;
            vm.SelectDeck((Preco)d.SelectedItem).ConfigureAwait(false);
        }

        private void AddContentToCollec(object sender, System.Windows.RoutedEventArgs e)
        {
            //TODO
        }

    }

}

using System.Windows;
using System;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using System.Windows.Controls;
using MageekService.Data.Mtg.Entities;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekService;

namespace MageekFrontWpf.UI.Views.AppPanels
{


    public partial class CardSearcher : BaseUserControl
    {
        CardSearcherViewModel vm;
        public CardSearcher(CardSearcherViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
            FillColorFilterCombo();
        }

        private void FilterName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                vm.ReloadData().ConfigureAwait(false);
            }
        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CardGrid.SelectedItem is Cards card)
                WeakReferenceMessenger.Default.Send(new CardSelectedMessage(card.Uuid));
        }

        private void FillColorFilterCombo()
        {
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            vm.FilterName = ((MenuItem)e.OriginalSource).Header.ToString();
        }

    }


}

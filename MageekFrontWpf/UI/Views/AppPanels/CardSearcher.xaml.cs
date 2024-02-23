using System.Windows;
using System;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekCore.Data;

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
                vm.DoSearch().ConfigureAwait(false);
            }
        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var v = CardGrid.SelectedItem as SearchedCards;
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(v.Card.Uuid));
        }

        private void FillColorFilterCombo()
        {
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            vm.FilterName = ((MenuItem)e.OriginalSource).Header.ToString();
        }

        //private async void AddToDeck(object sender, RoutedEventArgs e)
        //{
        //    foreach (Cards c in CardGrid.SelectedItems)
        //    {
        //        await MageekService.MageekService.AddCardToDeck(c.Uuid, App.State.SelectedDeck, 1);
        //    }
        //    App.Events.RaiseUpdateDeck();
        //}

    }


}

using System.Windows.Input;
using System;
using MageekService.Data.Collection.Entities;

namespace MageekFrontWpf.AppValues
{

    public class ChangeCardRelationVariantCommand : ICommand
    {

        private readonly DeckCard relation;

        public ChangeCardRelationVariantCommand(DeckCard relation)
        {
            this.relation = relation;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public async void Execute(object parameter)
        {
            string variant = (string)parameter;
            await MageekService.MageekService.SwitchCardInDeck(relation, variant);
        }

        public event EventHandler CanExecuteChanged;

    }

}
using MaGeek.Data.Entities;
using System;
using System.Windows.Input;

namespace MaGeek.Events
{
    public class ChangeCardRelationVariantCommand : ICommand
    {

        private CardDeckRelation relation;

        public ChangeCardRelationVariantCommand(CardDeckRelation relation)
        {
            this.relation = relation;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            MagicCardVariant variant = (MagicCardVariant)parameter;
            App.Biz.Utils.ChangeRelation(relation, variant);
        }

        public event EventHandler CanExecuteChanged;
    }
}

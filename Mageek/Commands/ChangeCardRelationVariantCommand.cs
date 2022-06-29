using MaGeek.Data;
using MaGeek.Data.Entities;
using MaGeek.Entities;
using MaGeek.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaGeek.Commands
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
            App.cardManager.ChangeRelation(relation, variant);
        }

        public event EventHandler CanExecuteChanged;
    }
}

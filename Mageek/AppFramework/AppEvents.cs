using MaGeek.AppData.Entities;
using System.Windows.Input;
using System;
using MaGeek.AppBusiness;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Event Observer
    /// </summary>
    public class AppEvents
    {

        // Import local prevent UI Action
        public virtual void RaisePreventUIAction(bool on, string reason) { PreventUIActionEvent?.Invoke(on,reason); }
        public delegate void PreventUIActionHandler(bool on, string reason);
        public event PreventUIActionHandler PreventUIActionEvent;

        // Layout Action 
        public virtual void RaiseLayoutAction(LayoutEventType Type) { LayoutActionEvent?.Invoke(Type); }
        public delegate void LayoutActionHandler(LayoutEventType Type);
        public event LayoutActionHandler LayoutActionEvent;

        // Card Selected
        public virtual void RaiseCardSelected(MagicCard Card) { CardSelectedEvent?.Invoke(Card); }
        public delegate void CardSelectedHandler(MagicCard Card);
        public event CardSelectedHandler CardSelectedEvent;

        // Deck Selected
        public virtual void RaiseDeckSelect(MagicDeck deck) { SelectDeckEvent?.Invoke(deck); }
        public delegate void SelectDeckHandler(MagicDeck deck);
        public event SelectDeckHandler SelectDeckEvent;

        // Update Card Collec 
        public virtual void RaiseUpdateCardCollec() { UpdateCardCollecEvent?.Invoke(); }
        public delegate void UpdateCardCollecdHandler();
        public event UpdateCardCollecdHandler UpdateCardCollecEvent;

        // Update Deck 
        public virtual void RaiseUpdateDeck() { UpdateDeckEvent?.Invoke(); }
        public delegate void UpdateDeckHandler();
        public event UpdateDeckHandler UpdateDeckEvent;

        // Update Deck List
        public virtual void RaiseUpdateDeckList() { UpdateDeckListEvent?.Invoke(); }
        public delegate void UpdateDeckListHandler();
        public event UpdateDeckListHandler UpdateDeckListEvent;

    }
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
        public async void Execute(object parameter)
        {
            MagicCardVariant variant = (MagicCardVariant)parameter;
            await MageekUtils.ChangeVariant(relation, variant);
        }

        public event EventHandler CanExecuteChanged;
    }

}
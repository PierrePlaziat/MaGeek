using System.Windows.Input;
using System;
using MageekService.Data.Collection.Entities;
using MageekFrontWpf.App;

namespace MageekFrontWpf.Framework.Services
{

    /// <summary>
    /// Event Observer
    /// </summary>
    public class AppEvents
    {

        #region Avalon Layout Actions

        public virtual void RaiseLayoutAction(LayoutEventArgs args) { LayoutActionEvent?.Invoke(args); }
        public delegate void LayoutActionHandler(LayoutEventArgs args);
        public event LayoutActionHandler LayoutActionEvent;
        public class LayoutEventArgs
        {
            public LayoutEventType EventType { get; set; }
            public AppPanelEnum information { get; set; }
        }
        public enum LayoutEventType
        {
            OpenPanel,
            Save,
            Load,
        }

        #endregion

        // Card Selected
        public virtual void RaiseCardSelected(string cardUuid) { CardSelectedEvent?.Invoke(cardUuid); }
        public delegate void CardSelectedHandler(string cardUuid);
        public event CardSelectedHandler CardSelectedEvent;

        // Deck Selected
        public virtual void RaiseDeckSelect(string deckId) { SelectDeckEvent?.Invoke(deckId); }
        public delegate void SelectDeckHandler(string deckId);
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

    /// <summary>
    /// TODO Here is currently the only use of the Command design pattern
    /// Study if this should be used more wildly
    /// or maybe find a cleaner war in current project conventions
    /// </summary>
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
            string variant = (string)parameter; //TODO test
            await MageekService.MageekService.SwitchCardInDeck(relation, variant);
        }

#pragma warning disable CS0067 // L'événement 'ChangeCardRelationVariantCommand.CanExecuteChanged' n'est jamais utilisé
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // L'événement 'ChangeCardRelationVariantCommand.CanExecuteChanged' n'est jamais utilisé

    }

}
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System.Windows.Controls;
using System;

namespace MageekFrontWpf.UI.Views
{

    public partial class DeckDocument : BaseUserControl, IDocument
    {

        private DeckDocumentViewModel vm;

        public DeckDocument(DeckDocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        public void OpenDocument(Framework.BaseMvvm.AbstractDocumentArguments args)
        {
            DeckContentPanel.SetDataContext(vm);
            DeckStatsPanel.SetDataContext(vm);
            DeckTablePanel.SetDataContext(vm);
            vm.Initialize((Framework.AppValues.DocumentArguments)args).ConfigureAwait(false);
        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Grid item = (Grid)sender;
            var data = e.Data.GetData(typeof(string)) as string;
            string deckId = vm.Deck.Header.DeckId;
            WeakReferenceMessenger.Default.Send(
                new AddCardToDeckMessage(new Tuple<string, string>(deckId, data))
            );
        }
    }

}

﻿using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.UI.ViewModels;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Docking;
using System;
using MageekFrontWpf.Framework;

namespace MageekFrontWpf.UI.Views
{

    public partial class Document : BaseUserControl, IDocument
    {

        private DocumentViewModel vm;

        public Document(DocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        public void OpenDocument(AbstractDocumentArguments args)
        {
            DeckContentPanel.SetDataContext(vm);
            DeckStatsPanel.SetDataContext(vm);
            DeckTablePanel.SetDataContext(vm);
            vm.OpenDocument((DocumentArguments)args).ConfigureAwait(false);
        }

        private void DropCard(object sender, System.Windows.DragEventArgs e)
        {
            string cardUuid = e.Data.GetData(typeof(string)) as string;
            string deckId = vm.Deck.Header.DeckId;
            WeakReferenceMessenger.Default.Send(
                new AddCardToDeckMessage(
                    new Tuple<string, string>(deckId, cardUuid)
                )
            );
        }

    }

}
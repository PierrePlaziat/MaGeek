using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System;

namespace MageekFrontWpf.UI.Views
{

    public partial class DeckDocument : BaseUserControl
    {

        private DeckDocumentViewModel vm;

        public DeckDocument(DeckDocumentViewModel vm) 
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        public void Initialize(Deck deck)
        {
            DeckContentPanel.SetDataContext(vm);
            DeckStatsPanel.SetDataContext(vm);
            DeckTablePanel.SetDataContext(vm);
            vm.Initialize(deck).ConfigureAwait(false);
        }

        public void Initialize(Preco preco)
        {
            DeckContentPanel.SetDataContext(vm);
            DeckStatsPanel.SetDataContext(vm);
            DeckTablePanel.SetDataContext(vm);
            vm.Initialize(preco).ConfigureAwait(false);
        }

    }

}

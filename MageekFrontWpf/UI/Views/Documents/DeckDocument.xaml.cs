using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

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

        public void Initialize(DocumentInitArgs args)
        {
            DeckContentPanel.SetDataContext(vm);
            DeckStatsPanel.SetDataContext(vm);
            DeckTablePanel.SetDataContext(vm);
            vm.Initialize((AppDocumentInitArgs)args).ConfigureAwait(false);
        }

    }

}

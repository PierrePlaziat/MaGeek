using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using System;
using System.Windows;

namespace MaGeek.UI.Windows.Importers
{

    public partial class PrecoImporter : BaseWindow
    {

        PrecoImporterViewModel vm;

        public PrecoImporter(PrecoImporterViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            vm.Init().ConfigureAwait(false);
        }

    }

}

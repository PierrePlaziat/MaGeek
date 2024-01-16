using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System;
using System.Windows;

namespace MaGeek.UI.Windows.Importers
{

    public partial class PrecosWindow : BaseWindow
    {

        PrecosViewModel vm;

        public PrecosWindow(PrecosViewModel vm)
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

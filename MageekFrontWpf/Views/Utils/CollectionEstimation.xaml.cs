using MageekFrontWpf;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using MageekService.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MaGeek.UI
{

    public partial class CollectionEstimation : BaseWindow
    {

        public CollectionEstimation(CollectionEstimationViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}

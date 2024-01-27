using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekService.Data.Collection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckStats : BaseUserControl
    {

        public DeckStats(DeckStatsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}

﻿using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckTable : BaseUserControl
    {

        private DeckDocumentViewModel vm;

        public DeckTable() {}

        public void SetDataContext(DeckDocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void ToCommandant_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ToSide_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void AddOne_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void RemoveOne_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

}

using MageekDesktopClient.UI.ViewModels;
using PlaziatWpf.Mvvm;
using System.Windows;

namespace MageekDesktopClient.UI.Views.AppPanels
{

    public partial class DeckTable : BaseUserControl
    {

        public static readonly DependencyProperty ControlWidthProperty =
            DependencyProperty.Register("ControlWidth", typeof(double), typeof(DeckTable), new PropertyMetadata(120.0));

        public static readonly DependencyProperty ControlHeightProperty =
            DependencyProperty.Register("ControlHeight", typeof(double), typeof(DeckTable), new PropertyMetadata(165.0));

        double zoomFactor = 1.1;

        public double ControlWidth
        {
            get { return (double)GetValue(ControlWidthProperty); }
            set { SetValue(ControlWidthProperty, value); }
        }

        public double ControlHeight
        {
            get { return (double)GetValue(ControlHeightProperty); }
            set { SetValue(ControlHeightProperty, value); }
        }

        private DocumentViewModel vm;

        public DeckTable() {}

        public void SetDataContext(DocumentViewModel vm)
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

        private void ZommOut_Click(object sender, RoutedEventArgs e)
        { 
            ControlWidth /= zoomFactor;
            ControlHeight /= zoomFactor;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ControlWidth *= zoomFactor;
            ControlHeight *= zoomFactor;
        }
    }

}

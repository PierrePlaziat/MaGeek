using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI.Controls
{

    public partial class BusyIndicator : UserControl
    {

        public static readonly DependencyProperty MessageProperty = 
            DependencyProperty.Register(
                "Message", 
                typeof(string),
                typeof(BusyIndicator)
        );

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public BusyIndicator()
        {
            InitializeComponent();
        }

    }

}

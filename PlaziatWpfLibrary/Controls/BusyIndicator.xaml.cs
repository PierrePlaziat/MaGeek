using System.Windows;

namespace PlaziatWpf.Controls
{

    public partial class BusyIndicator : System.Windows.Controls.UserControl
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

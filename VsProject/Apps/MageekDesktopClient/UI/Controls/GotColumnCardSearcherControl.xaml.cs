using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using System.Threading.Tasks;
using System.Windows;

namespace MageekDesktopClient.UI.Controls
{

    public partial class GotColumnCardSearcherControl : BaseUserControl
    {

        private IMageekService mageek;
        private SessionBag bag;

        public string gotValue = "-1";
        public string GotValue 
        {
            get { return gotValue; }
            set { gotValue = value; OnPropertyChanged(); }
        }
        
        public string Card
        {
            get { 
                return (string)GetValue(CardProperty); 
            }
            set { 
                SetValue(CardProperty, value); 
            }
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
                nameof(Card), typeof(string), typeof(GotColumnCardSearcherControl),
                new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnCardChange)
        );

        public GotColumnCardSearcherControl()
        {
            mageek = ServiceHelper.GetService<IMageekService>();
            bag = ServiceHelper.GetService<SessionBag>();
            InitializeComponent();
            DataContext = this;
        }

        private static void OnCardChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                GotColumnCardSearcherControl control = d as GotColumnCardSearcherControl;
                control.LoadGotValueAsync(e.NewValue as string).ConfigureAwait(false);
            }
        }

        private async Task LoadGotValueAsync(string newValue)
        {
            GotValue = (await mageek.Collec_OwnedCombined(bag.UserName,newValue)).ToString(); 
            OnPropertyChanged(nameof(GotValue));
        }

    }
}

using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{

    public partial class ManaCost : UserControl
    {


        private string cost;
        public string Cost
        {
            get { return cost; }
            set { cost = value; SetValue(CostProperty, value);}
        }

        public static readonly DependencyProperty CostProperty = DependencyProperty.Register(
            "Cost", typeof(string), typeof(ManaCost),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnCostChanged)
        );

        private static void OnCostChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (ManaCost)_control;
            //control.EmptyPanel();
            if (eventArgs.NewValue != null)
                control.UpdatePanel(eventArgs.NewValue.ToString());
        }


        public ManaCost()
        {
            InitializeComponent();
        }

        private void UpdatePanel(string costString)
        {
            string resultString = "";
            var costTable = costString.Split('}');
            foreach (string s in costTable)
            {
                if (s!="")
                {
                    string code = s.Substring(1);
                    code = code.Replace('/', ' ');
                         if (code == "B G") resultString     += "D";
                    else if (code == "B P") resultString     += "S";
                    else if (code == "B R") resultString     += "V";
                    else if (code == "B") resultString       += "b";
                    else if (code == "B2") resultString      += "b/";
                    else if (code == "G P") resultString     += "S";
                    else if (code == "G U") resultString     += "K";
                    else if (code == "G W") resultString     += "E";
                    else if (code == "G") resultString       += "g";
                    else if (code == "G2") resultString      += "g/";
                    else if (code == "R G") resultString     += "J";
                    else if (code == "R P") resultString     += "S";
                    else if (code == "R W") resultString     += "F";
                    else if (code == "R") resultString       += "r";
                    else if (code == "R2") resultString      += "r/";
                    else if (code == "U B") resultString     += "H";
                    else if (code == "U P") resultString     += "S";
                    else if (code == "U R") resultString     += "S";
                    else if (code == "U") resultString       += "u";
                    else if (code == "U2") resultString      += "u/";
                    else if (code == "W B") resultString     += "M";
                    else if (code == "W P") resultString     += "S";
                    else if (code == "W U") resultString     += "e";
                    else if (code == "W") resultString       += "w";
                    else if (code == "W2") resultString      += "w/";
                    else if (code == "X") resultString       += "x";
                    else if (code == "R") resultString += "r";
                    else if (code == "C") resultString += "*";
                    else resultString += code;
                }
            }
            ManaTxt.Content = resultString;
        }

        /*private void AddMana(string code)
        {
            code = code.Replace('/', ' ');
            CostPanel.Children.Add(new Image () { Source=ResourcesAccess.GetSymbol(code),Margin=new Thickness(1), Width=14});
        }

        private void EmptyPanel()
        {
            CostPanel.Children.Clear();
        }*/
    }
}

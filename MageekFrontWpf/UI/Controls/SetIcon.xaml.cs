using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.XPath;
using MageekCore.Data;

namespace MaGeek.UI.Controls
{

    public partial class SetIcon : UserControl
    {


        #region Dependancy Properties

        #region SetCode

        private string setCode;
        public string SetCode
        {
            get { return setCode; }
            set { setCode = value; SetValue(SetCodeProperty, value);}
        }

        public static readonly DependencyProperty SetCodeProperty = DependencyProperty.Register(
            "SetCode", typeof(string), typeof(SetIcon),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnSetCodeChanged)
        );

        private static void OnSetCodeChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (SetIcon)_control;
            if (eventArgs.NewValue != null)
                control.UpdateIcon();
        }

        #endregion
        
        #region RarityColor

        private string rarity;
        public string Rarity
        {
            get { return rarity; }
            set { rarity = value; SetValue(RarityProperty, value);}
        }

        public static readonly DependencyProperty RarityProperty = DependencyProperty.Register(
            "Rarity", typeof(string), typeof(SetIcon),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnRarityColorChanged)
        );

        private static void OnRarityColorChanged(DependencyObject _control, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (SetIcon)_control;
            if (eventArgs.NewValue != null)
                control.UpdateIcon();
        }

        #endregion
        
        #endregion

        public SetIcon()
        {
            InitializeComponent();
        }

        private void UpdateIcon()
        {
            svgViewBox.SvgSource = GetSvgContent(GetSvgPath(), GetColor());
        }

        private string GetSvgContent(string path, string color)
        {
            XElement root = XElement.Load(path);
            var colors = root.XPathSelectElements("//*[@fill]");
            foreach (XElement node in colors)
                node.Attribute("fill").Value = color;
            return root.Value;
        }

        private string GetSvgPath()
        {
            string s = Path.Combine(Folders.SetIcon, SetCode + "_.svg");
            if (File.Exists(s)) return s;
            else return "wut.svg";
        }

        private string GetColor()
        {
            //TODO fine tune
            return Rarity switch
            {
                "common" => "#ffffff",
                "uncommon" => "#777777",
                "rare" => "#123456",
                "mythic" => "#654321",
                "bonus" => "#765183",
                _ => "#461857",
            };
        }

    }
}

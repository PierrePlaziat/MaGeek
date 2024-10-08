using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MageekCore.Data;

namespace MageekDesktopClient.UI.Controls
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
                control.UpdateIcon((string)eventArgs.NewValue);
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
                control.UpdateRarity((string)eventArgs.NewValue);
        }

        #endregion
        
        #endregion

        public SetIcon()
        {
            InitializeComponent();
        }

        private void UpdateIcon(string code)
        {
            svgViewBox.Source = GetSvgPath(code);
        }

        private void UpdateRarity(string newValue)
        {
            Rarity = newValue;
            BG.Background = GetColor(newValue);
        }

        private Uri GetSvgPath(string code)
        {
            string s = Path.Combine(Paths.Folder_SetIcons, code + "_.svg");
            if (File.Exists(s)) return new Uri(s);
            else return null;
        }

        private Brush GetColor(string Rarity)
        {
            return Rarity switch
            {
                "common" => Brushes.AliceBlue,
                "uncommon" => Brushes.DimGray,
                "rare" => Brushes.Gold,
                "mythic" => Brushes.DarkOrange,
                "bonus" => Brushes.MediumPurple,
                _ => Brushes.MediumOrchid,
            };
        }

    }
}

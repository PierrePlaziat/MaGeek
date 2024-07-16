using System.Windows.Documents;
using PlaziatTools;
using System.Windows.Media;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace PlaziatWpf.Controls.TextBoxHelpers
{
    public static class RichTextBoxExtension
    {
        public static void AppendText(this RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            tr.ApplyPropertyValue(TextElement.FontFamilyProperty, "Consolas");
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }
    }
}
